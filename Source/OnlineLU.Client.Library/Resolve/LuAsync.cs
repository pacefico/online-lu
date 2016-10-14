using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OnlineLU.Client.Library.Resolve
{
    public class LuAsync
    {
        private static object m_lock = new object();
        private float[][] m_arrayMaxFromThreads;

        private static int nTasks = 100;
        private static int N;

        public LuAsync()
        {
            
        }

        public void ResolveLU(float[][] A)
        {
            N = A[0].Length;
            for (int i = 0; i < N - 1; i++)
            {
                ResolveMax(A, i);
                
                float _piv = A[i][i];
                int itemsToProcess = N - 1 - i; //threads reduzidas a cada laço

                Parallel.For(i + 1, itemsToProcess, k =>
                    {
                        ResolveLine(A[i], A[k], i, k, _piv);
                    });
            }
        }

        public void ResolveLine(float[] linAi, float[] linAk, int i, int k, float piv)
        {
            float _m = linAk[i] / piv;
            for (int j = i; j < N; j++)
            {
                linAk[j] = linAk[j] - _m * linAi[j];
            }
            linAk[i] = _m;
        }

        public void ResolveMax(float[][] A, int i)
        {
            int Alength = A.Length;

            float[] _array = new float[A.Length];
            int[] _indices = new int[A.Length];
            Parallel.For(i, Alength, k =>    
            {
                _array[k - i] = A[i][k];
                _indices[k - i] = k - i;
            });
            Array.Sort(_array, _indices);
            MaxPosition _maxPos = new MaxPosition()
            {
                maxValue = _array[_array.Length - 1],
                position = _indices[_array.Length - 1] + i
            };

            if (_maxPos.position != i)
            {
                float[] _toChange = A[_maxPos.position];
                A[_maxPos.position] = A[i];
                A[i] = _toChange;
                _toChange = null;
            }
        }

        public MaxPosition SolveMaxPositionSync(ref float[][] array, int column)
        {
            int _position = 0;
            float _max = 0;
            for (int i = column; i < array.Length; i++)
            {
                if (Math.Abs(array[i][column]) > _max)
                {
                    _position = i;
                    _max = Math.Abs(array[i][column]);
                }
            }
            return new MaxPosition() { maxValue = _max, position = _position };
        }

        public MaxPosition SolveMaxPositionNew(float[][] array, int column)
        {
            int _arLength = array.Length - column;

            float[] _array = new float[_arLength];
            int[] _indices = new int[_arLength];
            Parallel.For(column, array.Length,  i =>    //column, array.Length, i =>
                {
                    _array[i - column] = array[column][i];
                    _indices[i - column] = i - column;
                });
            Array.Sort(_array, _indices);

            return new MaxPosition()
            {
                maxValue = _array[_array.Length-1],
                position = _indices[_array.Length-1] + column
            };

        }

        public MaxPosition SolveMaxPositionASync(float[][] array, int column)
        {
            int n = array.Length;
            int maxLengthPerTask = n / nTasks;
            int lastTaskLength = n % nTasks;
            if (lastTaskLength > 0)
            {
                nTasks += 1;
            }
            m_arrayMaxFromThreads = new float[nTasks][];

            Task[] tasks = Enumerable.Range(0, nTasks).Select(i =>
                // Create task here.
                    Task.Factory.StartNew(() =>
                    {
                        m_arrayMaxFromThreads[i] = new float[2];
                        int _initialCount = (i + 1) * maxLengthPerTask - maxLengthPerTask;
                        int _finalCount = (i + 1) * maxLengthPerTask;

                        if (i == nTasks - 1 && lastTaskLength > 0)
                        {
                            _finalCount = _finalCount - maxLengthPerTask + lastTaskLength;
                        }
                        
                        int _position = 0;
                        float _max = 0;
                        for (int j = _initialCount; j < _finalCount; j++)
                        {
                            if (Math.Abs(array[j][column]) > _max)
                            {
                                _position = j;
                                _max = Math.Abs(array[j][column]);
                            }
                        }
                        updateMaxValues(_position, _max, i);
                    }
                        
                )).ToArray();
            // Wait on all the tasks.
            Task.WaitAll(tasks);

            return VerifyMaxValue();
        }

        public void updateMaxValues(int position, float value, int thread)
        {
            lock (m_lock)
            {
                m_arrayMaxFromThreads[thread][0] = position;
                m_arrayMaxFromThreads[thread][1] = value;
            }
        }

        public MaxPosition VerifyMaxValue()
        {
            float _max = 0;
            float _position = 0;
            for (int i = 0; i < nTasks; i++)
            {
                if (m_arrayMaxFromThreads[i][1] > _max)
                {
                    _position = m_arrayMaxFromThreads[i][0];
                    _max = m_arrayMaxFromThreads[i][1];
                }
            }
            return new MaxPosition() { maxValue = _max, position = Int32.Parse(_position.ToString()) };
        }
    }
}
