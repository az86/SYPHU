using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Microsoft.Research.DynamicDataDisplay.PointMarkers
{
    public class RetangleElementMarker : ShapeElementPointMarker
    {
        public Brush BorderBrush;
 
        public override UIElement CreateMarker()
        {
            var border = new Border { Height = Size, Width = Size, BorderThickness = new Thickness(2), BorderBrush = BorderBrush };
            if (!String.IsNullOrEmpty(ToolTipText))
            {
                var textblock = new TextBlock {Text = ToolTipText, Background = Fill};
                border.Child = textblock;
            }
            return border;
        }

        public override void SetPosition(UIElement marker, Point screenPoint)
        {
            Canvas.SetLeft(marker, screenPoint.X - Size / 2);
            Canvas.SetTop(marker, screenPoint.Y - Size / 2);
        }
    }
}
