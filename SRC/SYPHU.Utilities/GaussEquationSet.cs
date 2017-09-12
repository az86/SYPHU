using System;
using System.Collections.Generic;

namespace SYPHU.Utilities
{
    /// <summary>
    /// 求解高斯线性方程组
    /// </summary>
    public class GaussEquationSet
    {
        private readonly int _dim;

        private List<List<double>> _coeffMatrix;

        public List<double> Root;

        public GaussEquationSet(int dim)
        {
            _dim = dim;
        }

        public void Init(List<List<double>> coeffMatrix)
        {
            if (coeffMatrix.Count != _dim)
            {
                Console.WriteLine("Initializing failed: coefficient matrix dimension is not matching.");
                return;
            }
            for (int i = 0; i < _dim; i++)
            {
                if (coeffMatrix[i].Count != _dim + 1)
                {
                    Console.WriteLine("Initializing failed: coefficient matrix dimension is not matching.");
                    return;
                }
            }
            _coeffMatrix = coeffMatrix;
            Root = new List<double>();
            for (int i = 0; i < _dim; i++)
            {
                Root.Add(0.0);
            }
        }

        public bool Solve()
        {
            if (_coeffMatrix == null)
            {
                return false;
            }

            int i, j, k, r;
            double c;

            for (k = 0; k < _dim - 1; ++k)
            {
                r = k;
                for (i = k; i < _dim; ++i)
                {
                    if (Math.Abs(_coeffMatrix[i][k]) > Math.Abs(_coeffMatrix[r][k]))
                    {
                        r = i;
                    }
                    if (Math.Abs(_coeffMatrix[r][k]) < ConstantsExt.Eps())
                    {
                        return false;
                    }
                    if (r != k)
                    {
                        for (j = 0; j < _dim + 1; ++j)
                        {
                            c = _coeffMatrix[k][j];
                            _coeffMatrix[k][j] = _coeffMatrix[r][j];
                            _coeffMatrix[r][j] = c;
                        }
                    }
                }
                for (i = k + 1; i < _dim; ++i)
                {
                    c = _coeffMatrix[i][k] / _coeffMatrix[k][k];
                    for (j = 0; j < _dim + 1; ++j)
                    {
                        _coeffMatrix[i][j] -= c * _coeffMatrix[k][j];
                    }
                }
            }
            if (Math.Abs(_coeffMatrix[_dim - 1][_dim - 1]) < ConstantsExt.Eps())
            {
                return false;
            }
            else
            {
                for (k = _dim - 1; k >= 0; --k)
                {
                    Root[k] = _coeffMatrix[k][_dim];
                    for (j = k + 1; j < _dim; ++j)
                    {
                        Root[k] -= _coeffMatrix[k][j] * Root[j];
                    }
                    Root[k] /= _coeffMatrix[k][k];
                }
                return true;
            }
        }

        public void Display()
        {
            bool det;
            int t = 0;
            det = Solve();
            if (det == false)
            {
                Console.WriteLine("GaussEquation: failed to solve equation set.");
            }
            else
            {
                Console.Write("方程组的解为：");
                for (int i = 0; i < _dim; ++i)
                {
                    Console.Write("\n\t\tx{0:D} = {1:F4}", i, Root[i]);
                    t++;
                    if (t % 2 == 0)
                    {
                        Console.WriteLine();
                    }
                }
                if (t % 2 != 0)
                {
                    Console.WriteLine();
                }
            }
        }
    }
}
