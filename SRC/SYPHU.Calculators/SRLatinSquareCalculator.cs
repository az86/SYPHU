using System.Collections.Generic;
using SYPHU.Assay.Results.VarianceAnalysisResults;
using SYPHU.Utilities;

namespace SYPHU.Calculators
{
    /// <summary>
    /// 斜率比法拉丁方
    /// </summary>
    public class SRLatinSquareCalculator : SlopeRatioMensurationCalculator
    {
        public SRLatinSquareCalculator()
        {
            VAResult = new SRLatinSquareVarAnaResult();
        }

        #region 私有计算

        protected override void CalcFreedom()
        {
            base.CalcFreedom();
            ((SRLatinSquareVarAnaResult)VAResult).RowValues.FreedomDegree =
                CalcInfo.DataSize.ReplicateNum - 1;

            ((SRLatinSquareVarAnaResult)VAResult).ColValues.FreedomDegree =
                CalcInfo.DataSize.ReplicateNum - 1;

            VAResult.ResValues.FreedomDegree = (CalcInfo.DataSize.PreparationNum * CalcInfo.DataSize.DoseNum -
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
                rowAve[i] /= CalcInfo.DataSize.PreparationNum * CalcInfo.DataSize.DoseNum;
            }
            ((SRLatinSquareVarAnaResult)VAResult).RowValues.SquareSum = CalcInfo.DataSize.PreparationNum *
                                                                         CalcInfo.DataSize.DoseNum *
                                                                         rowAve.SquareSum() - VAResult.K;

            var bTmp = new List<double>();
            foreach (List<double> t in BList)
            {
                bTmp.AddRange(t);
            }
            ((SRLatinSquareVarAnaResult)VAResult).ColValues.SquareSum = CalcInfo.DataSize.PreparationNum *
                                                                         CalcInfo.DataSize.DoseNum *
                                                                         bTmp.SquareSum() - VAResult.K;

            VAResult.ResValues.SquareSum = VAResult.TotalValues.SquareSum - VAResult.TreatValues.SquareSum -
                                           ((SRLatinSquareVarAnaResult)VAResult).RowValues.SquareSum -
                                           ((SRLatinSquareVarAnaResult)VAResult).ColValues.SquareSum;
        }

        protected override void CalcFValues()
        {
            base.CalcFValues();
            ((SRLatinSquareVarAnaResult)VAResult).RowValues.FValue =
                ((SRLatinSquareVarAnaResult)VAResult).RowValues.MeanSquare /
                VAResult.ResValues.MeanSquare;
            ((SRLatinSquareVarAnaResult)VAResult).ColValues.FValue =
                ((SRLatinSquareVarAnaResult)VAResult).ColValues.MeanSquare /
                VAResult.ResValues.MeanSquare;
        }

        protected override void CalcPValues()
        {
            base.CalcPValues();
            ((SRLatinSquareVarAnaResult)VAResult).RowValues.PValue =
                Distributions.Dist_F(((SRLatinSquareVarAnaResult)VAResult).RowValues.FreedomDegree,
                                     VAResult.ResValues.FreedomDegree,
                                     ((SRLatinSquareVarAnaResult)VAResult).RowValues.FValue);

            ((SRLatinSquareVarAnaResult)VAResult).ColValues.PValue =
                Distributions.Dist_F(((SRLatinSquareVarAnaResult)VAResult).ColValues.FreedomDegree,
                                     VAResult.ResValues.FreedomDegree,
                                     ((SRLatinSquareVarAnaResult)VAResult).ColValues.FValue);
        }

        #endregion

        #region 填充表格

        protected override void FillVATable()
        {
            FillVAValues(1, VAResult.RegValues);
            FillVAValues(2, VAResult.IntersValues);
            if (CalcInfo.DataSize.DoseNum <= 2)
            {
                FillVAValues(3, VAResult.TreatValues);
                FillVAValues(4, ((SRLatinSquareVarAnaResult)VAResult).RowValues);
                FillVAValues(5, ((SRLatinSquareVarAnaResult)VAResult).ColValues);
                FillVAValues(6, VAResult.ResValues, 3);
                FillVAValues(7, VAResult.TotalValues, 3);
            }
            else
            {
                FillVAValues(3, VAResult.LinValues);
                for (int i = 0; i < CalcInfo.DataSize.PreparationNum; i++)
                {
                    FillVAValues(4 + i, VAResult.LinsValues[i]);
                }
                FillVAValues(4 + CalcInfo.DataSize.PreparationNum, VAResult.TreatValues);
                FillVAValues(5 + CalcInfo.DataSize.PreparationNum, ((SRLatinSquareVarAnaResult)VAResult).RowValues);
                FillVAValues(6 + CalcInfo.DataSize.PreparationNum, ((SRLatinSquareVarAnaResult)VAResult).ColValues);
                FillVAValues(7 + CalcInfo.DataSize.PreparationNum, VAResult.ResValues, 3);
                FillVAValues(8 + CalcInfo.DataSize.PreparationNum, VAResult.TotalValues, 3);
            }
        }

        #endregion
    }
}
