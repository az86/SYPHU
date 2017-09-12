using System;

namespace SYPHU.Assay.CalculationInfo
{
    /// <summary>
    /// 异常值检测方法
    /// </summary>
    [Serializable]
    public enum AbnormalDataCheckMethods
    {
        Null, Dixon, Grubb, Romanovsky, Hampel, Quartile
    }
}
