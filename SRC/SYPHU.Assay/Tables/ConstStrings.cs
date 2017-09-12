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
    public static class ConstStrings
    {
        static ConstStrings()
        {
            MethodsStringDict.Add(OutLang.Chinese, ConstStringsCN.MethodsStringDict);
            MethodsStringDict.Add(OutLang.English, ConstStringsEN.MethodsStringDict);

            DesignsStringDict.Add(OutLang.Chinese, ConstStringsCN.DesignsStringDict);
            DesignsStringDict.Add(OutLang.English, ConstStringsEN.DesignsStringDict);

            ModelsStringDict.Add(OutLang.Chinese, ConstStringsCN.ModelsStringDict);
            ModelsStringDict.Add(OutLang.English, ConstStringsEN.ModelsStringDict);

            TypesStringDict.Add(OutLang.Chinese, ConstStringsCN.TypesStringDict);
            TypesStringDict.Add(OutLang.English, ConstStringsEN.TypesStringDict);

            CheckMethodsStringDict.Add(OutLang.Chinese, ConstStringsCN.CheckMethodsStringDict);
            CheckMethodsStringDict.Add(OutLang.English, ConstStringsEN.CheckMethodsStringDict);

            VariationSourcesDict.Add(OutLang.Chinese, ConstStringsCN.VariationSourcesDict);
            VariationSourcesDict.Add(OutLang.English, ConstStringsEN.VariationSourcesDict);

            VariationSourceCheckResult.Add(OutLang.Chinese, ConstStringsCN.VariationSourceCheckResult);
            VariationSourceCheckResult.Add(OutLang.English, ConstStringsEN.VariationSourceCheckResult);

            SingleCalcReliabilityCheckConclution.Add(OutLang.Chinese, ConstStringsCN.SingleCalcReliabilityCheckConclution);
            SingleCalcReliabilityCheckConclution.Add(OutLang.English, ConstStringsEN.SingleCalcReliabilityCheckConclution);

            MergeCalcReliabilityCheckConclution.Add(OutLang.Chinese, ConstStringsCN.MergeCalcReliabilityCheckConclution);
            MergeCalcReliabilityCheckConclution.Add(OutLang.English, ConstStringsEN.MergeCalcReliabilityCheckConclution);

            MergeCalcDfCheckFalseConclution.Add(OutLang.Chinese, ConstStringsCN.MergeCalcDfCheckFalseConclution);
            MergeCalcDfCheckFalseConclution.Add(OutLang.English, ConstStringsEN.MergeCalcDfCheckFalseConclution);

            DataFirstLineString.Add(OutLang.Chinese, ConstStringsCN.DataFirstLineString);
            DataFirstLineString.Add(OutLang.English, ConstStringsEN.DataFirstLineString);

            PotencyEstThirdLineString.Add(OutLang.Chinese, ConstStringsCN.PotencyEstThirdLineString);
            PotencyEstThirdLineString.Add(OutLang.English, ConstStringsEN.PotencyEstThirdLineString);

            MergeCalcTypes.Add(OutLang.Chinese, ConstStringsCN.MergeCalcTypes);
            MergeCalcTypes.Add(OutLang.English, ConstStringsEN.MergeCalcTypes);

            Undetermined.Add(OutLang.Chinese, ConstStringsCN.Undetermined);
            Undetermined.Add(OutLang.English, ConstStringsEN.Undetermined);

            TestDepartmentNumber.Add(OutLang.Chinese, ConstStringsCN.TestDepartmentNumber);
            TestDepartmentNumber.Add(OutLang.English, ConstStringsEN.TestDepartmentNumber);

            Potency.Add(OutLang.Chinese, ConstStringsCN.Potency);
            Potency.Add(OutLang.English, ConstStringsEN.Potency);

            DataTableCategoryString.Add(OutLang.Chinese, ConstStringsCN.DataTableCategoryString);
            DataTableCategoryString.Add(OutLang.English, ConstStringsEN.DataTableCategoryString);

            FormulasStringDict.Add(DataTransformationFormula.Null, DataTransFormulaString[0]);
            FormulasStringDict.Add(DataTransformationFormula.LogE, DataTransFormulaString[1]);
            FormulasStringDict.Add(DataTransformationFormula.Log10, DataTransFormulaString[2]);
            FormulasStringDict.Add(DataTransformationFormula.Square, DataTransFormulaString[3]);
            FormulasStringDict.Add(DataTransformationFormula.SquareRoot, DataTransFormulaString[4]);
            FormulasStringDict.Add(DataTransformationFormula.Reciprocal, DataTransFormulaString[5]);

            HistogramHeader.Add(OutLang.Chinese, ConstStringsCN.HistogramHeader);
            HistogramHeader.Add(OutLang.English, ConstStringsEN.HistogramHeader);

            HistogramXLabel.Add(OutLang.Chinese, ConstStringsCN.HistogramXLabel);
            HistogramXLabel.Add(OutLang.English, ConstStringsEN.HistogramXLabel);

            HistogramYLabel.Add(OutLang.Chinese, ConstStringsCN.HistogramYLabel);
            HistogramYLabel.Add(OutLang.English, ConstStringsEN.HistogramYLabel);

            EDString.Add(EDEnum.ED, ConstStringsEN.EDString[0]);
            EDString.Add(EDEnum.LD, ConstStringsEN.EDString[1]);
            EDString.Add(EDEnum.ID, ConstStringsEN.EDString[2]);
            EDString.Add(EDEnum.EC, ConstStringsEN.EDString[3]);
            EDString.Add(EDEnum.LC, ConstStringsEN.EDString[4]);
            EDString.Add(EDEnum.IC, ConstStringsEN.EDString[5]);

            CompFigure.Add(OutLang.Chinese, ConstStringsCN.CompFigure);
            CompFigure.Add(OutLang.English, ConstStringsEN.CompFigure);
        }

        /// <summary>
        /// 统计模型字典
        /// </summary>
        public static readonly Dictionary<OutLang, Dictionary<Methods, string>> MethodsStringDict = new Dictionary<OutLang, Dictionary<Methods, string>>();

        /// <summary>
        /// 试验设计字典
        /// </summary>
        public static readonly Dictionary<OutLang, Dictionary<Designs, string>> DesignsStringDict =
            new Dictionary<OutLang, Dictionary<Designs, string>>();

        /// <summary>
        /// 计算方法字典
        /// </summary>
        public static readonly Dictionary<OutLang, Dictionary<Models, string>> ModelsStringDict =
            new Dictionary<OutLang, Dictionary<Models, string>>();

        /// <summary>
        /// 反应类型字典
        /// </summary>
        public static readonly Dictionary<OutLang, Dictionary<Types, string>> TypesStringDict =
            new Dictionary<OutLang, Dictionary<Types, string>>();

        /// <summary>
        /// 异常值检测方法字典
        /// </summary>
        public static readonly Dictionary<OutLang, Dictionary<AbnormalDataCheckMethods, string>> CheckMethodsStringDict
            = new Dictionary<OutLang, Dictionary<AbnormalDataCheckMethods, string>>();

        /// <summary>
        /// 转换公式字典 -- 不变
        /// </summary>
        public static readonly Dictionary<DataTransformationFormula, string> FormulasStringDict = new Dictionary<DataTransformationFormula, string>();

        private static readonly String[] DataTransFormulaString =
            {
                "y", "ln(y)", "lg(y)", "y^2", "sqrt(y)", "1/y"
            };

        /// <summary>
        /// 变异来源字典
        /// </summary>
        public static readonly Dictionary<OutLang, Dictionary<VariationSources, string>> VariationSourcesDict =
            new Dictionary<OutLang, Dictionary<VariationSources, string>>();

        /// <summary>
        /// 变异来源检测结果字典
        /// </summary>
        public static readonly Dictionary<OutLang, Dictionary<VariationSources, Dictionary<bool, string>>>
            VariationSourceCheckResult =
                new Dictionary<OutLang, Dictionary<VariationSources, Dictionary<bool, string>>>();

        /// <summary>
        /// 可靠性检测结果
        /// </summary>
        public static readonly Dictionary<OutLang, Dictionary<bool, string>> SingleCalcReliabilityCheckConclution =
            new Dictionary<OutLang, Dictionary<bool, string>>();

        public static readonly Dictionary<OutLang, Dictionary<bool, String[]>> MergeCalcReliabilityCheckConclution =
            new Dictionary<OutLang, Dictionary<bool, String[]>>();

        public static readonly Dictionary<OutLang, Dictionary<bool, String[]>> MergeCalcDfCheckFalseConclution =
            new Dictionary<OutLang, Dictionary<bool, string[]>>();



        //private static readonly String[] MethodString = { "直接检定法", "平行线检定法", "斜率比检定法", "S型曲检定法", "ED" };

        //private static readonly String[] DesignString = { "完全随机", "随机区组", "拉丁方", "双交叉" };

        //private static readonly String[] ModelString = { "Probit", "Logit", "Gompit", "Angle", "Four Paras" };

        //private static readonly String[] TypeString = { "定量反应", "定性反应" };

        //private static readonly String[] CheckerString = { "Dixon", "Grubb", "Romanovsky", "Hampel", "Quartile" };

        public static readonly Dictionary<OutLang, String[]> DataFirstLineString = new Dictionary<OutLang, string[]>();

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
        public static readonly Dictionary<OutLang, String[]> PotencyEstThirdLineString = new Dictionary<OutLang, string[]>();

        /// <summary>
        /// 合并计算类型
        /// </summary>
        public static readonly Dictionary<OutLang, String[]> MergeCalcTypes = new Dictionary<OutLang, string[]>();

        ///// <summary>
        ///// 变异来源列表
        ///// </summary>
        //private static readonly Dictionary<OutLang, String[]> VariationSourcesList = new Dictionary<OutLang, string[]>();

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

        public static Dictionary<OutLang, String> GetGroupString(int groupId)
        {
            return new Dictionary<OutLang, string>
                {
                    {OutLang.Chinese, ConstStringsCN.GetGroupString(groupId)},
                    {OutLang.English, ConstStringsEN.GetGroupString(groupId)}
                };
        }

        //addition
        public static readonly Dictionary<OutLang, String> Undetermined = new Dictionary<OutLang, string>();

        public static readonly Dictionary<OutLang, String> TestDepartmentNumber = new Dictionary<OutLang, string>();

        public static readonly Dictionary<OutLang, String> Potency = new Dictionary<OutLang, string>();

        public static readonly Dictionary<OutLang, Dictionary<TableCategory, String>> DataTableCategoryString =
            new Dictionary<OutLang, Dictionary<TableCategory, string>>();

        public static readonly Dictionary<OutLang, String> HistogramHeader = new Dictionary<OutLang, string>();

        public static readonly Dictionary<OutLang, String> HistogramXLabel = new Dictionary<OutLang, string>();

        public static readonly Dictionary<OutLang, String> HistogramYLabel = new Dictionary<OutLang, string>();

        public static readonly Dictionary<EDEnum, String> EDString = new Dictionary<EDEnum, string>();

        public static readonly Dictionary<OutLang, String> CompFigure = new Dictionary<OutLang, string>();
    }
}
