using System;
using System.Collections.Generic;
using SYPHU.Assay.Results.VarianceAnalysisResults;
using SYPHU.Utilities;

namespace SYPHU.Calculators
{
    /// <summary>
    /// 平行线法随机区组
    /// </summary>
    public class PLRandomisedBlockCalculator : PLCompletelyRandomisedCalculator
    {
        public PLRandomisedBlockCalculator()
        {
            VAResult = new PLRandomisedBlockVarAnaResult();
        }

        #region 私有计算

        protected override void CalcFreedom()
        {
            base.CalcFreedom();
            ((PLRandomisedBlockVarAnaResult) VAResult).RowValues.FreedomDegree =
                CalcInfo.DataSize.ReplicateNum - 1;
        }

        protected override void CalcResFreedom()
        {
            ((PLRandomisedBlockVarAnaResult)VAResult).ResValues.FreedomDegree = (CalcInfo.DataSize.PreparationNum * CalcInfo.DataSize.DoseNum - 1) * (CalcInfo.DataSize.ReplicateNum - 1) - ExtremeAbnormalDataNum;
        }

        protected override void CalcSquareSum()
        {
            base.CalcSquareSum();
            var rowAve = new List<double>();
            for (int i = 0; i < CalcInfo.DataSize.ReplicateNum; i++)
            {
                rowAve.Add(0.0);
                for (int j = 0; j < CalcInfo.DataSize.PreparationNum; j++)
                {
                    for (int k = 0; k < CalcInfo.DataSize.DoseNum; k++)
                    {
                        rowAve[i] += AssData.Data[j][k][i];
                    }
                }
                rowAve[i] /= CalcInfo.DataSize.PreparationNum*CalcInfo.DataSize.DoseNum;
            }
            ((PLRandomisedBlockVarAnaResult) VAResult).RowValues.SquareSum = CalcInfo.DataSize.PreparationNum*
                                                                             CalcInfo.DataSize.DoseNum*
                                                                             rowAve.SquareSum() - VAResult.K;
            CalcResSquareSum();
        }

        protected override void CalcResSquareSum()
        {
            ((PLRandomisedBlockVarAnaResult)VAResult).ResValues.SquareSum = VAResult.TotalValues.SquareSum - ((PLRandomisedBlockVarAnaResult)VAResult).TreatValues.SquareSum -
                               ((PLRandomisedBlockVarAnaResult)VAResult).RowValues.SquareSum;
        }

        protected override void CalcFValues()
        {
            base.CalcFValues();
            ((PLRandomisedBlockVarAnaResult) VAResult).RowValues.FValue =
                ((PLRandomisedBlockVarAnaResult) VAResult).RowValues.MeanSquare/
                ((PLRandomisedBlockVarAnaResult)VAResult).ResValues.MeanSquare;
        }

        protected override void CalcPValues()
        {
            base.CalcPValues();
            ((PLRandomisedBlockVarAnaResult) VAResult).RowValues.PValue =
                Distributions.Dist_F(((PLRandomisedBlockVarAnaResult) VAResult).RowValues.FreedomDegree,
                                     ((PLRandomisedBlockVarAnaResult)VAResult).ResValues.FreedomDegree,
                                     ((PLRandomisedBlockVarAnaResult) VAResult).RowValues.FValue);
        }

        #endregion

        protected override void CalcSM(int i, double secItem)
        {
            PEResult.PEValues[i].SM = secItem /
                                      Distributions.Dist_t(
                                          ((PLRandomisedBlockVarAnaResult)VAResult).ResValues.FreedomDegree) / Math.Log(10);
        }

        #region 填充表格

        protected override void FillVATable()
        {
            FillVAValues(1, VAResult.PrepValues);
            FillVAValues(2, VAResult.RegValues);
            FillVAValues(3, VAResult.ParValues);
            if (CalcInfo.DataSize.DoseNum <= 2)
            {
                FillVAValues(4, ((PLRandomisedBlockVarAnaResult)VAResult).TreatValues);
                FillVAValues(5, ((PLRandomisedBlockVarAnaResult)VAResult).RowValues);
                FillVAValues(6, ((PLRandomisedBlockVarAnaResult)VAResult).ResValues, 3);
                FillVAValues(7, VAResult.TotalValues, 3);
            }
            else
            {
                FillVAValues(4, ((PLRandomisedBlockVarAnaResult)VAResult).LinValues);
                for (int i = 0; i < CalcInfo.DataSize.PreparationNum; i++)
                {
                    FillVAValues(5 + i, ((PLCompletelyRandomisedVarAnaResult)VAResult).LinsValues[i]);
                }
                FillVAValues(5 + CalcInfo.DataSize.PreparationNum, ((PLRandomisedBlockVarAnaResult)VAResult).TreatValues);
                FillVAValues(6 + CalcInfo.DataSize.PreparationNum, ((PLRandomisedBlockVarAnaResult)VAResult).RowValues);
                FillVAValues(7 + CalcInfo.DataSize.PreparationNum, ((PLRandomisedBlockVarAnaResult)VAResult).ResValues, 3);
                FillVAValues(8 + CalcInfo.DataSize.PreparationNum, VAResult.TotalValues, 3);
            }
            
        }

        #endregion
    }
}