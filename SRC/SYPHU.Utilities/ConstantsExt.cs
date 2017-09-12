using System;
namespace SYPHU.Utilities
{
    /// <summary>
    /// 常数
    /// </summary>
    public static class ConstantsExt
    {
        public const double Pi = 3.141592653589793;
        public const double E = 2.718281828459046;
        public const double NAN = 0.0/0.0;
        public const double INF = 1.0/0.0;

        public static double Eps(int time = -16)
        {
            return Math.Pow(10, time);
        }
    }
}
