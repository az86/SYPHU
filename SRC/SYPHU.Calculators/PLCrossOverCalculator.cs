using System;
using System.Collections.Generic;
using System.Linq;
using SYPHU.Assay.CalculationInfo;
using SYPHU.Assay.Results.ReliabilityCheck;
using SYPHU.Assay.Results.VarianceAnalysisResults;
using SYPHU.Assay.Tables;
using SYPHU.Utilities;

namespace SYPHU.Calculators
{
    /// <summary>
    /// 平行线法双交叉设计
    /// </summary>
    public class PLCrossOverCalculator : ParallelLineMensurationCalculator
    {
        private readonly int _h;

        private readonly int _d;

        private readonly double _dh;

        private readonly double _dd;

        private double _dn;

        public PLCrossOverCalculator()
        {
            _d = 2;
            _h = 2;
            _dh = 2.0;
            _dd = 2.0;
            
            VAResult = new PLCrossOverVarAnaResult();
        }

        public override void DoCalculate(InitCalculationInfo calculationInfo, int extremeAbnormalDataNum, List<List<double>> bList)
        {
            CalcInfo = calculationInfo;
            _dn = CalcInfo.DataSize.ReplicateNum;
            ExtremeAbnormalDataNum = extremeAbnormalDataNum;
            BList = bList;
            VarAnaPreCalc();
            VarAnaCalc();
            PEPreCalc();
            PECalc(_h);
        }

        #region 方差分析预计算

        protected override void VarAnaPreCalc()
        {
            CalcAverage();
            SortAverage();
            CalcAverageSum();
            CalcL();
            CalcH();
            CalcK();
            CalcB();
        }

        private void SortAverage()
        {
            ((PLCrossOverVarAnaResult) VAResult).SampleAverageSorted = new List<List<List<double>>>();

            ((PLCrossOverVarAnaResult) VAResult).SampleAverageSorted.Add(new List<List<double>>());
            //S11,S21
            ((PLCrossOverVarAnaResult) VAResult).SampleAverageSorted[0].Add(new List<double>());
            ((PLCrossOverVarAnaResult) VAResult).SampleAverageSorted[0][0].Add(VAResult.SampleAverage[0][0]);
            ((PLCrossOverVarAnaResult) VAResult).SampleAverageSorted[0][0].Add(VAResult.SampleAverage[1][0]);
            //S12,S22
            ((PLCrossOverVarAnaResult) VAResult).SampleAverageSorted[0].Add(new List<double>());
            ((PLCrossOverVarAnaResult) VAResult).SampleAverageSorted[0][1].Add(VAResult.SampleAverage[3][1]);
            ((PLCrossOverVarAnaResult) VAResult).SampleAverageSorted[0][1].Add(VAResult.SampleAverage[2][1]);

            ((PLCrossOverVarAnaResult) VAResult).SampleAverageSorted.Add(new List<List<double>>());
            //T11,T21
            ((PLCrossOverVarAnaResult) VAResult).SampleAverageSorted[1].Add(new List<double>());
            ((PLCrossOverVarAnaResult) VAResult).SampleAverageSorted[1][0].Add(VAResult.SampleAverage[2][0]);
            ((PLCrossOverVarAnaResult) VAResult).SampleAverageSorted[1][0].Add(VAResult.SampleAverage[3][0]);
            //T12,T22
            ((PLCrossOverVarAnaResult) VAResult).SampleAverageSorted[1].Add(new List<double>());
            ((PLCrossOverVarAnaResult) VAResult).SampleAverageSorted[1][1].Add(VAResult.SampleAverage[1][1]);
            ((PLCrossOverVarAnaResult) VAResult).SampleAverageSorted[1][1].Add(VAResult.SampleAverage[0][1]);

            ((PLCrossOverVarAnaResult) VAResult).D = new List<double>();
            ((PLCrossOverVarAnaResult) VAResult).D.Add(
                (((PLCrossOverVarAnaResult) VAResult).SampleAverageSorted[0][0].Sum() +
                 ((PLCrossOverVarAnaResult) VAResult).SampleAverageSorted[1][0].Sum())*0.25);
            ((PLCrossOverVarAnaResult) VAResult).D.Add(
                (((PLCrossOverVarAnaResult) VAResult).SampleAverageSorted[0][1].Sum() +
                 ((PLCrossOverVarAnaResult) VAResult).SampleAverageSorted[1][1].Sum())*0.25);

            ((PLCrossOverVarAnaResult) VAResult).N = 2*_dh*_dd*_dn;
        }

        private void CalcAverageSum()
        {
            ((PLCrossOverVarAnaResult) VAResult).SampleAverageSum2 = new List<List<double>>();
            VAResult.SampleAverageSum = new List<double>();
            for (int i = 0; i < _h; i++)
            {
                ((PLCrossOverVarAnaResult) VAResult).SampleAverageSum2.Add(new List<double>());
                for (int j = 0; j < _d; j++)
                {
                    ((PLCrossOverVarAnaResult) VAResult).SampleAverageSum2[i].Add(
                        ((PLCrossOverVarAnaResult) VAResult).SampleAverageSorted[i][j].Sum());
                }
                VAResult.SampleAverageSum.Add(((PLCrossOverVarAnaResult) VAResult).SampleAverageSum2[i].Average());
            }
        }

        private void CalcL()
        {
            ((PLCrossOverVarAnaResult) VAResult).L2 = new List<List<double>>();
            VAResult.L = new List<double>();
            for (int i = 0; i < _h; i++)
            {
                ((PLCrossOverVarAnaResult) VAResult).L2.Add(new List<double>());
                for (int j = 0; j < _d; j++)
                {
                    ((PLCrossOverVarAnaResult) VAResult).L2[i].Add(
                        ((PLCrossOverVarAnaResult) VAResult).SampleAverageSorted[i][j][0] +
                        2*((PLCrossOverVarAnaResult) VAResult).SampleAverageSorted[i][j][1] -
                        0.5*(_dd + 1.0)*((PLCrossOverVarAnaResult) VAResult).SampleAverageSum2[i][j]);
                }
                VAResult.L.Add(((PLCrossOverVarAnaResult) VAResult).L2[i].Average());
            }
        }

        private void CalcH()
        {
            double vp = _dn/_dd;
            double vL = 12*_dn/(_dd*_dd*_dd - _dd);
            ((PLCrossOverVarAnaResult) VAResult).Hp2 = new List<double>();
            ((PLCrossOverVarAnaResult) VAResult).HL2 = new List<double>();
            for (int i = 0; i < _h; i++)
            {
                ((PLCrossOverVarAnaResult) VAResult).Hp2.Add(vp);
                ((PLCrossOverVarAnaResult) VAResult).HL2.Add(vL);
            }
            VAResult.Hp = 2*vp;
            VAResult.HL = 2*vL;
        }

        private void CalcK()
        {
            ((PLCrossOverVarAnaResult) VAResult).K2 = new List<double>();
            double ss = 0;
            for (int i = 0; i < _h; i++)
            {
                double s = ((PLCrossOverVarAnaResult) VAResult).SampleAverageSum2[0][i] +
                           ((PLCrossOverVarAnaResult) VAResult).SampleAverageSum2[1][i];
                ss += s;
                ((PLCrossOverVarAnaResult) VAResult).K2.Add(_dn/_dh/_dd*s*s);
            }
            VAResult.K = 0.5*_dn/_dh/_dd*ss*ss;
        }

        private void CalcB()
        {
            ((PLCrossOverVarAnaResult) VAResult).AverageInGroup = new List<List<double>>();
            for (int i = 0; i < CalcInfo.DataSize.PreparationNum; i++)
            {
                ((PLCrossOverVarAnaResult) VAResult).AverageInGroup.Add(new List<double>());
                for (int j = 0; j < CalcInfo.DataSize.ReplicateNum; j++)
                {
                    ((PLCrossOverVarAnaResult) VAResult).AverageInGroup[i].Add(0.5*
                                                                               (AssData.Data[i][0][j] +
                                                                                AssData.Data[i][1][j]));
                }
            }
        }

        #endregion

        protected override void CalcSM(int i, double secItem)
        {
            PEResult.PEValues[i].SM = secItem /
                                      Distributions.Dist_t(
                                          ((PLCrossOverVarAnaResult)VAResult).ResBetw.FreedomDegree) / Math.Log(10);
        }

        #region 方差分析

        protected override void CalcFreedom()
        {
            VAResult.PrepValues.FreedomDegree = _h - 1;
            VAResult.RegValues.FreedomDegree = 1;
            VAResult.ParValues.FreedomDegree = _h - 1;
            ((PLCrossOverVarAnaResult) VAResult).DaysPrep.FreedomDegree = _h - 1;
            ((PLCrossOverVarAnaResult) VAResult).DaysReg.FreedomDegree = 1;
            ((PLCrossOverVarAnaResult) VAResult).ResBetw.FreedomDegree = _h*_d*(CalcInfo.DataSize.ReplicateNum - 1);
            ((PLCrossOverVarAnaResult) VAResult).Rab.FreedomDegree = _h*_d*CalcInfo.DataSize.ReplicateNum - 1;
            ((PLCrossOverVarAnaResult) VAResult).Days.FreedomDegree = 1;
            ((PLCrossOverVarAnaResult) VAResult).DaysPar.FreedomDegree = 1;
            ((PLCrossOverVarAnaResult) VAResult).ResWith.FreedomDegree = _h*_d*(CalcInfo.DataSize.ReplicateNum - 1);
            VAResult.TotalValues.FreedomDegree = 2*_h*_d*CalcInfo.DataSize.ReplicateNum - 1 - ExtremeAbnormalDataNum;
        }

        protected override void CalcSquareSum()
        {
            base.CalcSquareSum();
            VAResult.RegValues.SquareSum *= 2;
            VAResult.ParValues.SquareSum = VAResult.HL * VAResult.L.SquareSum() -
                                               VAResult.RegValues.SquareSum;
            var prepSS = new List<double>();
            var regSS = new List<double>();
            var parSS = new List<double>();
            for (int i = 0; i < _h; i++)
            {
                prepSS.Add(((PLCrossOverVarAnaResult) VAResult).Hp2[i]*
                           (((PLCrossOverVarAnaResult) VAResult).SampleAverageSum2[0][i]*
                            ((PLCrossOverVarAnaResult) VAResult).SampleAverageSum2[0][i] +
                            ((PLCrossOverVarAnaResult) VAResult).SampleAverageSum2[1][i]*
                            ((PLCrossOverVarAnaResult) VAResult).SampleAverageSum2[1][i]) -
                           ((PLCrossOverVarAnaResult) VAResult).K2[i]);
                regSS.Add(((PLCrossOverVarAnaResult) VAResult).HL2[i]*
                          (((PLCrossOverVarAnaResult) VAResult).L2[0][i] + ((PLCrossOverVarAnaResult) VAResult).L2[1][i])*
                          (((PLCrossOverVarAnaResult) VAResult).L2[0][i] + ((PLCrossOverVarAnaResult) VAResult).L2[1][i])/
                          _dh);
                parSS.Add(((PLCrossOverVarAnaResult) VAResult).HL2[i]*
                          (((PLCrossOverVarAnaResult) VAResult).L2[0][i]*((PLCrossOverVarAnaResult) VAResult).L2[0][i] +
                           ((PLCrossOverVarAnaResult) VAResult).L2[1][i]*((PLCrossOverVarAnaResult) VAResult).L2[1][i]) -
                          regSS[i]);
            }
            ((PLCrossOverVarAnaResult) VAResult).Days.SquareSum = 0.5*((PLCrossOverVarAnaResult) VAResult).N*
                                                                  ((PLCrossOverVarAnaResult) VAResult).D.SquareSum() -
                                                                  VAResult.K;
            ((PLCrossOverVarAnaResult) VAResult).Rab.SquareSum = -VAResult.K;
            for (int i = 0; i < CalcInfo.DataSize.PreparationNum; i++)
            {
                ((PLCrossOverVarAnaResult) VAResult).Rab.SquareSum += 2*
                                                                      ((PLCrossOverVarAnaResult) VAResult)
                                                                          .AverageInGroup[i].SquareSum();
            }

            ((PLCrossOverVarAnaResult) VAResult).DaysPrep.SquareSum = prepSS.Sum() - VAResult.PrepValues.SquareSum;
            ((PLCrossOverVarAnaResult) VAResult).DaysReg.SquareSum = regSS.Sum() - VAResult.RegValues.SquareSum;
            ((PLCrossOverVarAnaResult) VAResult).ResBetw.SquareSum =
                ((PLCrossOverVarAnaResult) VAResult).Rab.SquareSum -
                ((PLCrossOverVarAnaResult) VAResult).DaysReg.SquareSum -
                ((PLCrossOverVarAnaResult) VAResult).DaysPrep.SquareSum - VAResult.ParValues.SquareSum;
            ((PLCrossOverVarAnaResult) VAResult).DaysPar.SquareSum = parSS.Sum() - VAResult.ParValues.SquareSum;
            ((PLCrossOverVarAnaResult) VAResult).ResWith.SquareSum = VAResult.TotalValues.SquareSum -
                                                                     ((PLCrossOverVarAnaResult) VAResult).Rab.SquareSum -
                                                                     VAResult.PrepValues.SquareSum -
                                                                     VAResult.RegValues.SquareSum -
                                                                     ((PLCrossOverVarAnaResult) VAResult).Days.SquareSum -
                                                                     ((PLCrossOverVarAnaResult) VAResult).DaysPar
                                                                                                         .SquareSum;
        }

        protected override void CalcFValues()
        {
            VAResult.ParValues.FValue = VAResult.ParValues.MeanSquare/
                                        ((PLCrossOverVarAnaResult)VAResult).ResBetw.MeanSquare;
            ((PLCrossOverVarAnaResult) VAResult).DaysPrep.FValue =
                ((PLCrossOverVarAnaResult) VAResult).DaysPrep.MeanSquare/
                ((PLCrossOverVarAnaResult)VAResult).ResBetw.MeanSquare;
            ((PLCrossOverVarAnaResult) VAResult).DaysReg.FValue =
                ((PLCrossOverVarAnaResult) VAResult).DaysReg.MeanSquare/
                ((PLCrossOverVarAnaResult)VAResult).ResBetw.MeanSquare;

            ((PLCrossOverVarAnaResult) VAResult).Rab.FValue =
                ((PLCrossOverVarAnaResult) VAResult).Rab.MeanSquare/
                ((PLCrossOverVarAnaResult)VAResult).ResWith.MeanSquare;
            VAResult.PrepValues.FValue =
                VAResult.PrepValues.MeanSquare / ((PLCrossOverVarAnaResult)VAResult).ResWith.MeanSquare;
            VAResult.RegValues.FValue =
                VAResult.RegValues.MeanSquare / ((PLCrossOverVarAnaResult)VAResult).ResWith.MeanSquare;
            ((PLCrossOverVarAnaResult) VAResult).Days.FValue =
                ((PLCrossOverVarAnaResult) VAResult).Days.MeanSquare/
                ((PLCrossOverVarAnaResult)VAResult).ResWith.MeanSquare;
            ((PLCrossOverVarAnaResult) VAResult).DaysPar.FValue =
                ((PLCrossOverVarAnaResult) VAResult).DaysPar.MeanSquare/
                ((PLCrossOverVarAnaResult)VAResult).ResWith.MeanSquare;
        }

        protected override void CalcPValues()
        {
            VAResult.ParValues.PValue = Distributions.Dist_F(VAResult.ParValues.FreedomDegree,
                                                             ((PLCrossOverVarAnaResult) VAResult).ResBetw.FreedomDegree,
                                                             VAResult.ParValues.FValue);
            ((PLCrossOverVarAnaResult) VAResult).DaysPrep.PValue =
                Distributions.Dist_F(((PLCrossOverVarAnaResult) VAResult).DaysPrep.FreedomDegree,
                                     ((PLCrossOverVarAnaResult) VAResult).ResBetw.FreedomDegree,
                                     ((PLCrossOverVarAnaResult) VAResult).DaysPrep.FValue);
            ((PLCrossOverVarAnaResult) VAResult).DaysReg.PValue =
                Distributions.Dist_F(((PLCrossOverVarAnaResult) VAResult).DaysReg.FreedomDegree,
                                     ((PLCrossOverVarAnaResult) VAResult).ResBetw.FreedomDegree,
                                     ((PLCrossOverVarAnaResult) VAResult).DaysReg.FValue);

            ((PLCrossOverVarAnaResult) VAResult).Rab.PValue =
                Distributions.Dist_F(((PLCrossOverVarAnaResult) VAResult).Rab.FreedomDegree,
                                     ((PLCrossOverVarAnaResult) VAResult).ResWith.FreedomDegree,
                                     ((PLCrossOverVarAnaResult) VAResult).Rab.FValue);
            VAResult.PrepValues.PValue =
                Distributions.Dist_F(VAResult.PrepValues.FreedomDegree,
                                     ((PLCrossOverVarAnaResult) VAResult).ResWith.FreedomDegree,
                                     VAResult.PrepValues.FValue);
            VAResult.RegValues.PValue =
                Distributions.Dist_F(VAResult.RegValues.FreedomDegree,
                                     ((PLCrossOverVarAnaResult) VAResult).ResWith.FreedomDegree,
                                     VAResult.RegValues.FValue);
            ((PLCrossOverVarAnaResult) VAResult).Days.PValue =
                Distributions.Dist_F(((PLCrossOverVarAnaResult) VAResult).Days.FreedomDegree,
                                     ((PLCrossOverVarAnaResult) VAResult).ResWith.FreedomDegree,
                                     ((PLCrossOverVarAnaResult) VAResult).Days.FValue);
            ((PLCrossOverVarAnaResult) VAResult).DaysPar.PValue =
                Distributions.Dist_F(((PLCrossOverVarAnaResult) VAResult).DaysPar.FreedomDegree,
                                     ((PLCrossOverVarAnaResult) VAResult).ResWith.FreedomDegree,
                                     ((PLCrossOverVarAnaResult) VAResult).DaysPar.FValue);
        }

        protected override void DoReliabilityCheck()
        {
            VariationList.Clear();
            VariationList.Add(ConstStrings.VariationSourcesDict[CalcInfo.Lang][VariationSources.Prep], VAResult.PrepValues.PValue);
            VariationList.Add(ConstStrings.VariationSourcesDict[CalcInfo.Lang][VariationSources.Reg], VAResult.RegValues.PValue);
            VariationList.Add(ConstStrings.VariationSourcesDict[CalcInfo.Lang][VariationSources.Par], VAResult.ParValues.PValue);
            VariationList.Add(ConstStrings.VariationSourcesDict[CalcInfo.Lang][VariationSources.DaysPrep], ((PLCrossOverVarAnaResult)VAResult).DaysPrep.PValue);
            VariationList.Add(ConstStrings.VariationSourcesDict[CalcInfo.Lang][VariationSources.DaysReg], ((PLCrossOverVarAnaResult)VAResult).DaysReg.PValue);
            VariationList.Add(ConstStrings.VariationSourcesDict[CalcInfo.Lang][VariationSources.DaysPar], ((PLCrossOverVarAnaResult)VAResult).DaysPar.PValue);
            ReliabilityChecker.DoCheck(VariationList, CalcInfo.Lang);
        }

        #endregion

        #region 重写可信限预计算内容

        protected override void Calcb()
        {
            PEResult.b = VAResult.HL*VAResult.L.Sum()/(PEResult.I*2*_dn*_dh);
        }

        protected override void CalcM()
        {
            PEResult.MList.Add((VAResult.SampleAverageSum[1] - VAResult.SampleAverageSum[0])/(_dd*PEResult.b));
        }

        protected override void CalcC()
        {
            double t = Distributions.Dist_t(((PLCrossOverVarAnaResult) VAResult).ResWith.FreedomDegree);
            PEResult.C = VAResult.RegValues.SquareSum/
                         (VAResult.RegValues.SquareSum - ((PLCrossOverVarAnaResult) VAResult).ResWith.MeanSquare*t*t);
        }

        protected override void CalcV()
        {
            PEResult.V = VAResult.RegValues.SquareSum/(PEResult.b*PEResult.b*_dd*2*_dn);
        }

        #endregion

        protected override void FillVATable()
        {
            FillVAValues(1, VAResult.ParValues);
            FillVAValues(2, ((PLCrossOverVarAnaResult)VAResult).DaysPrep);
            FillVAValues(3, ((PLCrossOverVarAnaResult)VAResult).DaysReg);
            FillVAValues(4, ((PLCrossOverVarAnaResult)VAResult).ResBetw, 3);

            FillVAValues(5, ((PLCrossOverVarAnaResult)VAResult).Rab);
            FillVAValues(6, VAResult.PrepValues);
            FillVAValues(7, VAResult.RegValues);
            FillVAValues(8, ((PLCrossOverVarAnaResult)VAResult).Days);
            FillVAValues(9, ((PLCrossOverVarAnaResult)VAResult).DaysPar);
            FillVAValues(10, ((PLCrossOverVarAnaResult)VAResult).ResWith, 3);
            FillVAValues(11, VAResult.TotalValues, 3);
        }

        public override void CreateTable()
        {
            VATable.InitTable(CalcInfo);
            for (int i = 0; i < _h - 1; i++)
            {
                PETableList.Add(new PotencyEstimateTable());
                PETableList[i].InitTable(_h, i + 1, CalcInfo);
            }
            FillVATable();
            FillPETable(_h);
            ConclusionTable.CreateTable(ReliabilityChecker.ConclusionList, ReliabilityChecker.FinalConclusion, CalcInfo.Lang);
        }
    }
}
