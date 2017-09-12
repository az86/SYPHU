using System;
using SYPHU.Assay.Tables;

namespace SYPHU.Assay.CalculationInfo
{
    /// <summary>
    /// 数据信息类：表的个数、每个表的列数、每列的行数
    /// </summary>
    [Serializable]
    public class DataSize
    {
        /// <summary>
        /// 实验组数：分表个数
        /// </summary>
        private int _preparationNum;

        /// <summary>
        /// h值
        /// </summary>
        public int PreparationNum
        {
            get { return _preparationNum; }
            set { _preparationNum = value; }
        }

        /// <summary>
        /// 每组实验计量数：分表列数
        /// </summary>
        private int _doseNum;

        /// <summary>
        /// d值
        /// </summary>
        public int DoseNum
        {
            get { return _doseNum; }
            set { _doseNum = value; }
        }

        /// <summary>
        /// 每组实验样本数：分表行数
        /// </summary>
        private int _replicateNum;

        /// <summary>
        /// n值
        /// </summary>
        public int ReplicateNum
        {
            get { return _replicateNum; }
            set { _replicateNum = value; }
        }

        public String Checker(CalcCases calcCase, Methods method, Designs design, Types type)
        {
            if (PreparationNum < 0 || DoseNum < 0 || ReplicateNum < 0)
            {
                return "数据大小不能为负值.";
            }
            if (PreparationNum == 0 || DoseNum == 0 || ReplicateNum == 0)
            {
                return "数据大小不能为0.";
            }
            if (calcCase == CalcCases.Merge)
            {
                if (PreparationNum != 1)
                {
                    return "合并计算只有1个表格.";
                }
                if (DoseNum != 4 && DoseNum != 5)
                {
                    return "合并计算支持的数据为4列或5列.";
                }
                if (ReplicateNum <= 2)
                {
                    return "用于合并计算的数据太少.";
                }
                return null;
            }
            if (type == Types.Quantal)
            {
                if (ReplicateNum != 2)
                {
                    return "定性反应的数据只支持2行.";
                }
            }
            else if (type == Types.Graded)
            {
                if (ReplicateNum == 1)
                {
                    return "每组试验不能少于2次.";
                }
            }
            
            if (method == Methods.Direct)
            {
                if (DoseNum != 1 || PreparationNum == 1)
                {
                    return "直接法应输入多个表格，每个表格1组剂量.";
                }
            }
            if (method == Methods.ParallelLine || method == Methods.SlopeRatio ||
                method == Methods.SigmoidCurve)
            {
                if (PreparationNum == 1)
                {
                    return "试验组数不能少于2组.";
                }
                if (DoseNum == 1)
                {
                    return "试验次数不能少于2次.";
                }
            }
            if (method == Methods.ED)
            {
                if (PreparationNum != 1)
                {
                    return ConstStrings.MethodsStringDict[OutLang.Chinese][method] + "只支持1组试验.";
                }
                if (DoseNum == 1)
                {
                    return "试验次数不能少于2次.";
                }
            }
            if (design == Designs.LatinSquare)
            {
                if (PreparationNum*DoseNum != ReplicateNum)
                {
                    return "拉丁方设计只支持方阵.";
                }
            }
            if (design == Designs.CrossOver)
            {
                if (PreparationNum != 4 || DoseNum != 2)
                {
                    return "双交叉设计表的个数为4，每个表的剂量数为2.";
                }
            }
            return null;
        }
    }
}
