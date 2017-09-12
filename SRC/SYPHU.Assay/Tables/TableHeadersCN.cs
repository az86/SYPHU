using System;
using System.Collections.Generic;
using SYPHU.Assay.CalculationInfo;

namespace SYPHU.Assay.Tables
{
    /// <summary>
    /// 表格表头信息类
    /// </summary>
    public static class TableHeadersCN
    {
        #region 基本信息表头

        public static readonly string[] CommonInfoTH = {"试验单位", "报告人", "复核人", "分析日期"};

        #endregion

        #region 计算信息表头

        public static readonly string[] SingleCalcInfoVerticalTH =
            {
                "药品名称", "统计模型", "试验设计", "计算方法", "反应类型", "转换公式", "异常数据检测方法"
            };

        public static readonly string[] MergeCalcInfoVerticalTH =
            {
                "药品名称", "统计模型","转换公式", "异常数据检测方法"
            };

        #endregion

        #region 数据表头

        public static readonly String[] DataVerticalTH = {"", "组别", "标示效价(A)", "稀释倍数(dil)", "剂量(dose)"};

        public static readonly String[] LatinConvertTH = {"拉丁方设计数据转换表 (标准表->试验表)"};

        #endregion

        #region 方差分析表头

        public static readonly String[] VarAnaHorizontalTH = {"变异来源", "自由度", "平方和", "均方", "F", "P"};

        public static readonly String[] SCVarAnaHorizontalTH = { "变异来源", "自由度", "平方和", "均方", "卡方", "P" };

        public static readonly Dictionary<Methods, Dictionary<Designs, String[]>> VarAnaVerticalTH = new Dictionary<Methods, Dictionary<Designs, string[]>>();

        static TableHeadersCN()
        {
            VarAnaVerticalTH.Add(Methods.ParallelLine, new Dictionary<Designs, string[]>());
            VarAnaVerticalTH[Methods.ParallelLine].Add(Designs.CompletelyRandomised, PLCompletelyRandomisedVarAnaVerticalTH);
            VarAnaVerticalTH[Methods.ParallelLine].Add(Designs.RandomisedBlock, PLRandomisedBlockVarAnaVerticalTH);
            VarAnaVerticalTH[Methods.ParallelLine].Add(Designs.LatinSquare, PLLatinSquareVarAnaVerticalTH);
            VarAnaVerticalTH[Methods.ParallelLine].Add(Designs.CrossOver, PLCrossOverVarAnaVerticalTH);

            VarAnaVerticalTH.Add(Methods.SlopeRatio, new Dictionary<Designs, string[]>());
            VarAnaVerticalTH[Methods.SlopeRatio].Add(Designs.CompletelyRandomised, SRCompletelyRandomisedVarAnaVerticalTH);
            VarAnaVerticalTH[Methods.SlopeRatio].Add(Designs.RandomisedBlock, SRRandomisedBlockVarAnaVerticalTH);
            VarAnaVerticalTH[Methods.SlopeRatio].Add(Designs.LatinSquare, SRLatinSquareVarAnaVerticalTH);

            VarAnaVerticalTH.Add(Methods.SigmoidCurve, new Dictionary<Designs, string[]>());
            VarAnaVerticalTH[Methods.SigmoidCurve].Add(Designs.CompletelyRandomised, SCCompletelyRandomisedVarAnaVerticalTH);

            VarAnaVerticalTH.Add(Methods.ED, new Dictionary<Designs, string[]>());
            VarAnaVerticalTH[Methods.ED].Add(Designs.CompletelyRandomised, EDCompletelyRandomisedVarAnaVerticalTH);
        }

        private static readonly String[] PLCompletelyRandomisedVarAnaVerticalTH =
            {
                "试品间", "回归", "非平行", "非线性", "剂间", "残差", "总和"
            };

        private static readonly String[] PLRandomisedBlockVarAnaVerticalTH =
            {
                "试品间", "回归", "非平行", "非线性", "剂间", "区组间", "残差", "总和"
            };

        private static readonly String[] PLLatinSquareVarAnaVerticalTH =
            {
                "试品间", "回归", "非平行", "非线性", "剂间", "行间", "列间", "残差", "总和"
            };

        private static readonly String[] PLCrossOverVarAnaVerticalTH =
            {
                "非平行", "次间×试品间", "次间×回归", "动物组间残差", "动物间", "试品间", "回归", "次间", "次间×非平行", "动物组内残差", "总和"
            };

        private static readonly String[] SRCompletelyRandomisedVarAnaVerticalTH = { "回归", "相交性", "非线性", "剂间", "残差", "总和" };

        private static readonly String[] SRRandomisedBlockVarAnaVerticalTH = { "回归", "相交性", "非线性", "剂间", "区组间", "残差", "总和" };

        private static readonly String[] SRLatinSquareVarAnaVerticalTH = { "回归", "相交性", "非线性", "剂间", "行间", "列间", "残差", "总和" };

        private static readonly String[] SCCompletelyRandomisedVarAnaVerticalTH = { "试品间", "回归", "非平行", "非线性", "剂间", "理论误差", "总和" };

        private static readonly String[] EDCompletelyRandomisedVarAnaVerticalTH = { "回归", "非线性", "剂间",/* "残差", */"理论误差", "总和" };

        #endregion

        #region 可靠性分析结论表头

        public static readonly String[] VarAnaConclusionTH = {"可靠性分析结论"};

        #endregion

        #region 效价估计表头

        public static readonly String[] PEVerticalTH =
            {
                "试验组", "组别", "", "效价", "相对标示效价变化率", "相对估计效价变化率", "标准误(Sm)", "可信限率"
            };

        public static readonly String[] EDPEVerticalTH =
            {
                "试验组", "组别", "", "","","标准误(Sm)"
            };

        public static readonly String[] MergeCalcPEVerticalTH =
            {
                "试验组", "合并类型", "", "效价", "相对标示效价变化率", "相对估计效价变化率", "标准误(Sm)", "可信限率"
            };

        #endregion

        #region 直接法计算结果表格

        public static readonly String[] DirectVarAnaVerticalTH = {"自由度", "t值"};

        public static readonly String[] DirectPEVerticalTH = { "试验组", "组别", "", "效价", "标准误(Sm)", "可信限率" };

        #endregion

        #region 合并计算表格

        public static readonly String[] MergeCalcFullHorizontalTH = {"序号", "组别", "下限", "效价", "上限", "自由度"};

        public static readonly String[] MergeCalcSmHorizontalTH = {"序号", "组别", "效价", "标准误", "自由度"};

        #endregion

    }
}
