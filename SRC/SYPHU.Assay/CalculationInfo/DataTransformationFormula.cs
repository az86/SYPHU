using System;

namespace SYPHU.Assay.CalculationInfo
{
    /// <summary>
    /// 数据转换公式：内嵌、用户自定义
    /// </summary>
    [Serializable]
    public enum DataTransformationFormula
    {
        Null, LogE, Log10, Square, SquareRoot, Reciprocal, UserDefined
    }
}
