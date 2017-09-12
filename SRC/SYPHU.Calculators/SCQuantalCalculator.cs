using SYPHU.Assay.Results.SigmoidCurveIterResult;

namespace SYPHU.Calculators
{
    /// <summary>
    /// S型曲线定性反应
    /// </summary>
    public class SCQuantalCalculator : SigmoidCurveMensurationCalculator
    {
        public SCQuantalCalculator()
        {
            IterResult = new SCQuantalIterResult();
        }

        /// <summary>
        /// 合并n、r值
        /// </summary>
        protected override void MergeData()
        {
            IterResult.DoseList.Clear();
            ((SCQuantalIterResult)IterResult).nList.Clear();
            ((SCQuantalIterResult)IterResult).rList.Clear();
            for (int i = 0; i < CalcInfo.DataSize.PreparationNum; i++)
            {
                //IterResult.DoseList.AddRange(CalcInfo.d[i]);
                for (int j = 0; j < CalcInfo.DataSize.DoseNum; j++)
                {
                    IterResult.DoseList.Add(CalcInfo.Unitd[i][j].Val);
                    ((SCQuantalIterResult)IterResult).nList.Add(AssData.Data[i][j][0]);
                    ((SCQuantalIterResult)IterResult).rList.Add(AssData.Data[i][j][1]);
                }
            }
            IterResult.DataLen = CalcInfo.DataSize.PreparationNum * CalcInfo.DataSize.DoseNum;
        }
    }
}
