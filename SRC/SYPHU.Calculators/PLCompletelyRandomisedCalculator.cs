using System;
using System.Globalization;
using System.Linq;
using SYPHU.Assay.Results.ReliabilityCheck;
using SYPHU.Assay.Results.VarianceAnalysisResults;
using SYPHU.Assay.Tables;
using SYPHU.Utilities;

namespace SYPHU.Calculators
{
    /// <summary>
    /// 平行线法完全随机
    /// </summary>
    public class PLCompletelyRandomisedCalculator : ParallelLineMensurationCalculator
    {
        public PLCompletelyRandomisedCalculator()
        {
            VAResult = new PLCompletelyRandomisedVarAnaResult();
        }

        #region 私有计算

        protected override void CalcFreedom()
        {
            base.CalcFreedom();
            if (CalcInfo.DataSize.DoseNum > 2)
            {
                ((PLCompletelyRandomisedVarAnaResult)VAResult).LinValues.FreedomDegree = CalcInfo.DataSize.PreparationNum *
                                       (CalcInfo.DataSize.DoseNum - 2);
                for (int i = 0; i < CalcInfo.DataSize.PreparationNum; i++)
                {
                    ((PLCompletelyRandomisedVarAnaResult)VAResult).LinsValues.Add(new BasicVarianceAnalysisValues());
                    ((PLCompletelyRandomisedVarAnaResult)VAResult).LinsValues[i].FreedomDegree = CalcInfo.DataSize.DoseNum - 2;
                }
            }

            ((PLCompletelyRandomisedVarAnaResult) VAResult).TreatValues.FreedomDegree =
                CalcInfo.DataSize.PreparationNum*CalcInfo.DataSize.DoseNum - 1;
            CalcResFreedom();
        }

        protected virtual void CalcResFreedom()
        {
            ((PLCompletelyRandomisedVarAnaResult)VAResult).ResValues.FreedomDegree = CalcInfo.DataSize.PreparationNum *
                                                                          CalcInfo.DataSize.DoseNum *
                                                                          (CalcInfo.DataSize.ReplicateNum -
                                                                           1) - ExtremeAbnormalDataNum;
        }

        protected override void CalcSquareSum()
        {
            base.CalcSquareSum();
            double treat = VAResult.SampleAverage.Sum(t => t.SquareSum());
            ((PLCompletelyRandomisedVarAnaResult)VAResult).TreatValues.SquareSum = CalcInfo.DataSize.ReplicateNum * treat -
                                                                                    VAResult.K;


            if (CalcInfo.DataSize.DoseNum > 2)
            {
                ((PLCompletelyRandomisedVarAnaResult) VAResult).LinValues.SquareSum =
                    ((PLCompletelyRandomisedVarAnaResult) VAResult).TreatValues.SquareSum -
                    VAResult.PrepValues.SquareSum -
                    VAResult.RegValues.SquareSum - VAResult.ParValues.SquareSum;
                for (int i = 0; i < CalcInfo.DataSize.PreparationNum; i++)
                {
                    ((PLCompletelyRandomisedVarAnaResult)VAResult).LinsValues[i].SquareSum = VAResult.SStreatList[i] - VAResult.SSregList[i];
                }
            }

            CalcResSquareSum();
        }

        protected virtual void CalcResSquareSum()
        {
            ((PLCompletelyRandomisedVarAnaResult)VAResult).ResValues.SquareSum = VAResult.TotalValues.SquareSum -
                                                          ((PLCompletelyRandomisedVarAnaResult)
                                                           VAResult).TreatValues.SquareSum;
        }

        protected override void CalcFValues()
        {
            VAResult.PrepValues.FValue = VAResult.PrepValues.MeanSquare/
                                         ((PLCompletelyRandomisedVarAnaResult) VAResult).ResValues.MeanSquare;
            VAResult.RegValues.FValue = VAResult.RegValues.MeanSquare/
                                        ((PLCompletelyRandomisedVarAnaResult) VAResult).ResValues.MeanSquare;
            VAResult.ParValues.FValue = VAResult.ParValues.MeanSquare/
                                        ((PLCompletelyRandomisedVarAnaResult) VAResult).ResValues.MeanSquare;

            if (CalcInfo.DataSize.DoseNum > 2)
            {
                ((PLCompletelyRandomisedVarAnaResult)VAResult).LinValues.FValue =
                    ((PLCompletelyRandomisedVarAnaResult)VAResult).LinValues.MeanSquare /
                    ((PLCompletelyRandomisedVarAnaResult)VAResult).ResValues.MeanSquare;

                for (int i = 0; i < CalcInfo.DataSize.PreparationNum; i++)
                {
                    ((PLCompletelyRandomisedVarAnaResult)VAResult).LinsValues[i].FValue =
                    ((PLCompletelyRandomisedVarAnaResult)VAResult).LinsValues[i].MeanSquare /
                    ((PLCompletelyRandomisedVarAnaResult)VAResult).ResValues.MeanSquare;
                }
            }


            ((PLCompletelyRandomisedVarAnaResult) VAResult).TreatValues.FValue =
                ((PLCompletelyRandomisedVarAnaResult) VAResult).TreatValues.MeanSquare/
                ((PLCompletelyRandomisedVarAnaResult) VAResult).ResValues.MeanSquare;
        }

        protected override void CalcPValues()
        {
            VAResult.PrepValues.PValue = Distributions.Dist_F(VAResult.PrepValues.FreedomDegree,
                                                              ((PLCompletelyRandomisedVarAnaResult) VAResult).ResValues.FreedomDegree,
                                                              VAResult.PrepValues.FValue);
            VAResult.RegValues.PValue = Distributions.Dist_F(VAResult.RegValues.FreedomDegree,
                                                             ((PLCompletelyRandomisedVarAnaResult) VAResult).ResValues.FreedomDegree,
                                                             VAResult.RegValues.FValue);
            VAResult.ParValues.PValue = Distributions.Dist_F(VAResult.ParValues.FreedomDegree,
                                                             ((PLCompletelyRandomisedVarAnaResult) VAResult).ResValues.FreedomDegree,
                                                             VAResult.ParValues.FValue);

            if (CalcInfo.DataSize.DoseNum > 2)
            {
                ((PLCompletelyRandomisedVarAnaResult) VAResult).LinValues.PValue =
                    Distributions.Dist_F(((PLCompletelyRandomisedVarAnaResult) VAResult).LinValues.FreedomDegree,
                                         ((PLCompletelyRandomisedVarAnaResult) VAResult).ResValues.FreedomDegree,
                                         ((PLCompletelyRandomisedVarAnaResult) VAResult).LinValues.FValue);
                for (int i = 0; i < CalcInfo.DataSize.PreparationNum; i++)
                {
                    ((PLCompletelyRandomisedVarAnaResult)VAResult).LinsValues[i].PValue =
                        Distributions.Dist_F(
                            ((PLCompletelyRandomisedVarAnaResult)VAResult).LinsValues[i].FreedomDegree,
                            ((PLCompletelyRandomisedVarAnaResult)VAResult).ResValues.FreedomDegree,
                            ((PLCompletelyRandomisedVarAnaResult)VAResult).LinsValues[i].FValue);
                }
            }


            ((PLCompletelyRandomisedVarAnaResult) VAResult).TreatValues.PValue =
                Distributions.Dist_F(((PLCompletelyRandomisedVarAnaResult) VAResult).TreatValues.FreedomDegree,
                                     ((PLCompletelyRandomisedVarAnaResult) VAResult).ResValues.FreedomDegree,
                                     ((PLCompletelyRandomisedVarAnaResult) VAResult).TreatValues.FValue);
        }

        protected override void DoReliabilityCheck()
        {
            VariationList.Clear();
            VariationList.Add(ConstStrings.VariationSourcesDict[CalcInfo.Lang][VariationSources.Prep], VAResult.PrepValues.PValue);
            VariationList.Add(ConstStrings.VariationSourcesDict[CalcInfo.Lang][VariationSources.Reg], VAResult.RegValues.PValue);
            VariationList.Add(ConstStrings.VariationSourcesDict[CalcInfo.Lang][VariationSources.Par], VAResult.ParValues.PValue);
            if (CalcInfo.DataSize.DoseNum > 2)
            {
                VariationList.Add(ConstStrings.VariationSourcesDict[CalcInfo.Lang][VariationSources.Lin], ((PLCompletelyRandomisedVarAnaResult)VAResult).LinValues.PValue);
                VariationList.Add(ConstStrings.VariationSourcesDict[CalcInfo.Lang][VariationSources.LinS], ((PLCompletelyRandomisedVarAnaResult)VAResult).LinsValues[0].PValue);
                for (int i = 1; i < CalcInfo.DataSize.PreparationNum; i++)
                {
                    VariationList.Add(ConstStrings.VariationSourcesDict[CalcInfo.Lang][VariationSources.LinT] + i.ToString(CultureInfo.InvariantCulture), ((PLCompletelyRandomisedVarAnaResult)VAResult).LinsValues[i].PValue);
                }
            }
            
            ReliabilityChecker.DoCheck(VariationList, CalcInfo.Lang);
        }

        #endregion

        protected override void CalcC()
        {
            PEResult.C = VAResult.RegValues.SquareSum /
                         (VAResult.RegValues.SquareSum -
                          ((PLCompletelyRandomisedVarAnaResult)VAResult).ResValues.MeanSquare *
                          Distributions.Dist_t(((PLCompletelyRandomisedVarAnaResult)VAResult).ResValues.FreedomDegree) *
                          Distributions.Dist_t(((PLCompletelyRandomisedVarAnaResult)VAResult).ResValues.FreedomDegree));
        }

        protected override void CalcSM(int i, double secItem)
        {
            PEResult.PEValues[i].SM = secItem/
                                      Distributions.Dist_t(
                                          ((PLCompletelyRandomisedVarAnaResult)VAResult).ResValues.FreedomDegree) / Math.Log(10);
        }

        #region 填充表格

        protected override void FillVATable()
        {
            FillVAValues(1, VAResult.PrepValues);
            FillVAValues(2, VAResult.RegValues);
            FillVAValues(3, VAResult.ParValues);
            if (CalcInfo.DataSize.DoseNum <= 2)
            {
                FillVAValues(4, ((PLCompletelyRandomisedVarAnaResult)VAResult).TreatValues);
                FillVAValues(5, ((PLCompletelyRandomisedVarAnaResult)VAResult).ResValues, 3);
                FillVAValues(6, VAResult.TotalValues, 3);
            }
            else
            {
                FillVAValues(4, ((PLCompletelyRandomisedVarAnaResult)VAResult).LinValues);
                for (int i = 0; i < CalcInfo.DataSize.PreparationNum; i++)
                {
                    FillVAValues(5 + i, ((PLCompletelyRandomisedVarAnaResult)VAResult).LinsValues[i]);
                }
                FillVAValues(5 + CalcInfo.DataSize.PreparationNum, ((PLCompletelyRandomisedVarAnaResult)VAResult).TreatValues);
                FillVAValues(6 + CalcInfo.DataSize.PreparationNum, ((PLCompletelyRandomisedVarAnaResult)VAResult).ResValues, 3);
                FillVAValues(7 + CalcInfo.DataSize.PreparationNum, VAResult.TotalValues, 3);
            }
            
        }

        #endregion
    }
}