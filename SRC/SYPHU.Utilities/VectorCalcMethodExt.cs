using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SYPHU.Utilities
{
    /// <summary>
    /// 向量方法扩展
    /// </summary>
    public static class VectorCalcMethodExt
    {
        /// <summary>
        /// 最大值位置
        /// </summary>
        /// <param name="ld"></param>
        /// <returns></returns>
        public static int MaxValPos(this List<double> ld)
        {
            return ld.IndexOf(ld.Max());
        }

        /// <summary>
        /// 最小值位置
        /// </summary>
        /// <param name="ld"></param>
        /// <returns></returns>
        public static int MinValPos(this List<double> ld)
        {
            return ld.IndexOf(ld.Min());
        }

        /// <summary>
        /// 两个向量点积
        /// </summary>
        /// <param name="ld1"></param>
        /// <param name="ld2"></param>
        /// <returns></returns>
        public static double DotProd(List<double> ld1, List<double> ld2)
        {
            double s = 0;
            for (int i = 0; i < ld1.Count(); i++)
            {
                s += ld1[i] * ld2[i];
            }
            return s;
        }

        /// <summary>
        /// 两个向量点积
        /// </summary>
        /// <param name="ld1"></param>
        /// <param name="ld2"></param>
        /// <returns></returns>
        public static double DotProd(ObservableCollection<double> ld1, ObservableCollection<double> ld2)
        {
            double s = 0;
            for (int i = 0; i < ld1.Count(); i++)
            {
                s += ld1[i] * ld2[i];
            }
            return s;
        }

        /// <summary>
        /// 向量与常数点积
        /// </summary>
        /// <param name="ld"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        public static double DotProd(List<double> ld, double d)
        {
            double s = 0;
            for (int i = 0; i < ld.Count(); i++)
            {
                s += ld[i] * d;
            }
            return s;
        }

        /// <summary>
        /// 向量与常数点积
        /// </summary>
        /// <param name="ld"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        public static double DotProd(ObservableCollection<double> ld, double d)
        {
            double s = 0;
            for (int i = 0; i < ld.Count(); i++)
            {
                s += ld[i] * d;
            }
            return s;
        }

        public static double DotProd(double d, List<double> ld)
        {
            return DotProd(ld, d);
        }

        public static double DotProd(double d, ObservableCollection<double> ld)
        {
            return DotProd(ld, d);
        }

        /// <summary>
        /// 向量各个元素的平方和
        /// </summary>
        /// <param name="ld"></param>
        /// <returns></returns>
        public static double SquareSum(this List<double> ld)
        {
            return DotProd(ld, ld);
        }

        /// <summary>
        /// 向量各个元素的平方和
        /// </summary>
        /// <param name="ld"></param>
        /// <returns></returns>
        public static double SquareSum(this ObservableCollection<double> ld)
        {
            return DotProd(ld, ld);
        }

        /// <summary>
        /// 向量二范数
        /// </summary>
        /// <param name="ld"></param>
        /// <returns></returns>
        public static double Norm(this List<double> ld)
        {
            return Math.Sqrt(ld.SquareSum());
        }

        /// <summary>
        /// 向量二范数
        /// </summary>
        /// <param name="ld"></param>
        /// <returns></returns>
        public static double Norm(this ObservableCollection<double> ld)
        {
            return Math.Sqrt(ld.SquareSum());
        }

        /// <summary>
        /// 误差
        /// </summary>
        /// <param name="ld"></param>
        /// <returns></returns>
        public static double Error(this List<double> ld)
        {
            return ld.SquareSum() - ld.Sum() * ld.Average();
        }

        /// <summary>
        /// 误差
        /// </summary>
        /// <param name="ld"></param>
        /// <returns></returns>
        public static double Error(this ObservableCollection<double> ld)
        {
            return ld.SquareSum() - ld.Sum() * ld.Average();
        }

        /// <summary>
        /// 自由度
        /// </summary>
        /// <param name="ld"></param>
        /// <returns></returns>
        public static int Freedom(this List<double> ld)
        {
            return ld.Count() - 1;
        }

        /// <summary>
        /// 自由度
        /// </summary>
        /// <param name="ld"></param>
        /// <returns></returns>
        public static int Freedom(this ObservableCollection<double> ld)
        {
            return ld.Count() - 1;
        }

        /// <summary>
        /// 10指数
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static double Antilg(double d)
        {
            return Math.Pow(10.0, d);
        }

        /// <summary>
        /// e指数
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static double Antiln(double d)
        {
            return Math.Pow(ConstantsExt.E, d);
        }

        /// <summary>
        /// 向量部分和
        /// </summary>
        /// <param name="ld"></param>
        /// <param name="begin">起始</param>
        /// <param name="end">终止</param>
        /// <returns></returns>
        public static double PartialSum(this List<double> ld, int begin, int end)
        {
            double s = 0;
            for (int i = begin; i <= end; i++)
            {
                s += ld[i];
            }
            return s;
        }

        /// <summary>
        /// 标准差
        /// </summary>
        /// <param name="ld"></param>
        /// <returns></returns>
        public static double StandardDevivation(this List<double> ld)
        {
            if (ld == null)
            {
                throw new NullReferenceException();
            }
            if (ld.Count == 0)
            {
                throw new Exception("StandardDevivation: List length = 0");
            }
            if (ld.Count == 1)
            {
                return 0;
            }
            double ave = ld.Average();
            return Math.Sqrt(ld.Sum(t => (t - ave) * (t - ave)) / (ld.Count - 1));
        }

        /// <summary>
        /// 中值
        /// </summary>
        /// <param name="ld"></param>
        /// <returns></returns>
        public static double Median(this List<double> ld)
        {
            var tmp = new List<double>();
            tmp.AddRange(ld);
            tmp.Sort();
            return tmp.Count%2 == 0 ? tmp[tmp.Count/2] : (tmp[(tmp.Count - 1)/2] + tmp[(tmp.Count + 1)/2])*0.5;
        }

        /// <summary>
        /// 计算三维数组的最值，isMax=true，计算最大值，isMax=false，计算最小值
        /// </summary>
        /// <param name="llld"></param>
        /// <param name="isMax"></param>
        /// <returns></returns>
        public static double List3DExtremum(List<List<ObservableCollection<double>>> llld, bool isMax = true)
        {
            var tmp = new List<double>();
            foreach (var t1 in llld.SelectMany(t => t))
            {
                tmp.AddRange(t1);
            }
            return isMax ? tmp.Max() : tmp.Min();
        }

        /// <summary>
        /// 计算一个向量的最大区间
        /// </summary>
        /// <param name="ld"></param>
        /// <returns></returns>
        public static double MathRange(this List<double> ld)
        {
            return ld.Max() - ld.Min();
        }

        /// <summary>
        /// 计算字符串数字的精度
        /// </summary>
        /// <param name="val"></param>
        /// <returns>10^(-精度)</returns>
        public static double Precision(String val)
        {
            int loc = val.LastIndexOf('.');
            return Antilg(loc - val.Length + 1);
        }

        public static int MaxSubLength(List<List<String>> oListList)
        {
            if (oListList == null || oListList.Count == 0)
            {
                return 0;
            }
            var subCountList = oListList.Select(subList => subList.Count).ToList();
            return subCountList.Max();
        }
    }
}
