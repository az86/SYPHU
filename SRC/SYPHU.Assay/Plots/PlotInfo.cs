using System;
using System.Collections.Generic;

namespace SYPHU.Assay.Plots
{
    /// <summary>
    /// 画图信息
    /// </summary>
    public class PlotInfo
    {
        /// <summary>
        /// 数据点（拟合前，散点）
        /// </summary>
        public List<PointGroup> DataPoints
        {
            get { return _dataPoints; }
            set { _dataPoints = value; }
        }

        private List<PointGroup> _dataPoints = new List<PointGroup>();

        /// <summary>
        /// 拟合的曲线
        /// </summary>
        public List<CurveDesc> CurveDescs;

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
        public double XRange { get; set; }

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