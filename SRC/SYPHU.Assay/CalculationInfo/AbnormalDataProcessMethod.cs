using System;

namespace SYPHU.Assay.CalculationInfo
{
    /// <summary>
    /// 异常值处理方法：均值法、解线性方程组法、双交叉法
    /// </summary>
    [Serializable]
    public enum AbnormalDataProcessMethod
    {
        Null, Average, EquationSet, CrossOverAdd
    }
}
