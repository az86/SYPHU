using System;
using System.Collections.Generic;

namespace SYPHU.Assay.Plots
{
    public class MergeCalcPlotInfo
    {
        /// <summary>
        /// 置信区间列表
        /// </summary>
        public List<ConfidenceRangeInfo> ConfidenceRangeInfos;

        /// <summary>
        /// 上下界虚线
        /// </summary>
        public List<CurveDesc> LimitsLines;

        /// <summary>
        /// 图像题头
        /// </summary>
        public String Header { get; private set; }

        /// <summary>
        /// X坐标label
        /// </summary>
        public String XLabel { get; private set; }

        /// <summary>
        /// Y坐标label
        /// </summary>
        public String YLabel { get; private set; }

        /// <summary>
        /// X范围
        /// </summary>
        public double XRange { get; private set; }

        /// <summary>
        /// Y范围
        /// </summary>
        public double YRange { get; private set; }

        /// <summary>
        /// X轴像素大小
        /// </summary>
        public double XLengthInPixel
        {
            get { return _dXLengthInPixel; }
            private set { _dXLengthInPixel = value; }
        }

        private double _dXLengthInPixel = 300;

        /// <summary>
        /// Y轴像素大小
        /// </summary>
        public double YLengthInPixel
        {
            get { return _dYLengthInPixel; }
            private set { _dYLengthInPixel = value; }
        }

        private double _dYLengthInPixel = 400;

        public void SetValues(String header, String xLabel, String yLabel, double xRange, double yRange, double dXLengthInPixel = 300, double dYLengthInPixel = 400)
        {
            Header = header;
            XLabel = xLabel;
            YLabel = yLabel;
            XRange = xRange;
            YRange = yRange;
            XLengthInPixel = dXLengthInPixel;
            YLengthInPixel = dYLengthInPixel;
        }
    }
}
