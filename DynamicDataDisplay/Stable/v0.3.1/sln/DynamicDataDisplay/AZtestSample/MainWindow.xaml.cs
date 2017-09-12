using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay.PointMarkers;

namespace AZtestSample
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var ds = new ObservableDataSource<Point>();
            var penDesc = new PenDescription("AZ");
            penDesc.LegendItem.Visibility = Visibility.Collapsed;
            plotter.AddLineGraph(
                ds,
                new Pen(),
                new RetangleElementMarker { Size = 50, Fill = Brushes.Blue, ToolTipText = "AZ" },
                penDesc);
            ds.AppendAsync(Dispatcher, new Point(100, 100));
        }
    }
}
