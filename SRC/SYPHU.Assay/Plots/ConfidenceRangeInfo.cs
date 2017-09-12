using System.Windows.Media;
using SYPHU.Assay.Results.PotencyEstimateResults;

namespace SYPHU.Assay.Plots
{
    public class ConfidenceRangeInfo
    {
        public BasicPEValues PEValues = new BasicPEValues();

        /// <summary>
        ///     画图类型
        /// </summary>
        public PlotTypes Type = PlotTypes.SolidLine;

        public Color Color = Colors.Blue;
    }
}