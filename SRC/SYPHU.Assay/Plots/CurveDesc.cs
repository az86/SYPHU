using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace SYPHU.Assay.Plots
{
    /// <summary>
    /// 曲线描述类
    /// </summary>
    public class CurveDesc
    {
        /// <summary>
        /// 曲线公式
        /// </summary>
        public String CurveEquation { get; set; }

        /// <summary>
        /// 拟合曲线点（连续散点）
        /// </summary>
        public List<Point> FitPoints
        {
            get { return _fitPoints; }
            set { _fitPoints = value; }
        }

        private List<Point> _fitPoints = new List<Point>();

        /// <summary>
        /// 曲线颜色，默认蓝色
        /// </summary>
        public Color Color = Colors.Blue;

        /// <summary>
        /// 曲线散点最大个数
        /// </summary>
        public int MaxPointNum = 100;

        /// <summary>
        /// 画线类型
        /// </summary>
        public PlotTypes PlotType = PlotTypes.SolidLine;
    }
}
