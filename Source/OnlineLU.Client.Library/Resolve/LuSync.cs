using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineLU.Client.Library.Resolve
{
    public class LuSync
    {
        public LuSync()
        {
            
        }
        public void ResolveLU(float[][] A)
        {
            int n = A[0].Length;
            for (int i = 0; i < n-1; i++)
            {
                ResolveMax(ref A, i);
                float _piv = A[i][i];

                for (int k = i + 1; k < n; k++)
                {
                    ResolveLine(A[i], A[k], i, n, _piv);
                }
            }
        }
        public void ResolveLine(float[] linAi, float[] linAk, int i, int n, float piv)
        {
            float _m = linAk[i] / piv;

            for (int j = i; j < n; j++)
            {
                linAk[j] = linAk[j] - _m * linAi[j];
            }

            linAk[i] = _m;
        }
        public void ResolveMax(ref float[][] A, int i)
        {
            MaxPosition _maxPos = SolveMaxPosition(A, i);
            if (_maxPos.position != i)
            {
                float[] _toChange = A[_maxPos.position];
                A[_maxPos.position] = A[i];
                A[i] = _toChange;
                _toChange = null;
            }
        }

        public MaxPosition SolveMaxPosition(float[][] array, int column)
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

    }


}
