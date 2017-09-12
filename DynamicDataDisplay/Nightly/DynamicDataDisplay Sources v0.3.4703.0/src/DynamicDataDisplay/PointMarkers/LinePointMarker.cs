using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Microsoft.Research.DynamicDataDisplay.PointMarkers
{
    public class LinePointMarker : ShapeElementPointMarker
    {
        public override UIElement CreateMarker()
        {
            Line result = new Line();
            result.Width = Size;
            result.Height = 5;
            result.Stroke = Brush;
            result.Fill = Fill;
            if (!String.IsNullOrEmpty(ToolTipText))
            {
                ToolTip tt = new ToolTip();
                tt.Content = ToolTipText;
                result.ToolTip = tt;
            }
            return result;
        }

        public override void SetMarkerProperties(UIElement marker)
        {
            Line rect = (Line)marker;

            rect.Width = Size;
            rect.Height = Size;
            rect.Stroke = Brush;
            rect.Fill = Fill;

            rect.X1 = 0;
            rect.Y1 = 0;
            rect.X2 = Size;
            rect.Y2 = 0;
            if (!String.IsNullOrEmpty(ToolTipText))
            {
                ToolTip tt = new ToolTip();
                tt.Content = ToolTipText;
                rect.ToolTip = tt;
            }
        }

        public override void SetPosition(UIElement marker, Point screenPoint)
        {
            Canvas.SetLeft(marker, screenPoint.X - Size / 2);
            Canvas.SetTop(marker, screenPoint.Y);
            
        }
    }
}
