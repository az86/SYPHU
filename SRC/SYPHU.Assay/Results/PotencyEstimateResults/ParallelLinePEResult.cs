using System.Collections.Generic;

namespace SYPHU.Assay.Results.PotencyEstimateResults
{
    /// <summary>
    /// 平行线检定法效价计算结果
    /// </summary>
    public class ParallelLinePEResult : PEResult
    {
        public double I;
        public double b;
        public List<double> MList = new List<double>();
        public double C;
        public double V;
    }
}
