using System;

namespace SYPHU.Assay.CalculationInfo
{
    /// <summary>
    /// 试验设计类型：完全随机、随机区组、拉丁方、双交叉
    /// </summary>
    [Serializable]
    public enum Designs
    {
        CompletelyRandomised, RandomisedBlock, LatinSquare, CrossOver
    }
}
