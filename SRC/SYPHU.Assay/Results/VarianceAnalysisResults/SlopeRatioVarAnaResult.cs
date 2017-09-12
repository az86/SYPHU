using System.Collections.Generic;

namespace SYPHU.Assay.Results.VarianceAnalysisResults
{
    public class SlopeRatioVarAnaResult
    {
        #region 预计算参数

        /// <summary>
        /// S1, S2; T1,T2; U1,U2
        /// </summary>
        public List<List<double>> SampleAverage;

        /// <summary>
        /// Ps, Pt, Pu
        /// </summary>
        public List<double> SampleAverageSum;

        public List<double> L;

        public List<double> aList;

        public List<double> bList;

        public List<double> GList;

        public List<double> JList;

        public double HB;

        public double HI;

        public double a;

        public double K;

        #endregion
        
        public BasicVarianceAnalysisValues RegValues = new BasicVarianceAnalysisValues();
        public BasicVarianceAnalysisValues IntersValues = new BasicVarianceAnalysisValues();
        public BasicVarianceAnalysisValues LinValues = new BasicVarianceAnalysisValues();
        public List<BasicVarianceAnalysisValues> LinsValues = new List<BasicVarianceAnalysisValues>();
        public BasicVarianceAnalysisValues TreatValues = new BasicVarianceAnalysisValues();
        public BasicVarianceAnalysisValues ResValues = new BasicVarianceAnalysisValues();
        public BasicVarianceAnalysisValues TotalValues = new BasicVarianceAnalysisValues();
    }

    public class SRCompletelyRandomisedVarAnaResult : SlopeRatioVarAnaResult
    {
        
    }

    public class SRRandomisedBlockVarAnaResult : SlopeRatioVarAnaResult
    {
        public BasicVarianceAnalysisValues RowValues = new BasicVarianceAnalysisValues();
    }

    public class SRLatinSquareVarAnaResult : SlopeRatioVarAnaResult
    {
        public BasicVarianceAnalysisValues RowValues = new BasicVarianceAnalysisValues();
        public BasicVarianceAnalysisValues ColValues = new BasicVarianceAnalysisValues();
    }
}
