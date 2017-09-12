using System;
using System.Collections.Generic;
using SYPHU.Assay.Results.VarianceAnalysisResults;
using SYPHU.Utilities;

namespace SYPHU.Calculators
{
    /// <summary>
    /// 平行线法拉丁方
    /// </summary>
    public class PLLatinSquareCalculator : PLCompletelyRandomisedCalculator
    {
        public PLLatinSquareCalculator()
        {
            VAResult = new PLLatinSquareVarAnaResult();
        }

        #region 私有计算

        protected override void CalcFreedom()
        {
            base.CalcFreedom();
            ((PLLatinSquareVarAnaResult) VAResult).RowValues.FreedomDegree =
                CalcInfo.DataSize.ReplicateNum - 1;

            ((PLLatinSquareVarAnaResult) VAResult).ColValues.FreedomDegree =
                CalcInfo.DataSize.ReplicateNum - 1;
            CalcResFreedom();
        }

        protected override void CalcResFreedom()
        {
            ((PLLatinSquareVarAnaResult)VAResult).ResValues.FreedomDegree = (CalcInfo.DataSize.PreparationNum * CalcInfo.DataSize.DoseNum -
                                    2) * (CalcInfo.DataSize.ReplicateNum - 1) - ExtremeAbnormalDataNum;
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
            ((PLLatinSquareVarAnaResult) VAResult).RowValues.SquareSum = CalcInfo.DataSize.PreparationNum*
                                                                         CalcInfo.DataSize.DoseNum*
                                                                         rowAve.SquareSum() - VAResult.K;

            //var colAve = new List<double>();
            //for (int i = 0; i < CalcInfo.DataSize.PreparationNum; i++)
            //{
            //    for (int j = 0; j < CalcInfo.DataSize.DoseNum; j++)
            //    {
            //        colAve.Add(AssData.Data[i][j].Average());
            //    }
            //}
            var bTmp = new List<double>();
            foreach (List<double> t in BList)
            {
                bTmp.AddRange(t);
            }
            ((PLLatinSquareVarAnaResult) VAResult).ColValues.SquareSum = CalcInfo.DataSize.PreparationNum*
                                                                         CalcInfo.DataSize.DoseNum*
                                                                         bTmp.SquareSum() - VAResult.K;
            CalcResSquareSum();
        }

        protected override void CalcResSquareSum()
        {
            ((PLLatinSquareVarAnaResult)VAResult).ResValues.SquareSum = VAResult.TotalValues.SquareSum -
                                                             ((PLLatinSquareVarAnaResult)VAResult)
                                                                 .TreatValues.SquareSum -
                                                             ((PLLatinSquareVarAnaResult)VAResult)
                                                                 .RowValues.SquareSum -
                                                             ((PLLatinSquareVarAnaResult)VAResult)
                                                                 .ColValues.SquareSum;
        }

        protected override void CalcFValues()
        {
            base.CalcFValues();
            ((PLLatinSquareVarAnaResult) VAResult).RowValues.FValue =
                ((PLLatinSquareVarAnaResult) VAResult).RowValues.MeanSquare/
                ((PLLatinSquareVarAnaResult)VAResult).ResValues.MeanSquare;
            ((PLLatinSquareVarAnaResult) VAResult).ColValues.FValue =
                ((PLLatinSquareVarAnaResult) VAResult).ColValues.MeanSquare/
                ((PLLatinSquareVarAnaResult)VAResult).ResValues.MeanSquare;
        }

        protected override void CalcPValues()
        {
            base.CalcPValues();
            ((PLLatinSquareVarAnaResult) VAResult).RowValues.PValue =
                Distributions.Dist_F(((PLLatinSquareVarAnaResult) VAResult).RowValues.FreedomDegree,
                                     ((PLLatinSquareVarAnaResult)VAResult).ResValues.FreedomDegree,
                                     ((PLLatinSquareVarAnaResult) VAResult).RowValues.FValue);

            ((PLLatinSquareVarAnaResult) VAResult).ColValues.PValue =
                Distributions.Dist_F(((PLLatinSquareVarAnaResult) VAResult).ColValues.FreedomDegree,
                                     ((PLLatinSquareVarAnaResult)VAResult).ResValues.FreedomDegree,
                                     ((PLLatinSquareVarAnaResult) VAResult).ColValues.FValue);
        }

        #endregion

        protected override void CalcSM(int i, double secItem)
        {
            PEResult.PEValues[i].SM = secItem /
                                      Distributions.Dist_t(
                                          ((PLLatinSquareVarAnaResult)VAResult).ResValues.FreedomDegree) / Math.Log(10);
        }

        #region 填充表格

        protected override void FillVATable()
        {
            FillVAValues(1, VAResult.PrepValues);
            FillVAValues(2, VAResult.RegValues);
            FillVAValues(3, VAResult.ParValues);
            if (CalcInfo.DataSize.DoseNum <= 2)
            {
                FillVAValues(4, ((PLLatinSquareVarAnaResult)VAResult).TreatValues);
                FillVAValues(5, ((PLLatinSquareVarAnaResult)VAResult).RowValues);
                FillVAValues(6, ((PLLatinSquareVarAnaResult)VAResult).ColValues);
                FillVAValues(7, ((PLLatinSquareVarAnaResult)VAResult).ResValues, 3);
                FillVAValues(8, VAResult.TotalValues, 3);
            }
            else
            {
                FillVAValues(4, ((PLLatinSquareVarAnaResult)VAResult).LinValues);
                for (int i = 0; i < CalcInfo.DataSize.PreparationNum; i++)
                {
                    FillVAValues(5 + i, ((PLCompletelyRandomisedVarAnaResult)VAResult).LinsValues[i]);
                }
                FillVAValues(5 + CalcInfo.DataSize.PreparationNum, ((PLLatinSquareVarAnaResult)VAResult).TreatValues);
                FillVAValues(6 + CalcInfo.DataSize.PreparationNum, ((PLLatinSquareVarAnaResult)VAResult).RowValues);
                FillVAValues(7 + CalcInfo.DataSize.PreparationNum, ((PLLatinSquareVarAnaResult)VAResult).ColValues);
                FillVAValues(8 + CalcInfo.DataSize.PreparationNum, ((PLLatinSquareVarAnaResult)VAResult).ResValues, 3);
                FillVAValues(9 + CalcInfo.DataSize.PreparationNum, VAResult.TotalValues, 3);
            }
            
        }

        #endregion
    }
}
