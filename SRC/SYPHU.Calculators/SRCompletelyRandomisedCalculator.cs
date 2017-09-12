using SYPHU.Assay.Results.PotencyEstimateResults;
using SYPHU.Assay.Results.VarianceAnalysisResults;
using SYPHU.Utilities;

namespace SYPHU.Calculators
{
    /// <summary>
    /// 斜率比法完全随机
    /// </summary>
    public class SRCompletelyRandomisedCalculator : SlopeRatioMensurationCalculator
    {
        public SRCompletelyRandomisedCalculator()
        {
            VAResult = new SRCompletelyRandomisedVarAnaResult();
        }

        #region 私有计算

        protected override void CalcFreedom()
        {
            base.CalcFreedom();
            VAResult.ResValues.FreedomDegree = CalcInfo.DataSize.PreparationNum * CalcInfo.DataSize.DoseNum *
                                       (CalcInfo.DataSize.ReplicateNum - 1) - ExtremeAbnormalDataNum;
        }

        protected override void CalcSquareSum()
        {
            base.CalcSquareSum();
            VAResult.ResValues.SquareSum = VAResult.TotalValues.SquareSum - VAResult.TreatValues.SquareSum;
        }

        #endregion

        protected override void FillVATable()
        {
            FillVAValues(1, VAResult.RegValues);
            FillVAValues(2, VAResult.IntersValues);
            if (CalcInfo.DataSize.DoseNum <= 2)
            {
                FillVAValues(3, VAResult.TreatValues);
                FillVAValues(4, VAResult.ResValues, 3);
                FillVAValues(5, VAResult.TotalValues, 3);
            }
            else
            {
                FillVAValues(3, VAResult.LinValues);
                for (int i = 0; i < CalcInfo.DataSize.PreparationNum; i++)
                {
                    FillVAValues(4 + i, VAResult.LinsValues[i]);
                }
                FillVAValues(4 + CalcInfo.DataSize.PreparationNum, VAResult.TreatValues);
                FillVAValues(5 + CalcInfo.DataSize.PreparationNum, VAResult.ResValues, 3);
                FillVAValues(6 + CalcInfo.DataSize.PreparationNum, VAResult.TotalValues, 3);
            }
        }
    }
}