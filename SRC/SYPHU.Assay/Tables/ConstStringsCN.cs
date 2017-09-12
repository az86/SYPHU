using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Media;
using SYPHU.Assay.CalculationInfo;
using SYPHU.Assay.Results.ReliabilityCheck;

namespace SYPHU.Assay.Tables
{
    /// <summary>
    /// 常字符串
    /// </summary>
    public static class ConstStringsCN
    {
        /// <summary>
        /// 统计模型字典
        /// </summary>
        public static readonly Dictionary<Methods, string> MethodsStringDict = new Dictionary<Methods, string>();

        /// <summary>
        /// 试验设计字典
        /// </summary>
        public static readonly Dictionary<Designs, string> DesignsStringDict = new Dictionary<Designs, string>();

        /// <summary>
        /// 计算方法字典
        /// </summary>
        public static readonly Dictionary<Models, string> ModelsStringDict = new Dictionary<Models, string>();

        /// <summary>
        /// 反应类型字典
        /// </summary>
        public static readonly Dictionary<Types, string> TypesStringDict = new Dictionary<Types, string>();

        /// <summary>
        /// 异常值检测方法字典
        /// </summary>
        public static readonly Dictionary<AbnormalDataCheckMethods, string> CheckMethodsStringDict = new Dictionary<AbnormalDataCheckMethods, string>();

        /// <summary>
        /// 转换公式字典
        /// </summary>
        public static readonly Dictionary<DataTransformationFormula, string> FormulasStringDict = new Dictionary<DataTransformationFormula, string>();

        /// <summary>
        /// 变异来源字典
        /// </summary>
        public static readonly Dictionary<VariationSources, string> VariationSourcesDict = new Dictionary<VariationSources, string>();

        /// <summary>
        /// 变异来源检测结果字典
        /// </summary>
        public static readonly Dictionary<VariationSources, Dictionary<bool, string>> VariationSourceCheckResult = new Dictionary<VariationSources, Dictionary<bool, string>>();

        /// <summary>
        /// 可靠性检测结果
        /// </summary>
        public static readonly Dictionary<bool, string> SingleCalcReliabilityCheckConclution = new Dictionary<bool, string>();

        public static readonly Dictionary<bool, String[]>  MergeCalcReliabilityCheckConclution = new Dictionary<bool, String[]>();

        public static readonly Dictionary<bool, String[]> MergeCalcDfCheckFalseConclution = new Dictionary<bool, string[]>();

        static ConstStringsCN()
        {
            #region 计算信息

            MethodsStringDict.Add(Methods.Direct, MethodString[0]);
            MethodsStringDict.Add(Methods.ParallelLine, MethodString[1]);
            MethodsStringDict.Add(Methods.SlopeRatio, MethodString[2]);
            MethodsStringDict.Add(Methods.SigmoidCurve, MethodString[3]);
            MethodsStringDict.Add(Methods.ED, MethodString[4]);

            DesignsStringDict.Add(Designs.CompletelyRandomised, DesignString[0]);
            DesignsStringDict.Add(Designs.RandomisedBlock, DesignString[1]);
            DesignsStringDict.Add(Designs.LatinSquare, DesignString[2]);
            DesignsStringDict.Add(Designs.CrossOver, DesignString[3]);

            ModelsStringDict.Add(Models.Null, "无");
            ModelsStringDict.Add(Models.Probit, ModelString[0]);
            ModelsStringDict.Add(Models.Logit, ModelString[1]);
            ModelsStringDict.Add(Models.Gompit, ModelString[2]);
            ModelsStringDict.Add(Models.Angle, ModelString[3]);
            ModelsStringDict.Add(Models.FourPara, ModelString[4]);

            TypesStringDict.Add(Types.Graded, TypeString[0]);
            TypesStringDict.Add(Types.Quantal, TypeString[1]);

            CheckMethodsStringDict.Add(AbnormalDataCheckMethods.Null, "无");
            CheckMethodsStringDict.Add(AbnormalDataCheckMethods.Dixon, CheckerString[0]);
            CheckMethodsStringDict.Add(AbnormalDataCheckMethods.Grubb, CheckerString[1]);
            CheckMethodsStringDict.Add(AbnormalDataCheckMethods.Romanovsky, CheckerString[2]);
            CheckMethodsStringDict.Add(AbnormalDataCheckMethods.Hampel, CheckerString[3]);
            CheckMethodsStringDict.Add(AbnormalDataCheckMethods.Quartile, CheckerString[4]);

            FormulasStringDict.Add(DataTransformationFormula.Null, DataTransFormulaString[0]);
            FormulasStringDict.Add(DataTransformationFormula.LogE, DataTransFormulaString[1]);
            FormulasStringDict.Add(DataTransformationFormula.Log10, DataTransFormulaString[2]);
            FormulasStringDict.Add(DataTransformationFormula.Square, DataTransFormulaString[3]);
            FormulasStringDict.Add(DataTransformationFormula.SquareRoot, DataTransFormulaString[4]);
            FormulasStringDict.Add(DataTransformationFormula.Reciprocal, DataTransFormulaString[5]);

            #endregion

            #region 方差分析

            VariationSourcesDict.Add(VariationSources.Prep, VariationSourcesList[0]);
            VariationSourcesDict.Add(VariationSources.Reg, VariationSourcesList[1]);
            VariationSourcesDict.Add(VariationSources.Par, VariationSourcesList[2]);
            VariationSourcesDict.Add(VariationSources.Lin, VariationSourcesList[3]);
            VariationSourcesDict.Add(VariationSources.LinS, VariationSourcesList[4]);
            VariationSourcesDict.Add(VariationSources.LinT, VariationSourcesList[5]);
            VariationSourcesDict.Add(VariationSources.Inters, VariationSourcesList[6]);
            VariationSourcesDict.Add(VariationSources.DaysPrep, VariationSourcesList[7]);
            VariationSourcesDict.Add(VariationSources.DaysReg, VariationSourcesList[8]);
            VariationSourcesDict.Add(VariationSources.DaysPar, VariationSourcesList[9]);

            VariationSourceCheckResult.Add(VariationSources.Prep, new Dictionary<bool, string>());
            VariationSourceCheckResult[VariationSources.Prep].Add(true, "无显著差异。");
            VariationSourceCheckResult[VariationSources.Prep].Add(false, "有显著性差异，建议进行重复试验，应参考所得结果重新估计样品的效价或重新调整剂量试验。");
            VariationSourceCheckResult.Add(VariationSources.Reg, new Dictionary<bool, string>());
            VariationSourceCheckResult[VariationSources.Reg].Add(true, "有非常显著性差异，有统计学意义，回归方程有效。");
            VariationSourceCheckResult[VariationSources.Reg].Add(false, "无差异、无统计学意义，回归方程无效，分析失败。");
            VariationSourceCheckResult.Add(VariationSources.Par, new Dictionary<bool, string>());
            VariationSourceCheckResult[VariationSources.Par].Add(true, "无显著性差异，可认为多组直线平行。");
            VariationSourceCheckResult[VariationSources.Par].Add(false, "有显著性差异，至少有一组直线与其他不平行。个别组有离群点，请剔除该组或剔除异常值后重新统计。");
            VariationSourceCheckResult.Add(VariationSources.Lin, new Dictionary<bool, string>());
            VariationSourceCheckResult[VariationSources.Lin].Add(true, "无显著差异，可认为多组变量间呈直线关系。");
            VariationSourceCheckResult[VariationSources.Lin].Add(false,
                                                                 "有显著性差异，至少有一组变量间不存在直线关系,建议检查个别组是否有离群点，请剔除异常值后重新统计。");
            VariationSourceCheckResult.Add(VariationSources.LinS, new Dictionary<bool, string>());
            VariationSourceCheckResult[VariationSources.LinS].Add(true, "无显著差异，可认为变量间呈直线关系。");
            VariationSourceCheckResult[VariationSources.LinS].Add(false, "有显著性差异，该组变量间不存在直线关系。");
            VariationSourceCheckResult.Add(VariationSources.LinT, new Dictionary<bool, string>());
            VariationSourceCheckResult[VariationSources.LinT].Add(true, "无显著差异，可认为变量间呈直线关系。");
            VariationSourceCheckResult[VariationSources.LinT].Add(false, "有显著性差异，该组变量间不存在直线关系。");
            VariationSourceCheckResult.Add(VariationSources.Inters, new Dictionary<bool, string>());
            VariationSourceCheckResult[VariationSources.Inters].Add(true, "无显著性差异，可认为标准组与样品组在零剂量处效应相同。");
            VariationSourceCheckResult[VariationSources.Inters].Add(false,
                                                                    "有显著性差异，至少有一个样品组在零剂量处效应不同，个别组有离群点，请剔除该组或剔除异常值后重新统计。");
            VariationSourceCheckResult.Add(VariationSources.DaysPrep, new Dictionary<bool, string>());
            VariationSourceCheckResult[VariationSources.DaysPrep].Add(true, "无显著差异，可认为次间与试品间无交互作用。");
            VariationSourceCheckResult[VariationSources.DaysPrep].Add(false, "有显著性差异，认为次间与试品间有交互作用，建议重复试验。");
            VariationSourceCheckResult.Add(VariationSources.DaysReg, new Dictionary<bool, string>());
            VariationSourceCheckResult[VariationSources.DaysReg].Add(true, "无显著差异，可认为次间不影响变量间的直线关系，回归有效。");
            VariationSourceCheckResult[VariationSources.DaysReg].Add(false, "有显著性差异，认为次间与变量间的直线关系有交互作用，建议重复实验。");
            VariationSourceCheckResult.Add(VariationSources.DaysPar, new Dictionary<bool, string>());
            VariationSourceCheckResult[VariationSources.DaysPar].Add(true, "无显著差异，可认为次间不影响两组变量间直线的平行关系，回归有效。");
            VariationSourceCheckResult[VariationSources.DaysPar].Add(false, "有显著性差异，认为次间与两组变量间的直线平行关系有交互作用，建议重复实验。");

            #endregion

            #region 可靠性检验结论

            SingleCalcReliabilityCheckConclution.Add(true, "试验结论有效，可靠。");
            SingleCalcReliabilityCheckConclution.Add(false, "试验可靠性检验统计学意义不显著，结论可信度较低，使用需谨慎。");

            MergeCalcDfCheckFalseConclution.Add(false, new[] {"有个别样品的自由度小于6，不符合合并计算条件。", "合并计算结果不可信，请增大样本量。"});

            MergeCalcReliabilityCheckConclution.Add(true, new[] { "效价通过一致性检验，没有显著性差异，具有同质性。合并效价以及限度具有统计学意义。", "建议使用加权合并计算结果，可信度较高。" });

            MergeCalcReliabilityCheckConclution.Add(false, new[] { "效价未通过一致性检验，有显著性差异，不具有同质性。每个估算值之间的偏差较大。", "建议使用校正加权合并计算结果，可信度较高。" });

            #endregion

            //addition
            DataTableCategoryString.Add(TableCategory.OrigDataTable, "原始数据表");
            DataTableCategoryString.Add(TableCategory.TranDataTable, "转换后数据表");
            DataTableCategoryString.Add(TableCategory.CorrDataTable, "校正后数据表");
            DataTableCategoryString.Add(TableCategory.LatinDataTable, "拉丁方数据表");
        }

        private static readonly String[] MethodString = { "直接检定法", "平行线检定法", "斜率比检定法", "S型曲检定法", "半数反应量"};

        private static readonly String[] DesignString = { "完全随机", "随机区组", "拉丁方", "双交叉" };

        private static readonly String[] ModelString = { "Probit", "Logit", "Gompit", "Angle", "Four Paras" };

        private static readonly String[] TypeString = { "定量反应", "定性反应" };

        private static readonly String[] CheckerString = { "Dixon", "Grubb", "Romanovsky", "Hampel", "Quartile" };
        
        public static readonly String[] DataFirstLineString = { "标准组", "样品组" };

        private static readonly String[] DataTransFormulaString =
            {
                "y", "ln(y)", "lg(y)", "y^2", "sqrt(y)", "1/y"
            };

        /// <summary>
        /// 试验组编号，组数大于8个编号改为S、T1、T2...
        /// </summary>
        public static String GetGroupIdString(int maxId, int curId)
        {
            return maxId <= GroupIdString.Length ? GroupIdString[curId] : (curId == 0 ? GroupIdString[0] : GroupIdString[1] + curId.ToString(CultureInfo.InvariantCulture));
        }

        private static readonly String[] GroupIdString = { "S", "T", "U", "V", "W", "X", "Y", "Z" };

        /// <summary>
        /// 双交叉编号
        /// </summary>
        public static readonly String[] CrossOverDesignIdString = {"S11", "T22", "S21", "T12", "T11", "S22", "T21", "S12"};

        /// <summary>
        /// 画图颜色
        /// </summary>
        public static readonly Color[] ColorList =
            {
                Colors.Blue,  Colors.Red, Colors.Green, Colors.DarkOrchid, 
                Colors.CadetBlue, Colors.DeepSkyBlue, Colors.Orange, Colors.Brown
            };

        /// <summary>
        /// 效价表第三行内容
        /// </summary>
        public static readonly String[] PotencyEstThirdLineString = { "下限", "估计值", "上限" };

        /// <summary>
        /// 合并计算类型
        /// </summary>
        public static readonly String[] MergeCalcTypes = { "加权合并", "校正加权合并", "不加权合并" };

        /// <summary>
        /// 变异来源列表
        /// </summary>
        private static readonly String[] VariationSourcesList = { "试品间", "回归", "非平行", "非线性", "标准组", "样品组", "相交性", "次间×试品间", "次间×回归", "次间×非平行" };

        /// <summary>
        /// 获取x轴label
        /// </summary>
        /// <param name="formula">转换公式</param>
        /// <param name="useDilutionsTimes">是否使用稀释倍数</param>
        /// <returns></returns>
        public static String GetXLabel(DataTransformationFormula formula, bool useDilutionsTimes = false)
        {
            String xLabel = useDilutionsTimes ? "dose*A*dil" : "dose";
            switch (formula)
            {
                case DataTransformationFormula.LogE:
                    xLabel = "ln(" + xLabel + ")";
                    break;
                case DataTransformationFormula.Log10:
                    xLabel = "lg(" + xLabel + ")";
                    break;
            }
            return xLabel + " (x)";
        }

        /// <summary>
        /// 获取y轴label
        /// </summary>
        /// <param name="type">反应类型</param>
        /// <returns></returns>
        public static String GetYLabel(Types type)
        {
            switch (type)
            {
                case Types.Quantal:
                    return "p = r/n";
                case Types.Graded:
                    return "y'";
            }
            return null;
        }

        public static String GetGroupString(int groupId)
        {
            return "第" + groupId.ToString(CultureInfo.InvariantCulture) + "组";
        }

        //addition
        public static readonly String Undetermined = "待确定";

        public static readonly String TestDepartmentNumber = "试验单位编号";

        public static readonly String Potency = "效价";

        public static readonly Dictionary<TableCategory, String> DataTableCategoryString = new Dictionary<TableCategory, string>();

        public static readonly String HistogramHeader = "频数分布图";

        public static readonly String HistogramXLabel = "估计效价";

        public static readonly String HistogramYLabel = "频数";

        public static readonly String CompFigure = "对比图";
    }
}
