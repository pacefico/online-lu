using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineLU.Client.Library.Resolve
{
    public class gaussPivot : gaussBase
    {
        public gaussPivot()
        {
            
        }
       
        public double[] SolveAxb(double[][] A, double[] b)
        {
            return this.SolveU(this.SolveUc(A, b));
        }

        private Uc SolveUc(double[][] A, double[] b)
        {
            int n = A[0].Length;
            double[] _diagU = new double[n];
            double _prod = 1;
            double _piv;
            double[][] L = new double[n][];
            for (int col = 0; col < n; col++)
            {
                MaxPosition _maxPos = SolveMaxPosition(A, col);

                if (_maxPos.position != col)
                {
                    double[] _toChange = A[_maxPos.position];
                    A[_maxPos.position] = A[col];
                    A[col] = _toChange;
                    _toChange = null;

                    double _cToChange = b[_maxPos.position];
                    b[_maxPos.position] = b[col];
                    b[col] = _cToChange;
                }

                _piv = A[col][col];

                double _m;
                for (int k = col + 1; k < n; k++)
                {
                    _m = A[k][col] / _piv;

                    A[k] = CalcLinha(A[k], A[col], _m);
                    b[k] = b[k] - _m * b[col];

                }
                _diagU[col] = A[col][col];
                _prod = _prod * A[col][col];
            }
            return new Uc() { U = A, c = b, diagU = _diagU, prodDiagU = _prod };
        }

        private MaxPosition SolveMaxPosition(double[][] array, int column)
        {
            int _position = 0;
            double _max = 0;
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
