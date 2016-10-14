using ManagedCuda;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ManagedCuda.VectorTypes;

namespace OnlineLU.Client.ConsoleService
{
    public class LuMatrix
    {
        public float[] n { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public int size { get; set; }
        public bool hasPivot { get; set; }
        public int[] pivot { get; set; }

        public LuMatrix(int Width, int Height, bool HasPivot)
        {
            this.width = Width;
            this.height = Height;
            this.size = Width * Height;
            this.hasPivot = HasPivot;
            this.pivot = new int[Height];
            this.n = new float[this.size];
        }
    }

    public class LuCuda
    {
        static readonly int LUDBLOCK_TRIANGULAR_BLOCKSIZE = 64;
        static readonly int LUDBLOCK_PIVOT_BLOCKSIZE = 64;
        static readonly int LUDBLOCK_SWAP_BLOCKSIZE2 = 64;

        static CudaContext ctx;
        static Stream stream;
        private CudaDeviceVariable<float> d_lu;
        private CudaKernel lud_block_scale_v2;
        private CudaKernel lud_block_pivot;
        private CudaKernel lud_block_pivot_L2;
        private CudaKernel lud_block_swap_v2;

        public LuCuda()
        {
            //Init Cuda context
            ctx = new CudaContext(CudaContext.GetMaxGflopsDeviceId());

            //Load Kernel image from resources
            string resName;
            if (IntPtr.Size == 8)
                resName = "LuCuda_x64.ptx";
            else
                resName = "LuCuda.ptx";

            //string resNamespace = "vectorAdd";
            string resNamespace = "OnlineLU.Client.ConsoleService";
            string resource = resNamespace + "." + resName;
            stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource);
            if (stream == null) throw new ArgumentException("Kernel not found in resources.");

            lud_block_scale_v2 = ctx.LoadKernelPTX(stream, "lud_block_scale_v2");
            lud_block_pivot = ctx.LoadKernelPTX(stream, "lud_block_pivot");
            lud_block_pivot_L2 = ctx.LoadKernelPTX(stream, "lud_block_pivot_L2");
            lud_block_swap_v2 = ctx.LoadKernelPTX(stream, "lud_block_swap_v2");
        }

        public void ResolveLuBlock(ref LuMatrix luMatrix)
        {
            int blockDimension = 100;
            d_lu = new CudaDeviceVariable<float>(luMatrix.size);

            // Allocate memory on GPU for LU matrix
            CudaDeviceVariable<int> d_pivot = luMatrix.pivot;

            // Copy matrix A vector values to the LU pointer on the GPU/device = A copy to LU (will be modified)
            d_lu.CopyToDevice(luMatrix.n);
            //ctx.CopyToDevice(d_lu.DevicePointer, );

            int n = luMatrix.height;

            for (int i = 0; i < n; i += blockDimension)
            {
                int realBlockSize = n - i < blockDimension ? n - i : blockDimension;

                //decompose the current rectangular block + include pivot of all rows k+i
                decomposeLU(n, i, realBlockSize, d_pivot);
                //triangularSolveInPlace(d_lu, n, i, blockDimension, version);
                //matrixMultiplyInPlace(d_lu, n, i, blockDimension, version);
            }
            d_pivot.Dispose();
            luMatrix.n = d_lu;
        }

        private void decomposeLU(int M, int i, int blockDimension, CudaDeviceVariable<int> pivot)
        {
		    int end = i + blockDimension;
	        int threads = blockDimension;
            
	        for (int k = i; k < end; k++) // Foreach column
	        {
                // Pivoting
		        pivot[k] = pivotRow(M, k);
		        swapRows(M, pivot[k], k);

                // Execute LUD Block Column scaling
		        int memsize = sizeof(float)*blockDimension;

		        // version = 2
		        threads = M-k;
                int blocks = (threads + blockDimension-1) / blockDimension;

                lud_block_scale_v2.BlockDimensions = blockDimension;
                lud_block_scale_v2.GridDimensions = blocks;
                lud_block_scale_v2.DynamicSharedMemory = (uint)memsize;
                lud_block_scale_v2.Run(d_lu.DevicePointer, M, k, Math.Min(end, M));
		        //lud_block_scale_v2<<< blocks, blockDimension, memsize >>>(a, M, k, min(end, M) );
	        }
	        //free(lu);
        }

        private int pivotRow(int M, int k)
        {
	        int threads = M-k;
            int blocks = (threads + LUDBLOCK_PIVOT_BLOCKSIZE-1) / LUDBLOCK_PIVOT_BLOCKSIZE;

	        dim3 dimBlock = new dim3(LUDBLOCK_PIVOT_BLOCKSIZE, 1, 1);
            dim3 dimGrid = new dim3(blocks, 1, 1);
            int smemSize = LUDBLOCK_PIVOT_BLOCKSIZE*sizeof(int)*2;

	        CudaDeviceVariable<int> d_out = new CudaDeviceVariable<int>(dimGrid.x);

            // First run on a, subsequential will be on d_out
            lud_block_pivot.BlockDimensions = dimBlock;
            lud_block_pivot.GridDimensions = dimGrid;
            lud_block_pivot.DynamicSharedMemory = (uint)smemSize;
            lud_block_pivot.Run(d_out.DevicePointer, d_lu.DevicePointer, M, k, M*M);
	        //lud_block_pivot<<< dimGrid, dimBlock, smemSize >>>(d_out, a, M, k, M*M);

            int[] h_out = new int[dimGrid.x];
            
            while(blocks > 1)
	        {
		        // Adjust the number of required blocks, for the second round
		        threads = blocks;
		        blocks = threads > LUDBLOCK_PIVOT_BLOCKSIZE
			        ? (threads + LUDBLOCK_PIVOT_BLOCKSIZE-1) / LUDBLOCK_PIVOT_BLOCKSIZE
			        : 1;

		        dimGrid.x = (uint)blocks;
                
                lud_block_pivot_L2.BlockDimensions = dimBlock;
                lud_block_pivot_L2.GridDimensions = dimGrid;
                lud_block_pivot_L2.DynamicSharedMemory = (uint)smemSize;
                lud_block_pivot_L2.Run(d_out.DevicePointer, d_lu.DevicePointer, M, k, M * M);
		        //lud_block_pivot_L2<<< dimGrid, dimBlock, smemSize >>>(d_out, a, M, k, threads);
	        }
            ctx.Synchronize();
            h_out = d_out;
            
            d_out.Dispose();
	        return h_out[0]; // The highest index is in the first location
        }

        private void swapRows(int M, int r1, int r2)
        {
	        if (r1 != r2) // Only swap if there are any difference
	        {
			    int blocks = (M + LUDBLOCK_SWAP_BLOCKSIZE2-1) / LUDBLOCK_SWAP_BLOCKSIZE2;

                lud_block_swap_v2.BlockDimensions = LUDBLOCK_SWAP_BLOCKSIZE2;
                lud_block_swap_v2.GridDimensions = blocks;

                lud_block_swap_v2.Run(d_lu.DevicePointer, M, r1, r2);
                //lud_block_swap_v2<<< blocks, LUDBLOCK_SWAP_BLOCKSIZE2 >>>(a, M, r1, r2);
	        }
        }

        private void triangularSolveInPlace(CudaDeviceVariable<float> a, int M, int k, int LU_BlockDim, int version)
        {
	        int threads = M-k-LU_BlockDim;
	        if (threads <= 0) return;
	
            int blocks = (threads + LUDBLOCK_TRIANGULAR_BLOCKSIZE-1) / LUDBLOCK_TRIANGULAR_BLOCKSIZE;
            int smemSize = LU_BlockDim*LUDBLOCK_TRIANGULAR_BLOCKSIZE*sizeof(float);

	        if (version <= 4)
	        {
                CudaKernel lud_block_triangular_solve = ctx.LoadKernelPTX(stream, "lud_block_triangular_solve");
                lud_block_triangular_solve.BlockDimensions = LUDBLOCK_TRIANGULAR_BLOCKSIZE;
                lud_block_triangular_solve.GridDimensions = blocks;
                lud_block_triangular_solve.Run(a.DevicePointer, M, k, LU_BlockDim);
		        //lud_block_triangular_solve<<< blocks, LUDBLOCK_TRIANGULAR_BLOCKSIZE, smemSize >>>(a, M, k, LU_BlockDim);
	        }
            ctx.Synchronize();
	        //cudaThreadSynchronize();
        }

        private void matrixMultiplyInPlace(CudaDeviceVariable<float> a, int M, int k, int LU_BlockDim, int version)
        {
	        // Block dim size must match LU_BlockDim, and phases is not necessary
            int threadsPerDim = M-k-LU_BlockDim;
	        if (threadsPerDim <= 0) return;

            int gridX = (threadsPerDim + LU_BlockDim-1) / LU_BlockDim;
	        int gridY = gridX; // It is square

            dim3 dimBlock = new dim3(LU_BlockDim, LU_BlockDim, 1);
            dim3 dimGrid = new dim3(gridX, gridY, 1);
            
            //dim3 dimBlock(LU_BlockDim, LU_BlockDim, 1);
            //dim3 dimGrid(gridX, gridY, 1);
            int smemSize = LU_BlockDim*LU_BlockDim*sizeof(float)*2;

            if (version <= 2)
            {
                CudaKernel lud_block_matrixMultiplication = ctx.LoadKernelPTX(stream, "lud_block_matrixMultiplication");
                lud_block_matrixMultiplication.BlockDimensions = LU_BlockDim;
                lud_block_matrixMultiplication.GridDimensions = gridX;
                
                lud_block_matrixMultiplication.Run(a.DevicePointer, M, k, threadsPerDim);
                //lud_block_matrixMultiplication<<< dimGrid, dimBlock, smemSize >>>(a, M, k, threadsPerDim);
            }
            
	        //cudaThreadSynchronize();
            ctx.Synchronize();
        }

        static void LuPrint(ref float[] A, int ordem)
        {
            if (true)
            {
                int j = 0;
                for (int i = 0; i < A.Length; i++)
                {   
                    Console.Write("{0:N6}", A[i]);
                    Console.Write("  ");
                    j++;
                    
                    if (j == ordem)
                    {
                        j = 0;
                        Console.Write("\n");
                    }
                }
            }
        }

    }
}
