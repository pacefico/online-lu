using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineLU.Client.Library.Resolve
{
    public class gauss : gaussBase
    {
        public gauss()
        {

        }

        public double[] SolveAxb(double[][] A, double[] b)
        {
            return this.SolveU(this.SolveUc(A, b));
        }

        private Uc SolveUc(double[][] A, double[] b)
        {
            int n = A[0].Length;
            double _piv;
            double[] _diagU = new double[n];
            double _prod = 1;
            for (int col = 0; col < n; col++)
            {
                _piv = A[col][col];

                double _m;
                for (int k = col+1; k < n; k++)
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

    }
}
