using System;
using System.Collections.Generic;
using SYPHU.Assay.CalculationInfo;

namespace SYPHU.Assay.Tables
{
    /// <summary>
    /// 表格表头信息类
    /// </summary>
    public static class TableHeadersEN
    {
        #region 基本信息表头

        public static readonly string[] CommonInfoTH = { "Department", "Reporter", "Reviewer", "Date"};

        #endregion

        #region 计算信息表头

        public static readonly string[] SingleCalcInfoVerticalTH =
            {
                "Drug name", "Statistics model", "Assay design", "Calculation method", "Response type", "Transformation", "Outlier Tests"
            };

        public static readonly string[] MergeCalcInfoVerticalTH =
            {
                "Drug name", "Statistics model","Transformation", "Outlier Tests"
            };

        #endregion

        #region 数据表头

        public static readonly String[] DataVerticalTH = { "", "Id.", "Ass.pot.", "Pre-dilution", "doses" };

        public static readonly String[] LatinConvertTH = { "Transformation of Latin square design table (Standard ->Assay)" };

        #endregion

        #region 方差分析表头

        public static readonly String[] VarAnaHorizontalTH = { "Source of variation", "Degrees of freedom", "Sum of squares", "Mean square", "F-ratio", "Probability" };

        public static readonly String[] SCVarAnaHorizontalTH = { "Source of variation", "Degrees of freedom", "Sum of squares", "Mean square", "Chi-square", "Probability" };

        public static readonly Dictionary<Methods, Dictionary<Designs, String[]>> VarAnaVerticalTH = new Dictionary<Methods, Dictionary<Designs, string[]>>();

        static TableHeadersEN()
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
                "Preparations", "Regression", "Non-parallelism", "Non-linearity", "Treatments", "Residual error", "Total"
            };

        private static readonly String[] PLRandomisedBlockVarAnaVerticalTH =
            {
                "Preparations", "Regression", "Non-parallelism", "Non-linearity", "Treatments", "Blocks", "Residual error", "Total"
            };

        private static readonly String[] PLLatinSquareVarAnaVerticalTH =
            {
                "Preparations", "Regression", "Non-parallelism", "Non-linearity", "Treatments", "Rows", "Columns", "Residual error", "Total"
            };

        private static readonly String[] PLCrossOverVarAnaVerticalTH =
            {
                "Non-parallelism", "Days×Preparations", "Days×Regression", "Residual error between rabbits", "Rabbits", "Preparations", "Regression", "Days", "Days×Non-parallelism", "Residual error within rabbits", "Total"
            };

        private static readonly String[] SRCompletelyRandomisedVarAnaVerticalTH = { "Regression", "Intersection", "Non-linearity", "Treatments", "Residual error", "Total" };

        private static readonly String[] SRRandomisedBlockVarAnaVerticalTH = { "Regression", "Intersection", "Non-linearity", "Treatments", "Blocks", "Residual error", "Total" };

        private static readonly String[] SRLatinSquareVarAnaVerticalTH = { "Regression", "Intersection", "Non-linearity", "Treatments", "Rows", "Columns", "Residual error", "Total" };

        private static readonly String[] SCCompletelyRandomisedVarAnaVerticalTH = { "Preparations", "Regression", "Non-parallelism", "Non-linearity", "Treatments", "Theoretical variance", "Total" };

        private static readonly String[] EDCompletelyRandomisedVarAnaVerticalTH = { "Regression", "Non-linearity", "Treatments",/* "Residual error", */"Theoretical variance", "Total" };

        #endregion

        #region 可靠性分析结论表头

        public static readonly String[] VarAnaConclusionTH = { "Results of validity tests" };

        #endregion

        #region 效价估计表头

        public static readonly String[] PEVerticalTH =
            {
                "Sample", "Id.", "", "Potency", "Rel.to Ass.pot", "Rel.to Est.pot", "Standard error ", "FL%"
            };

        public static readonly String[] EDPEVerticalTH =
            {
                "Sample", "Id.", "", "","","Standard error "
            };

        public static readonly String[] MergeCalcPEVerticalTH =
            {
                "Sample", "Combination case", "", "Potency", "Rel.to Ass.pot", "Rel.to Est.pot", "Standard error ", "FL%"
            };

        #endregion

        #region 直接法计算结果表格

        public static readonly String[] DirectVarAnaVerticalTH = { "degrees of freedom", "T-value" };

        public static readonly String[] DirectPEVerticalTH = { "Sample", "Id.", "", "Estimate potency", "Standard error ", "FL%" };

        #endregion

        #region 合并计算表格

        public static readonly String[] MergeCalcFullHorizontalTH = { "Assay", "Sample", "Lower limit", "Est.pot.", "Upper limit", "Degrees of freedom" };

        public static readonly String[] MergeCalcSmHorizontalTH = { "Assay", "Sample", "Est.pot.", "Standard error ", "Degrees of freedom" };

        #endregion

    }
}
