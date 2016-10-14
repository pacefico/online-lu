/*
#define _SIZE_T_DEFINED
#ifndef __CUDACC__
#define __CUDACC__
#endif
#ifndef __cplusplus
#define __cplusplus
#endif
*/

#define THREADS_PER_BLOCK 64
#define _SIZE_T_DEFINED
#ifndef __cplusplus
#define __cplusplus
#endif
#ifndef __CUDACC__  
    #define __CUDACC__
#endif

#include <cuda.h>
#include <math_functions.h>
#include <device_launch_parameters.h>
//#include <texture_fetch_functions.h>
#include "float.h"
#include <builtin_types.h>
#include <vector_functions.h>
#include <device_functions.h>

//#include <cuda_runtime_api.h>
extern "C"  
{
	// Scale v2, Several blocks with 64 (LUDBLOCK_SCALE_BLOCKSIZE) threads
	__global__ void lud_block_scale_v2(float *a, int M, int k, int end)
	{
		extern __shared__ float ac[];

		int aWidth = M;
		int tid = blockIdx.x * blockDim.x + threadIdx.x;
		ac[threadIdx.x] = a[k * aWidth + k + threadIdx.x]; // Load k row to shared memory, as it is used across threads

		// Sync threads to make sure all other also have loaded values
		__syncthreads();

		//for(int i = k+1 + tx; i < M; i+=blockDim.x) { // Foreach row
		int i = k+1 + tid;
		if (i < M)
		{
			 // Compute scale factor Rik, 1 operation=divide
			float rik = (a[i * aWidth + k] /= ac[0]); // CGMA: 0.5

			for (int c = 1; c < end-k; c++)  // Foreach column value in row
				a[i * aWidth + k + c] -= rik * ac[c]; // CGMA: 1.0
		}
	}
	// Tri.solve v1, Several blocks with 64 (LUDBLOCK_TRIANGULAR_BLOCKSIZE) threads
	__global__ void lud_block_triangular_solve(float *a, int M, int k, int LU_BlockDim)
	{
		extern __shared__ float y[];

		int tx = threadIdx.x;
		int tid = blockIdx.x * blockDim.x + tx;
		int column = tid + k + LU_BlockDim;

		if (column < M)
		{
			// Calculate values and write to shared memory array
			for (int r = 0; r < LU_BlockDim; r++) // For each row in block
			{
				float res = a[(r+k) * M + column];
				for (int c = 0; c < r; c++) // 0<=c<r, so below diagonal
					res -= a[(r+k) * M + c + k] * y[tx * LU_BlockDim + c];
				y[tx * LU_BlockDim + r] = res;
			}

			// Copy values from shared memory to global memory
			for (int r = 0; r < LU_BlockDim; r++)
				a[(r+k) * M + column] = y[tx * LU_BlockDim + r];
		}
	}
	// Mat.mult v1, Several blocks with LU_BlockDim x LU_BlockDim threads
	__global__ void lud_block_matrixMultiplication(float *a, int M, int k, int maxDimThreads) {
   
		// Declare cache
		extern __shared__ float shared[];
        
		float* ac = (float*)shared; // Tiling dim should be equal to blockDim.x and blockDim.y
		float* bc = (float*)&shared[blockDim.x*blockDim.y]; // Tiling dim should be equal to blockDim.x and blockDim.y
    
		// Coordinates + row/column end
		const int txid = blockIdx.x * blockDim.x + threadIdx.x; // Column
		const int tyid = blockIdx.y * blockDim.y + threadIdx.y; // Row
    
		float av = 0, bv = 0;

		// Data prefetching + calculate the first index in of row in a.
		if (tyid < maxDimThreads) av = a[(tyid+k+blockDim.y) * M + threadIdx.x + k];

		// Insert register value to shared
		ac[threadIdx.y * blockDim.y + threadIdx.x] = av;

		// Data prefetch + calculate the first index in of column in b.
		if (txid < maxDimThreads) bv = a[(k+threadIdx.y) * M + k + blockDim.x + txid];

		// Calculate index in c, latency hiding
		const int cidx = (tyid+k+blockDim.y) * M + txid+k+blockDim.x;
    
		// Insert register value to shared
		bc[threadIdx.y * blockDim.y + threadIdx.x] = bv;

		// Synchronze to make sure all threads in block have saved values to the shared memory for this phase
		__syncthreads();

		if (txid < maxDimThreads && tyid < maxDimThreads)
		{    
			float sum = 0.0;

			// Calculate the dot-product
			for (int i=0; i < blockDim.x; ++i) {
				sum += ac[threadIdx.y * blockDim.y + i]*bc[i * blockDim.y + threadIdx.x];
			}
        
			// Synchronise to make sure that computation are done
			__syncthreads();
        
			// Insert dot-product in resulting matrix
			a[cidx] -= sum;
		}
	}

	/* Pivot optimised for O(log(N)). Needs 2 kernels. First for reduction on matrix A, 
   sequential reductions are performed on array. TODO: Optimise */
	__global__ void lud_block_pivot(int *out, float *a, int M, int k, int max)
{
    extern __shared__ float shared[];
	float* max_cache = (float*)shared;
	int* idx_cache = (int*)&shared[blockDim.x];

    unsigned int tx = threadIdx.x;
    unsigned int i = blockIdx.x * blockDim.x + tx + k; // Get row index
    
	unsigned int idx = i * M;// + k;

	// Clear cache for threads that exceeds max + they should not influence result
	max_cache[tx] = 0;
	idx_cache[tx] = -1;

	if (i < M)
	{
		// Read value + set row index
		max_cache[tx] = abs(a[idx + k]);
		idx_cache[tx] = i;

		// Sync threads to make sure all other also have loaded values
		__syncthreads();

		// Do the actual pivot finding
		for(unsigned int stride = blockDim.x/2; stride>0; stride>>=1)
		{
			if (tx < stride && (stride+tx+k) < M && max_cache[tx] < max_cache[tx + stride])
			{
				max_cache[tx] = max_cache[tx + stride]; // Update value
				idx_cache[tx] = idx_cache[tx + stride]; // Update index
			}

			// Sync threads
			__syncthreads();
		}

		// The first thread should write result from block to output
		if (tx == 0)
		{
			//out[blockIdx.x][0] = max_cache[0][0]; // Load value to output
			out[blockIdx.x] = idx_cache[0]; // Load index to output
		}
	}
}
	__global__ void lud_block_pivot_L2(int *val, float *a, int M, int k, int max)
	{
		extern __shared__ float shared[];
		float* max_cache = (float*)shared;
		int* idx_cache = (int*)&shared[blockDim.x];

		unsigned int tid = blockIdx.x * blockDim.x + threadIdx.x;

		unsigned int tx = threadIdx.x;
		unsigned int i = blockIdx.x * blockDim.x + tx; // Get row index
    
		//unsigned int idx = i * M + k;

		// Clear cache for threads that exceeds max + they should not influence result
		max_cache[tx] = 0;

		if (tid < max)
		{
			// Read value + set row index
			int v = val[i];
			max_cache[tx] = abs(a[v * M + k]);
			idx_cache[tx] = v;

			// Sync threads to make sure all other also have loaded values
			__syncthreads();

			// Do the actual pivot finding
			for(unsigned int stride = blockDim.x/2; stride>0; stride>>=1)
			{
				if (tx < stride && max_cache[tx] < max_cache[tx + stride])
				{
					max_cache[tx] = max_cache[tx + stride]; // Update value
					idx_cache[tx] = idx_cache[tx + stride]; // Update index
				}

				// Sync threads
				__syncthreads();
			}

			// The first thread should write result from block to output
			if (tx == 0)
			{
				//out[blockIdx.x][0] = max_cache[0]; // Load value to output
				val[blockIdx.x] = idx_cache[0]; // Load index to output
			}
		}
	}
	// Swap v2, several blocks with 64 (LUDBLOCK_SWAP_BLOCKSIZE2) threads
	__global__ void lud_block_swap_v2(float *a, int M, int r1, int r2)
	{
		int column = blockIdx.x * blockDim.x + threadIdx.x;

		if (column < M)
		{
			float tmp = a[r1 * M + column];
			a[r1 * M + column] = a[r2 * M + column];
			a[r2 * M + column] = tmp;
		}
	}


}

