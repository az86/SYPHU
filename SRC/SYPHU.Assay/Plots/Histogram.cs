using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using SYPHU.Utilities;

namespace SYPHU.Assay.Plots
{
     public class Histogram
     {
         public const double BoxPixelSize = 23;

         /// <summary>
         /// x区间列表
         /// </summary>
         public List<double> XLabelList;

         /// <summary>
         /// 频数标号分布列表
         /// </summary>
         public List<List<String>> FrequencyListList;

         /// <summary>
         /// 画图坐标
         /// </summary>
         public List<List<Point>> PlotCoordinateListList;

         /// <summary>
         /// 分组数
         /// </summary>
         public int GroupNum { get; set; }

         /// <summary>
         /// 颜色
         /// </summary>
         public Color Color = Colors.Blue;

         /// <summary>
         /// 画线类型
         /// </summary>
         public PlotTypes PlotType = PlotTypes.SolidLine;

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
         public double YRange { get; set; }

         /// <summary>
         /// X轴像素大小
         /// </summary>
         public double XLengthInPixel
         {
             get
             {
                 return GroupNum * BoxPixelSize > _dXLengthInPixel ? GroupNum * BoxPixelSize : _dXLengthInPixel;
             }
             private set { _dXLengthInPixel = value; }
         }

         private double _dXLengthInPixel = 300;

         /// <summary>
         /// Y轴像素大小
         /// </summary>
         public double YLengthInPixel
         {
             get
             {
                 int maxLength = VectorCalcMethodExt.MaxSubLength(FrequencyListList);
                 return maxLength * BoxPixelSize > _dYLengthInPixel ? maxLength * BoxPixelSize : _dYLengthInPixel;
             }
             private set { _dYLengthInPixel = value; }
         }

         private double _dYLengthInPixel = 400;

         public void SetValues(String header, String xLabel, String yLabel)
         {
             Header = header;
             XLabel = xLabel;
             YLabel = yLabel;
         }
     }
}
