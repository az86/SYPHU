using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Microsoft.Research.DynamicDataDisplay.PointMarkers
{
    /// <summary>
    /// 短横线标识
    /// </summary>
    public class LinePointMarker : ShapeElementPointMarker
    {
        public override UIElement CreateMarker()
        {
            var line = new Line {Stroke = Brush, Fill = Fill, X2 = Size};
            if (!String.IsNullOrEmpty(ToolTipText))
            {
                var tt = new ToolTip {Content = ToolTipText};
                line.ToolTip = tt;
            }
            return line;
        }

        public override void SetPosition(UIElement marker, Point screenPoint)
        {
            Canvas.SetLeft(marker, screenPoint.X - Size / 2);
            Canvas.SetTop(marker, screenPoint.Y);
        }
    }
}
