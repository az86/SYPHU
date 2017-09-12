using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace SYPHU.Assay.Plots
{
    /// <summary>
    /// 点集
    /// </summary>
    public class PointGroup
    {
        //散点
        public List<List<Point>> Points = new List<List<Point>>();

        //颜色
        public Color Color = Colors.Blue;
    }
}