using System;

namespace SYPHU.Assay.CalculationInfo
{
    /// <summary>
    /// 统计模型：直接法、平行线法、斜率比法、S型曲线法、ED法
    /// </summary>
    [Serializable]
    public enum Methods
    {
        Direct, ParallelLine, SlopeRatio, SigmoidCurve, ED
    }
}
