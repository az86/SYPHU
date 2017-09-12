using System;
using System.Collections.Generic;

namespace SYPHU.Utilities
{
    /// <summary>
    /// 最小二乘法
    /// </summary>
    public class LSM
    {
        public List<double> Root;

        private readonly int _pointNum;

        private readonly int _times;

        private List<double> _dataX, _dataY;

        private List<List<double>> _matrixA, _coeffA, _coeffFormula;

        private List<double> _tmpY;

        public LSM(int pointNum, int times)
        {
            _pointNum = pointNum;
            _times = times;
        }

        public void Init(List<double> dataX, List<double> dataY)
        {
            _dataX = new List<double>(_pointNum);
            _dataY = new List<double>(_pointNum);
            _tmpY = new List<double>(_pointNum);
            _matrixA = new List<List<double>>(_pointNum);
            for (int i = 0; i < _pointNum; ++i)
            {
                _dataX.Add(dataX[i]);
                _dataY.Add(dataY[i]);
                _tmpY.Add(0);
                _matrixA.Add(new List<double>(_times + 1));
                for (int j = 0; j < _times + 1; ++j)
                {
                    _matrixA[i].Add(0);
                }
            }

            Root = new List<double>(_times + 1);
            _coeffA = new List<List<double>>(_times + 1);
            _coeffFormula = new List<List<double>>(_times + 1);
            for (int i = 0; i < _times + 1; ++i)
            {
                Root.Add(0);
                _coeffA.Add(new List<double>(_times + 1));
                _coeffFormula.Add(new List<double>(_times + 1));
                for (int j = 0; j < _times + 1; ++j)
                {
                    _coeffA[i].Add(0);
                }
                for (int j = 0; j < _times + 2; ++j)
                {
                    _coeffFormula[i].Add(0);
                }
            }
        }

        public void DoLSM()
        {
            GetA();
            Multiple();
            GetY();
            ConsFormula();
            ConvertCoeff();
            GetRoot();
            Display();
        }

        private void GetA()
        {
            for (int i = 0; i < _pointNum; ++i)
            {
                for (int j = 0; j < _times + 1; ++j)
                {
                    _matrixA[i][j] = Math.Pow(_dataX[i], Convert.ToDouble(j));
                }
            }
        }

        private void Multiple()
        {
            for (int i = 0; i < _times + 1; ++i)
            {
                for (int j = 0; j < _times + 1; ++j)
                {
                    double res = 0;
                    for (int k = 0; k < _pointNum; ++k)
                    {
                        res += _matrixA[k][i] * _matrixA[k][j];
                        _coeffA[i][j] = res;
                    }
                }
            }
        }

        private void GetY()
        {
            for (int i = 0; i < _times + 1; ++i)
            {
                double s = 0;
                for (int j = 0; j < _pointNum; ++j)
                {
                    s += _matrixA[j][i] * _dataY[j];
                }
                _tmpY[i] = s;
            }
        }

        private void ConsFormula()
        {
            for (int i = 0; i < _times + 1; ++i)
            {
                for (int j = 0; j < _times + 2; ++j)
                {
                    if (j == _times + 1)
                    {
                        _coeffFormula[i][j] = _tmpY[i];
                    }
                    else
                    {
                        _coeffFormula[i][j] = _coeffA[i][j];
                    }
                }
            }
        }

        private void ConvertCoeff()
        {
            for (int i = 1; i < _times + 1; ++i)
            {
                for (int j = i; j < _times + 1; ++j)
                {
                    if (Math.Abs(_coeffFormula[i - 1][i - 1]) < ConstantsExt.Eps())
                    {
                        int p;
                        for (p = i; p < _times + 1; ++p)
                        {
                            if (Math.Abs(_coeffFormula[p][i - 1]) > ConstantsExt.Eps())
                            {
                                break;
                            }
                        }
                        if (p == _times + 1)
                        {
                            Console.WriteLine("方程组无解.");
                            return;
                        }
                        for (int t = 0; t < _times + 2; ++t)
                        {
                            double tmp = _coeffFormula[i - 1][t];
                            _coeffFormula[i - 1][t] = _coeffFormula[p][t];
                            _coeffFormula[p][t] = tmp;
                        }
                    }
                    double rate = _coeffFormula[j][i - 1] / _coeffFormula[i - 1][i - 1];
                    for (int k = i - 1; k < _times + 2; ++k)
                    {
                        _coeffFormula[j][k] -= _coeffFormula[i - 1][k] * rate;
                        if (Math.Abs(_coeffFormula[j][k]) <= ConstantsExt.Eps())
                        {
                            _coeffFormula[j][k] = 0;
                        }
                    }
                }
            }
        }

        private void GetRoot()
        {
            for (int i = _times; i >= 0; --i)
            {
                double tmp = _coeffFormula[i][_times + 1];
                for (int j = _times; j > i; --j)
                {
                    tmp -= _coeffFormula[i][j] * Root[j];
                }
                Root[i] = tmp / _coeffFormula[i][i];
            }
        }

        private void Display()
        {
            Console.Write("拟合的多项式为：\n\t\ty = {0:F4} ", Root[0]);
            for (int i = 1; i < _times + 1; ++i)
            {
                if (Math.Abs(Root[i] - 0) > ConstantsExt.Eps())
                {
                    Console.Write("+ {0:F4} ", Root[i]);
                    for (int j = 0; j < i; ++j)
                    {
                        Console.Write("* x ");
                    }
                }
            }
            Console.WriteLine();
        }
    }
}
