using System;

namespace SYPHU.Assay.CalculationInfo
{
    /// <summary>
    /// 反应类型
    /// </summary>
    [Serializable]
    public enum Types
    {
        /// <summary>
        /// 定量反应
        /// </summary>
        Graded, 

        /// <summary>
        /// 定性反应
        /// </summary>
        Quantal 
    }
}
