using System.Collections.Generic;
using SYPHU.Assay.Results.VarianceAnalysisResults;
using SYPHU.Utilities;

namespace SYPHU.Calculators
{
    /// <summary>
    /// 斜率比法随机区组
    /// </summary>
    public class SRRandomisedBlockCalculator : SlopeRatioMensurationCalculator
    {
        public SRRandomisedBlockCalculator()
        {
            VAResult = new SRRandomisedBlockVarAnaResult();
        }

        #region 私有计算

        protected override void CalcFreedom()
        {
            base.CalcFreedom();
            ((SRRandomisedBlockVarAnaResult)VAResult).RowValues.FreedomDegree = CalcInfo.DataSize.ReplicateNum - 1;
            VAResult.ResValues.FreedomDegree = (CalcInfo.DataSize.PreparationNum * CalcInfo.DataSize.DoseNum -
                                                1) * (CalcInfo.DataSize.ReplicateNum - 1) - ExtremeAbnormalDataNum;
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
            ((SRRandomisedBlockVarAnaResult)VAResult).RowValues.SquareSum = CalcInfo.DataSize.PreparationNum *
                                                                             CalcInfo.DataSize.DoseNum *
                                                                             rowAve.SquareSum() - VAResult.K;

            VAResult.ResValues.SquareSum = VAResult.TotalValues.SquareSum - VAResult.TreatValues.SquareSum -
                                           ((SRRandomisedBlockVarAnaResult)VAResult).RowValues.SquareSum;
        }

        protected override void CalcFValues()
        {
            base.CalcFValues();
            ((SRRandomisedBlockVarAnaResult)VAResult).RowValues.FValue =
                ((SRRandomisedBlockVarAnaResult)VAResult).RowValues.MeanSquare /
                VAResult.ResValues.MeanSquare;
        }

        protected override void CalcPValues()
        {
            base.CalcPValues();
            ((SRRandomisedBlockVarAnaResult)VAResult).RowValues.PValue =
                Distributions.Dist_F(((SRRandomisedBlockVarAnaResult)VAResult).RowValues.FreedomDegree,
                                     VAResult.ResValues.FreedomDegree,
                                     ((SRRandomisedBlockVarAnaResult)VAResult).RowValues.FValue);
        }

        #endregion

        protected override void FillVATable()
        {
            FillVAValues(1, VAResult.RegValues);
            FillVAValues(2, VAResult.IntersValues);
            if (CalcInfo.DataSize.DoseNum <= 2)
            {
                FillVAValues(3, VAResult.TreatValues);
                FillVAValues(4, ((SRRandomisedBlockVarAnaResult) VAResult).RowValues);
                FillVAValues(5, VAResult.ResValues, 3);
                FillVAValues(6, VAResult.TotalValues, 3);
            }
            else
            {
                FillVAValues(3, VAResult.LinValues);
                for (int i = 0; i < CalcInfo.DataSize.PreparationNum; i++)
                {
                    FillVAValues(4 + i, VAResult.LinsValues[i]);
                }
                FillVAValues(4 + CalcInfo.DataSize.PreparationNum, VAResult.TreatValues);
                FillVAValues(5 + CalcInfo.DataSize.PreparationNum, ((SRRandomisedBlockVarAnaResult)VAResult).RowValues);
                FillVAValues(6 + CalcInfo.DataSize.PreparationNum, VAResult.ResValues, 3);
                FillVAValues(7 + CalcInfo.DataSize.PreparationNum, VAResult.TotalValues, 3);
            }
        }
    }
}
