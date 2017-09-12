namespace SYPHU.Assay.Results.PotencyEstimateResults
{
    /// <summary>
    /// 每个试验组的效价值
    /// </summary>
    public class TreatPEValues
    {
        public BasicPEValues Potency = new BasicPEValues();
        public BasicPEValues RelToAss = new BasicPEValues();
        public BasicPEValues RelToEst = new BasicPEValues();
        public double CLPercent
        {
            //get { return (RelToEst.Upper - RelToEst.Lower)*0.5; }
            get { return (Potency.Upper - Potency.Lower) * 0.5 / Potency.Est; }
        }

        public double SM;
    }
}