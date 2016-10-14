using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineLU.Client.ConsoleService.Resolve
{
    public class gaussBase
    {
        public class Uc
        {
            public double[][] U { get; set; }
            public double[] c { get; set; }
            public double[] diagU { get; set; }
            public double prodDiagU { get; set; }
        }

        public gaussBase(){

        }

        protected double[] SolveU(Uc uc)
        {
            int n = uc.c.Length;

            double[] x = new double[n];
            if (uc.prodDiagU == 0)
            {
                //singular
                return null;
            }
            for (int i = n - 1; i != 0; i--)
            {
                double[] ui = getElementsArray(uc.U[i], i);
                double[] xi = getElementsArray(x, i);
                double _sum = SumUx(ui, xi);

                x[i] = (uc.c[i] - _sum) / uc.diagU[i];
            }
            return x;
        }

        protected double[] getElementsArray(double[] arr, int n)
        {
            double[] _resp = new double[arr.Length - n - 1];
            n++;
            int j = 0;
            for (int i = n; i < arr.Length; i++)
            {
                _resp[j] = arr[i];
                j++;
            }
            return _resp;
        }

        protected double SumUx(double[] U, double[] x)
        {
            double _sum = 0;
            for (int i = 0; i < U.Length; i++)
            {
                _sum = _sum + (U[i] * x[i]);
            }
            return _sum;
        }

        protected double[] CalcLinha(double[] linA, double[] colA, double coef)
        {
            for (int i = 0; i < linA.Length; i++)
            {
                linA[i] = linA[i] - coef * colA[i];
            }

            return linA;
        }

    }
}
