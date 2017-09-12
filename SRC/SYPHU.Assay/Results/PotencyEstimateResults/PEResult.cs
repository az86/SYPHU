using System.Collections.Generic;

namespace SYPHU.Assay.Results.PotencyEstimateResults
{
    /// <summary>
    /// 每次分析的效价结果列表
    /// </summary>
    public class PEResult
    {
        public List<TreatPEValues> PEValues = new List<TreatPEValues>();
    }
}