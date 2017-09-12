using System.Linq;
using SYPHU.Assay.Results.SigmoidCurveIterResult;

namespace SYPHU.Calculators
{
    /// <summary>
    /// S型曲线定量反应
    /// </summary>
    public class SCGradedCalculator : SigmoidCurveMensurationCalculator
    {
        public SCGradedCalculator()
        {
            IterResult = new SCGradedIterResult();
        }

        /// <summary>
        /// 合并u值
        /// </summary>
        protected override void MergeData()
        {
            IterResult.DoseList.Clear();
            ((SCGradedIterResult)IterResult).uList.Clear();
            for (int i = 0; i < CalcInfo.DataSize.PreparationNum; i++)
            {
                //IterResult.DoseList.AddRange(CalcInfo.d[i]);
                for (int j = 0; j < CalcInfo.DataSize.DoseNum; j++)
                {
                    IterResult.DoseList.Add(CalcInfo.Unitd[i][j].Val);
                    ((SCGradedIterResult)IterResult).uList.Add(AssData.Data[i][j].Average());
                }
            }
            IterResult.DataLen = CalcInfo.DataSize.PreparationNum * CalcInfo.DataSize.DoseNum;
        }
    }
}
