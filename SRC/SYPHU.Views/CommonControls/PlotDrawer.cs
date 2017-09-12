using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay.PointMarkers;
using SYPHU.Assay.Plots;
using System.Windows.Media;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SYPHU.Views.CommonControls
{
    class PlotDrawer
    {
        public readonly ChartPlotter Plotter;

        private readonly UIElement _parentPanel;

        public PlotDrawer(PlotInfo plotInfo, UIElement parentPanel)
        {
            Plotter = CreateChartPlotter(plotInfo);
            _parentPanel = parentPanel;
        }

        public PlotDrawer(MergeCalcPlotInfo plotInfo, UIElement parentPanel)
        {
            var plotInfoTmp = new PlotInfo();
            plotInfoTmp.SetValues(plotInfo.Header, plotInfo.XLabel, plotInfo.YLabel, plotInfo.XRange, plotInfo.YRange, plotInfo.XLengthInPixel, plotInfo.YLengthInPixel);
            Plotter = CreateChartPlotter(plotInfoTmp);

            _parentPanel = parentPanel;
        }

        public PlotDrawer(Histogram hist, UIElement parentPanel)
        {
            var plotInfoTmp = new PlotInfo();
            plotInfoTmp.SetValues(hist.Header, hist.XLabel, hist.YLabel, hist.XRange, hist.YRange, hist.XLengthInPixel, hist.YLengthInPixel);
            Plotter = CreateChartPlotter(plotInfoTmp);

            _parentPanel = parentPanel;
        }

        private static ChartPlotter CreateChartPlotter(PlotInfo plotInfo)
        {
            var plotter = new ChartPlotter { Width = plotInfo.XLengthInPixel, Height = plotInfo.YLengthInPixel };

            var header =  new TextBlock
            {
                Text = plotInfo.Header,
                HorizontalAlignment = HorizontalAlignment.Center,
                FontSize = 15.5,
                Height = 23
            };

            plotter.HeaderPanel.Children.Add(header);

            var verticalTitle = new VerticalAxisTitle { Content = new TextBlock { Text = plotInfo.YLabel } };
            plotter.Children.Add(verticalTitle);

            var horizoltalTitle = new HorizontalAxisTitle { Content = new TextBlock { Text = plotInfo.XLabel } };
            plotter.Children.Add(horizoltalTitle);

            return plotter;
        }

        public void DisplayCurve(CurveDesc curve)
        {
            var data = new ObservableDataSource<Point>();
            if (curve.PlotType == PlotTypes.DashedLine)
            {
                var pen = new Pen
                {
                    DashStyle = DashStyles.DashDotDot,
                    Thickness = 1.5,
                    Brush = new SolidColorBrush(curve.Color)
                };
                var penDesc = new PenDescription();
                penDesc.LegendItem.Visibility = Visibility.Collapsed;
                Plotter.AddLineGraph(data, pen, penDesc);
            }
            else
            {
                if (string.IsNullOrEmpty(curve.CurveEquation))
                {
                    Plotter.AddLineGraph(data, curve.Color, 1);
                }
                else
                {
                    Plotter.AddLineGraph(data, curve.Color, 1, curve.CurveEquation);
                }
            }
            Plotter.Legend.LegendLeft = 0;
            data.AppendMany(curve.FitPoints);
        }

        public void DisplayPoint(Point pt, double xDiff, double yDiff, Color clr)
        {
            var ds = new ObservableDataSource<Point>();
            var pen = new Pen { Thickness = 1.5, Brush = new SolidColorBrush(clr) };
            var penDesc = new PenDescription();
            penDesc.LegendItem.Visibility = Visibility.Collapsed;
            Plotter.AddLineGraph(ds, pen, penDesc);
            var ptLeftTop = new Point(pt.X - xDiff, pt.Y - yDiff);
            var ptRightBottom = new Point(pt.X + xDiff, pt.Y + yDiff);
            ds.AppendAsync(_parentPanel.Dispatcher, ptLeftTop);
            ds.AppendAsync(_parentPanel.Dispatcher, ptRightBottom);

            ds = new ObservableDataSource<Point>();
            penDesc = new PenDescription();
            penDesc.LegendItem.Visibility = Visibility.Collapsed;
            Plotter.AddLineGraph(ds, pen.Clone(), penDesc);
            var ptRightTop = new Point(pt.X + xDiff, pt.Y - yDiff);
            var ptLeftBottom = new Point(pt.X - xDiff, pt.Y + yDiff);
            ds.AppendAsync(_parentPanel.Dispatcher, ptRightTop);
            ds.AppendAsync(_parentPanel.Dispatcher, ptLeftBottom);
        }

        public void DisplayMarkerCurve(ConfidenceRangeInfo curve, double index)
        {
            var pts = new[] { new Point(index, curve.PEValues.Upper), new Point(index, curve.PEValues.Lower) };
            var ds = new ObservableDataSource<Point>(pts);
            var pen = new Pen
            {
                Thickness = 1, 
                Brush = new SolidColorBrush(curve.Color),
                DashStyle = curve.Type == PlotTypes.DashedLine ? DashStyles.DashDotDot : DashStyles.Solid
            };
            var penDesc = new PenDescription();
            penDesc.LegendItem.Visibility = Visibility.Collapsed;
            var marker = new LinePointMarker {Brush = new SolidColorBrush(curve.Color)};
            Plotter.AddLineGraph(ds, pen, marker, penDesc);

            pts = new[] { new Point(index, curve.PEValues.Est) };
            ds = new ObservableDataSource<Point>(pts);
            penDesc = new PenDescription();
            penDesc.LegendItem.Visibility = Visibility.Collapsed;
            var circleMarker = new CirclePointMarker {Fill = new SolidColorBrush(curve.Color)};
            Plotter.AddLineGraph(ds, pen.Clone(), circleMarker, penDesc);
            Plotter.LegendVisible = false;
        }

        public void DisplayHistogram(Histogram hist)
        {
            for (var i = 0; i !=hist.PlotCoordinateListList.Count; ++i)
            {
                var pts = hist.PlotCoordinateListList[i];
                for (var j = 0; j != pts.Count; ++j)
                {
                    var pt = pts[j];
                    var ds = new ObservableDataSource<Point>();
                    var pen = new Pen
                    {
                        Thickness = 1,
                        Brush = new SolidColorBrush(hist.Color)
                    };
                    var penDesc = new PenDescription();
                    penDesc.LegendItem.Visibility = Visibility.Collapsed;
                    var marker = new RetangleElementMarker { Size=Histogram.BoxPixelSize, Fill=Brushes.White, ToolTipText = hist.FrequencyListList[i][j] };
                    Plotter.AddLineGraph(ds, pen, marker, penDesc);
                    ds.AppendAsync(_parentPanel.Dispatcher, pt);

                    var ds1 = new ObservableDataSource<Point>();
                    var pen1 = new Pen
                    {
                        Thickness = 1,
                        Brush = new SolidColorBrush(hist.Color)
                    };
                    var penDesc1 = new PenDescription();
                    penDesc1.LegendItem.Visibility = Visibility.Collapsed;
                    Plotter.AddLineGraph(ds1, pen1, penDesc1);
                    ds1.AppendAsync(_parentPanel.Dispatcher, new Point(hist.XLabelList[i], j));
                    ds1.AppendAsync(_parentPanel.Dispatcher, new Point(hist.XLabelList[i + 1], j));
                    ds1.AppendAsync(_parentPanel.Dispatcher, new Point(hist.XLabelList[i + 1], j + 1));
                    ds1.AppendAsync(_parentPanel.Dispatcher, new Point(hist.XLabelList[i], j + 1));
                    ds1.AppendAsync(_parentPanel.Dispatcher, new Point(hist.XLabelList[i], j));
                }
            }

            var dsDesc = new ObservableDataSource<Point>();
            var lpen = new Pen { Thickness = 0, Brush = Brushes.White };
            var lpenDesc = new PenDescription();
            lpenDesc.LegendItem.Visibility = Visibility.Collapsed;
            Plotter.AddLineGraph(dsDesc, lpen, lpenDesc);
            dsDesc.AppendAsync(_parentPanel.Dispatcher, new Point(hist.XLabelList.Min(), 0));
            dsDesc.AppendAsync(_parentPanel.Dispatcher, new Point(hist.XLabelList.Max(), hist.YRange + 2));
            Plotter.LegendVisible = false;
        }
    }
}
