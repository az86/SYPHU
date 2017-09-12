using System.Collections.Generic;

namespace SYPHU.Assay.Results.PotencyEstimateResults
{
    /// <summary>
    /// 斜率比法效价计算结果
    /// </summary>
    public class SlopeRatioPEResult : PEResult
    {
        public double V1, V2;

        public List<double> bList = new List<double>();

        public List<double> RList = new List<double>();

        public double C;

        public double K;
    }
}
