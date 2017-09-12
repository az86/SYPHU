using System;
using System.Collections.Generic;
using SYPHU.Assay.CalculationInfo;

namespace SYPHU.Assay.Tables
{
    /// <summary>
    /// 表格表头信息类
    /// </summary>
    public static class TableHeaders
    {
        static TableHeaders()
        {
            CommonInfoTH.Add(OutLang.Chinese, TableHeadersCN.CommonInfoTH);
            CommonInfoTH.Add(OutLang.English, TableHeadersEN.CommonInfoTH);

            SingleCalcInfoVerticalTH.Add(OutLang.Chinese, TableHeadersCN.SingleCalcInfoVerticalTH);
            SingleCalcInfoVerticalTH.Add(OutLang.English, TableHeadersEN.SingleCalcInfoVerticalTH);

            MergeCalcInfoVerticalTH.Add(OutLang.Chinese, TableHeadersCN.MergeCalcInfoVerticalTH);
            MergeCalcInfoVerticalTH.Add(OutLang.English, TableHeadersEN.MergeCalcInfoVerticalTH);

            DataVerticalTH.Add(OutLang.Chinese, TableHeadersCN.DataVerticalTH);
            DataVerticalTH.Add(OutLang.English, TableHeadersEN.DataVerticalTH);

            LatinConvertTH.Add(OutLang.Chinese, TableHeadersCN.LatinConvertTH);
            LatinConvertTH.Add(OutLang.English, TableHeadersEN.LatinConvertTH);

            VarAnaHorizontalTH.Add(OutLang.Chinese, TableHeadersCN.VarAnaHorizontalTH);
            VarAnaHorizontalTH.Add(OutLang.English, TableHeadersEN.VarAnaHorizontalTH);

            SCVarAnaHorizontalTH.Add(OutLang.Chinese, TableHeadersCN.SCVarAnaHorizontalTH);
            SCVarAnaHorizontalTH.Add(OutLang.English, TableHeadersEN.SCVarAnaHorizontalTH);

            VarAnaVerticalTH.Add(OutLang.Chinese, TableHeadersCN.VarAnaVerticalTH);
            VarAnaVerticalTH.Add(OutLang.English, TableHeadersEN.VarAnaVerticalTH);

            VarAnaConclusionTH.Add(OutLang.Chinese, TableHeadersCN.VarAnaConclusionTH);
            VarAnaConclusionTH.Add(OutLang.English, TableHeadersEN.VarAnaConclusionTH);

            PEVerticalTH.Add(OutLang.Chinese, TableHeadersCN.PEVerticalTH);
            PEVerticalTH.Add(OutLang.English, TableHeadersEN.PEVerticalTH);

            EDPEVerticalTH.Add(OutLang.Chinese, TableHeadersCN.EDPEVerticalTH);
            EDPEVerticalTH.Add(OutLang.English, TableHeadersEN.EDPEVerticalTH);

            MergeCalcPEVerticalTH.Add(OutLang.Chinese, TableHeadersCN.MergeCalcPEVerticalTH);
            MergeCalcPEVerticalTH.Add(OutLang.English, TableHeadersEN.MergeCalcPEVerticalTH);

            DirectVarAnaVerticalTH.Add(OutLang.Chinese, TableHeadersCN.DirectVarAnaVerticalTH);
            DirectVarAnaVerticalTH.Add(OutLang.English, TableHeadersEN.DirectVarAnaVerticalTH);

            DirectPEVerticalTH.Add(OutLang.Chinese, TableHeadersCN.DirectPEVerticalTH);
            DirectPEVerticalTH.Add(OutLang.English, TableHeadersEN.DirectPEVerticalTH);

            MergeCalcFullHorizontalTH.Add(OutLang.Chinese, TableHeadersCN.MergeCalcFullHorizontalTH);
            MergeCalcFullHorizontalTH.Add(OutLang.English, TableHeadersEN.MergeCalcFullHorizontalTH);

            MergeCalcSmHorizontalTH.Add(OutLang.Chinese, TableHeadersCN.MergeCalcSmHorizontalTH);
            MergeCalcSmHorizontalTH.Add(OutLang.English, TableHeadersEN.MergeCalcSmHorizontalTH);
        }

        #region 基本信息表头

        public static readonly Dictionary<OutLang, String[]> CommonInfoTH = new Dictionary<OutLang, string[]>();

        #endregion

        #region 计算信息表头

        public static readonly Dictionary<OutLang, String[]> SingleCalcInfoVerticalTH = new Dictionary<OutLang, string[]>();

        public static readonly Dictionary<OutLang, String[]> MergeCalcInfoVerticalTH = new Dictionary<OutLang, string[]>();

        #endregion

        #region 数据表头

        public static readonly Dictionary<OutLang, String[]> DataVerticalTH = new Dictionary<OutLang, string[]>();

        public static readonly Dictionary<OutLang, String[]> LatinConvertTH = new Dictionary<OutLang, string[]>();

        #endregion

        #region 方差分析表头

        public static readonly Dictionary<OutLang, String[]> VarAnaHorizontalTH = new Dictionary<OutLang, string[]>();

        public static readonly Dictionary<OutLang, String[]> SCVarAnaHorizontalTH = new Dictionary<OutLang, string[]>();

        public static readonly Dictionary<OutLang, Dictionary<Methods, Dictionary<Designs, String[]>>> VarAnaVerticalTH
            = new Dictionary<OutLang, Dictionary<Methods, Dictionary<Designs, string[]>>>();

        //private static readonly String[] PLCompletelyRandomisedVarAnaVerticalTH =
        //    {
        //        "试品间", "回归", "非平行", "非线性", "剂间", "残差", "总和"
        //    };

        //private static readonly String[] PLRandomisedBlockVarAnaVerticalTH =
        //    {
        //        "试品间", "回归", "非平行", "非线性", "剂间", "区组间", "残差", "总和"
        //    };

        //private static readonly String[] PLLatinSquareVarAnaVerticalTH =
        //    {
        //        "试品间", "回归", "非平行", "非线性", "剂间", "行间", "列间", "残差", "总和"
        //    };

        //private static readonly String[] PLCrossOverVarAnaVerticalTH =
        //    {
        //        "非平行", "次间×试品间", "次间×回归", "动物组间残差", "动物间", "试品间", "回归", "次间", "次间×非平行", "动物组内残差", "总和"
        //    };

        //private static readonly String[] SRCompletelyRandomisedVarAnaVerticalTH = { "回归", "相交性", "非线性", "剂间", "残差", "总和" };

        //private static readonly String[] SRRandomisedBlockVarAnaVerticalTH = { "回归", "相交性", "非线性", "剂间", "区组间", "残差", "总和" };

        //private static readonly String[] SRLatinSquareVarAnaVerticalTH = { "回归", "相交性", "非线性", "剂间", "行间", "列间", "残差", "总和" };

        //private static readonly String[] SCCompletelyRandomisedVarAnaVerticalTH = { "试品间", "回归", "非平行", "非线性", "剂间", "理论误差", "总和" };

        //private static readonly String[] EDCompletelyRandomisedVarAnaVerticalTH = { "回归", "非线性", "剂间",/* "残差", */"理论误差", "总和" };

        #endregion

        #region 可靠性分析结论表头

        public static readonly Dictionary<OutLang, String[]> VarAnaConclusionTH = new Dictionary<OutLang, string[]>();

        #endregion

        #region 效价估计表头

        public static readonly Dictionary<OutLang, String[]> PEVerticalTH = new Dictionary<OutLang, string[]>();

        public static readonly Dictionary<OutLang, String[]> EDPEVerticalTH = new Dictionary<OutLang, string[]>();

        public static readonly Dictionary<OutLang, String[]> MergeCalcPEVerticalTH = new Dictionary<OutLang, string[]>();

        #endregion

        #region 直接法计算结果表格

        public static readonly Dictionary<OutLang, String[]> DirectVarAnaVerticalTH = new Dictionary<OutLang, string[]>();

        public static readonly Dictionary<OutLang, String[]> DirectPEVerticalTH = new Dictionary<OutLang, string[]>();

        #endregion

        #region 合并计算表格

        public static readonly Dictionary<OutLang, String[]> MergeCalcFullHorizontalTH = new Dictionary<OutLang, string[]>();

        public static readonly Dictionary<OutLang, String[]> MergeCalcSmHorizontalTH = new Dictionary<OutLang, string[]>();

        #endregion
    }
}
