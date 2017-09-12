using System;

namespace SYPHU.Assay.Results.VarianceAnalysisResults
{
    /// <summary>
    /// 每个方差分析项需要计算的值
    /// </summary>
    public class BasicVarianceAnalysisValues
    {
        public int FreedomDegree = 1;
        public double SquareSum;
        public double MeanSquare
        {
            get { return FreedomDegree == 0 ? 0 : SquareSum / Convert.ToDouble(FreedomDegree); }
        }

        public double FValue;
        public double PValue;
    }
}
