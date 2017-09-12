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
    public static class ConstStringsEN
    {
        /// <summary>
        /// 检定方法字典
        /// </summary>
        public static readonly Dictionary<Methods, string> MethodsStringDict = new Dictionary<Methods, string>();

        /// <summary>
        /// 试验设计字典
        /// </summary>
        public static readonly Dictionary<Designs, string> DesignsStringDict = new Dictionary<Designs, string>();

        /// <summary>
        /// 计算模型字典
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

        public static readonly Dictionary<bool, String[]> MergeCalcReliabilityCheckConclution = new Dictionary<bool, String[]>();

        public static readonly Dictionary<bool, String[]> MergeCalcDfCheckFalseConclution = new Dictionary<bool, string[]>();

        static ConstStringsEN()
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

            ModelsStringDict.Add(Models.Null, "None");
            ModelsStringDict.Add(Models.Probit, ModelString[0]);
            ModelsStringDict.Add(Models.Logit, ModelString[1]);
            ModelsStringDict.Add(Models.Gompit, ModelString[2]);
            ModelsStringDict.Add(Models.Angle, ModelString[3]);
            ModelsStringDict.Add(Models.FourPara, ModelString[4]);

            TypesStringDict.Add(Types.Graded, TypeString[0]);
            TypesStringDict.Add(Types.Quantal, TypeString[1]);

            CheckMethodsStringDict.Add(AbnormalDataCheckMethods.Null, "None");
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
            VariationSourceCheckResult[VariationSources.Prep].Add(true, " is not significant.");
            VariationSourceCheckResult[VariationSources.Prep].Add(false, " is significant. It is suggested to repeat the test. You should refer to the revised estimate of the potency of the sample or dose adjustments and re-test.");
            VariationSourceCheckResult.Add(VariationSources.Reg, new Dictionary<bool, string>());
            VariationSourceCheckResult[VariationSources.Reg].Add(true, " is significant difference. It is statistical significance. Regression equation is valid.");
            VariationSourceCheckResult[VariationSources.Reg].Add(false, " is not significant. It is not statistical significance. Regression equation is not valid, and analysis is a failure.");
            VariationSourceCheckResult.Add(VariationSources.Par, new Dictionary<bool, string>());
            VariationSourceCheckResult[VariationSources.Par].Add(true, " is not significant. We can consider that they are a group of parallel lines.");
            VariationSourceCheckResult[VariationSources.Par].Add(false, " is significant. At least one group of lines is not parallel with the others. Individual group has outliers, please remove the group or the outliers and re-statistics.");
            VariationSourceCheckResult.Add(VariationSources.Lin, new Dictionary<bool, string>());
            VariationSourceCheckResult[VariationSources.Lin].Add(true, " is not significant. We can consider that groups of variables are linear relationships.");
            VariationSourceCheckResult[VariationSources.Lin].Add(false, " is significant. At least one group of variables is not a linear relationship. Individual group has outliers, please remove the group or the outliers and re-statistics.");
            VariationSourceCheckResult.Add(VariationSources.LinS, new Dictionary<bool, string>());
            VariationSourceCheckResult[VariationSources.LinS].Add(true, " is not significant. We can consider that these variables have a linear relationship.");
            VariationSourceCheckResult[VariationSources.LinS].Add(false, " is significant. We can consider that group of variables is not a linear relationship.");
            VariationSourceCheckResult.Add(VariationSources.LinT, new Dictionary<bool, string>());
            VariationSourceCheckResult[VariationSources.LinT].Add(true, " is not significant. We can consider that these variables have a linear relationship.");
            VariationSourceCheckResult[VariationSources.LinT].Add(false, " is significant. We can consider that the group of variables is not a linear relationship.");
            VariationSourceCheckResult.Add(VariationSources.Inters, new Dictionary<bool, string>());
            VariationSourceCheckResult[VariationSources.Inters].Add(true, " is not significant. We can consider that the standard group is the same response at zero dose with the sample group.");
            VariationSourceCheckResult[VariationSources.Inters].Add(false, " is significant. At least one sample group is the different response at zero dose.Individual group has outliers, please remove the group or the outliers and re-statistics.");
            VariationSourceCheckResult.Add(VariationSources.DaysPrep, new Dictionary<bool, string>());
            VariationSourceCheckResult[VariationSources.DaysPrep].Add(true, " is not significant. We can consider that the Days is non-interacting with the preparations.");
            VariationSourceCheckResult[VariationSources.DaysPrep].Add(false, " is significant. We can consider that the Days is interaction with the preparations. Please propose to repeat testing.");
            VariationSourceCheckResult.Add(VariationSources.DaysReg, new Dictionary<bool, string>());
            VariationSourceCheckResult[VariationSources.DaysReg].Add(true, " is not significant. Days does not affect the linear relationship of the variables. Regression equation is valid.");
            VariationSourceCheckResult[VariationSources.DaysReg].Add(false, " is significant. Days affect the linear relationship of the variables,they are interactions. Please propose to repeat testing.");
            VariationSourceCheckResult.Add(VariationSources.DaysPar, new Dictionary<bool, string>());
            VariationSourceCheckResult[VariationSources.DaysPar].Add(true, " is not significant. Days does not affect the parallel relationship of the two lines. Regression equation is valid.");
            VariationSourceCheckResult[VariationSources.DaysPar].Add(false, " is significant. Days does not affect the parallel relationship of the two lines,they are interactions. Please propose to repeat testing.");

            #endregion

            #region 可靠性检验结论

            SingleCalcReliabilityCheckConclution.Add(true, "Conclusions of assay are valid.");
            SingleCalcReliabilityCheckConclution.Add(false, "Validity of tests is not statistical significance. It is less reliable and careful to use it.");

            MergeCalcDfCheckFalseConclution.Add(false, new[] { "There are less than six for degrees of freedom of individual samples, they do not meet the conditions of the combination." ,"Combination results are not trusted, please increase the sample size."});

            MergeCalcReliabilityCheckConclution.Add(true, new[] { "Potency passes the consistency tests. It is homogeneity and not  significant difference. Combined potency and confidence limits are statistical significance.", "We recommend using the weighted results of the combination." });

            MergeCalcReliabilityCheckConclution.Add(false, new[] { "Potency does not pass the consistency tests. It is significant difference and not homogeneity. Each deviation is large between the estimated value.", "We recommend using the Semi-weighted results of the combination." });

            #endregion

            //addition
            DataTableCategoryString.Add(TableCategory.OrigDataTable, "Original Table");
            DataTableCategoryString.Add(TableCategory.TranDataTable, "Transformation Table");
            DataTableCategoryString.Add(TableCategory.CorrDataTable, "Correction Table");
            DataTableCategoryString.Add(TableCategory.LatinDataTable, "Latin Square Table");
        }

        private static readonly String[] MethodString = { "Direct determination model", "The parallel-line model", "The slope-ratio model", "The sigmoid curve model", "ED"};

        private static readonly String[] DesignString = { "Completely randomised design", "Randomised block design", "Latin square design", "Cross-over design" };

        private static readonly String[] ModelString = { "Probit", "Logit", "Gompit", "Angle", "Four-Para." };

        private static readonly String[] TypeString = { "Quantitative responses", "Quantal responses" };

        private static readonly String[] CheckerString = { "Dixon", "Grubb", "Romanovsky", "Hampel", "Quartile" };

        public static readonly String[] DataFirstLineString = { "Standard", "Test sample" };

        private static readonly String[] DataTransFormulaString =
            {
                "y", "ln(y)", "lg(y)", "y^2", "sqrt(y)", "1/y"
            };

        /// <summary>
        /// 试验组编号,组数大于8个编号改为S、T1、T2...
        /// </summary>
        public static String GetGroupIdString(int maxId, int curId)
        {
            return maxId <= GroupIdString.Length ? GroupIdString[curId] : (curId == 0 ? GroupIdString[0] : GroupIdString[1] + curId.ToString(CultureInfo.InvariantCulture));
        }

        private static readonly String[] GroupIdString = { "S", "T", "U", "V", "W", "X", "Y", "Z" };

        /// <summary>
        /// 双交叉编号
        /// </summary>
        public static readonly String[] CrossOverDesignIdString = { "S11", "T22", "S21", "T12", "T11", "S22", "T21", "S12" };

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
        public static readonly String[] PotencyEstThirdLineString = { "Lower limit", "Estimate ", "Upper limit" };

        /// <summary>
        /// 合并计算类型
        /// </summary>
        public static readonly String[] MergeCalcTypes = { "Weighted combination", "Semi-weighted combination", "Unweighted combination" };

        /// <summary>
        /// 变异来源列表
        /// </summary>
        private static readonly String[] VariationSourcesList = { "Preparations", "Regression", "Non-parallelism", "Non-linearity", "Standard", "Sample", "Intersection", "Days×Preparations", "Days×Regression", "Days×Non-parallelism" };

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
            return "Group " + groupId.ToString(CultureInfo.InvariantCulture);
        }

        //addition
        public static readonly String Undetermined = "Undetermined";

        public static readonly String TestDepartmentNumber = "Test Department Number";

        public static readonly String Potency = "Potency";

        public static readonly Dictionary<TableCategory, String> DataTableCategoryString = new Dictionary<TableCategory, string>();

        public static readonly String HistogramHeader = "Frequency Distribution Chart";

        public static readonly String HistogramXLabel = "Estimate Potency";

        public static readonly String HistogramYLabel = "Frequency";

        public static readonly String[] EDString = { "ED", "LD", "ID", "EC", "LC", "IC" };

        public static readonly String CompFigure = " Comparison Figure";
    }
}
