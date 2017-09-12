using System.Collections.Generic;

namespace SYPHU.Assay.Results.VarianceAnalysisResults
{
    public class ParallelLinesVarAnaResult
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
        public double Hp;
        public double HL;
        public double K;

        public List<double> KList;
        public List<double> SStreatList;
        public List<double> SSregList;

        #endregion

        public BasicVarianceAnalysisValues PrepValues = new BasicVarianceAnalysisValues();
        public BasicVarianceAnalysisValues RegValues = new BasicVarianceAnalysisValues();
        public BasicVarianceAnalysisValues ParValues = new BasicVarianceAnalysisValues();
        public BasicVarianceAnalysisValues TotalValues = new BasicVarianceAnalysisValues();
    }

    public class PLCompletelyRandomisedVarAnaResult : ParallelLinesVarAnaResult
    {
        public BasicVarianceAnalysisValues LinValues = new BasicVarianceAnalysisValues();
        public List<BasicVarianceAnalysisValues> LinsValues = new List<BasicVarianceAnalysisValues>();
        public BasicVarianceAnalysisValues TreatValues = new BasicVarianceAnalysisValues();
        public BasicVarianceAnalysisValues ResValues = new BasicVarianceAnalysisValues();
    }

    public class PLRandomisedBlockVarAnaResult : PLCompletelyRandomisedVarAnaResult
    {
        public BasicVarianceAnalysisValues RowValues = new BasicVarianceAnalysisValues();
    }

    public class PLLatinSquareVarAnaResult : PLCompletelyRandomisedVarAnaResult
    {
        public BasicVarianceAnalysisValues RowValues = new BasicVarianceAnalysisValues();
        public BasicVarianceAnalysisValues ColValues = new BasicVarianceAnalysisValues();
    }

    public class PLCrossOverVarAnaResult : ParallelLinesVarAnaResult
    {
        /// <summary>
        /// Bij
        /// </summary>
        public List<List<double>> AverageInGroup;

        public List<List<List<double>>> SampleAverageSorted;

        public List<double> D;

        public double N;

        public List<List<double>> SampleAverageSum2;

        public List<List<double>> L2;

        public List<double> Hp2;

        public List<double> HL2;

        public List<double> K2;

        public BasicVarianceAnalysisValues DaysPrep = new BasicVarianceAnalysisValues();
        public BasicVarianceAnalysisValues DaysReg = new BasicVarianceAnalysisValues();
        public BasicVarianceAnalysisValues ResBetw = new BasicVarianceAnalysisValues();
        public BasicVarianceAnalysisValues Rab = new BasicVarianceAnalysisValues();
        public BasicVarianceAnalysisValues Days = new BasicVarianceAnalysisValues();
        public BasicVarianceAnalysisValues DaysPar = new BasicVarianceAnalysisValues();
        public BasicVarianceAnalysisValues ResWith = new BasicVarianceAnalysisValues();
    }
}
