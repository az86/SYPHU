using System.Collections.Generic;

namespace SYPHU.Assay.Results.VarianceAnalysisResults
{
    public class SigmoidCurveVarAnaResult
    {
        public BasicVarianceAnalysisValues PrepValues = new BasicVarianceAnalysisValues();
        public BasicVarianceAnalysisValues RegValues = new BasicVarianceAnalysisValues();
        public BasicVarianceAnalysisValues ParValues = new BasicVarianceAnalysisValues();
        public BasicVarianceAnalysisValues LinValues = new BasicVarianceAnalysisValues();
        public List<BasicVarianceAnalysisValues> LinsValues = new List<BasicVarianceAnalysisValues>();
        public BasicVarianceAnalysisValues TreatValues = new BasicVarianceAnalysisValues();
        //public BasicVarianceAnalysisValues ResValues = new BasicVarianceAnalysisValues();
        public BasicVarianceAnalysisValues TotalValues = new BasicVarianceAnalysisValues();
    }
}
