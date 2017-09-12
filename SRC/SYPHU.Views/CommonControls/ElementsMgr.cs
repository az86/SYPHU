using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using SYPHU.Assay.Plots;
using SYPHU.Assay.Tables;
using System.Windows.Input;
using SYPHU.Configs;

namespace SYPHU.Views.CommonControls
{
    class ElementsMgr
    {
        /// <summary>
        /// 子元素集合
        /// </summary>
        private readonly Panel _elementsPanel;

        /// <summary>
        /// 父元素
        /// </summary>
        private readonly UIElement _parent;

        private UserOperationMgr _usrOpMgr = new UserOperationMgr();

        public ElementsMgr(Panel elementsPanle, UIElement parent)
        {
            _elementsPanel = elementsPanle;
            _parent = parent;
        }

        public void Clear()
        {
            _elementsPanel.Children.Clear();
            _usrOpMgr = new UserOperationMgr();
        }

        public void UpdateTables(List<TableDesc> elementList, int startIndex = 0, int endIndex = int.MinValue)
        {
            while (elementList != null && startIndex < elementList.Count && startIndex != endIndex)
            {
                var tableDesc = elementList[startIndex++];
                if (tableDesc.IsShown)
                {
                    var table = new Table {UsrOpMgr = _usrOpMgr};
                    table.CreateTable(tableDesc);
                    _elementsPanel.Children.Add(table);
                }
                if (tableDesc.IsSeparator)
                {
                    CreateSeparator();
                }
            }
        }

        public void UnDo()
        {
            _usrOpMgr.UnDo();
        }

        public void ReDo()
        {
            _usrOpMgr.ReDo();
        }

        private void CreateSeparator(int height = 0)
        {
            var sp = new Separator
                {
                    Width = SystemParameters.PrimaryScreenWidth,
                    Height = height,
                    Background = Brushes.White
                };
            _elementsPanel.Children.Add(sp);
        }

        public void UpdateCharts(List<PlotInfo> plots, int startIndex = 0, int endIndex = int.MinValue)
        {
            while (plots != null && startIndex < plots.Count && startIndex != endIndex)
            {
                var plotInfo = plots[startIndex++];
                var plotter = new PlotDrawer(plotInfo, _parent);
                foreach (var pointGroup in plotInfo.DataPoints)
                {
                    foreach (var pts in pointGroup.Points)
                    {
                        pts.ForEach(pt => plotter.DisplayPoint(pt, plotInfo.XRange / 75, plotInfo.YRange / 75, pointGroup.Color));
                    }
                }
                plotInfo.CurveDescs.ForEach(plotter.DisplayCurve);
                
                _elementsPanel.Children.Add(plotter.Plotter);
            }
            CreateSeparator();
        }

        public void UpdateCharts(List<MergeCalcPlotInfo> plots, int startIndex = 0, int endIndex = int.MinValue)
        {
            while (plots != null && startIndex < plots.Count && startIndex != endIndex)
            {
                var plotInfo = plots[startIndex++];
                var plotter = new PlotDrawer(plotInfo, _parent);
                plotInfo.LimitsLines.ForEach(plotter.DisplayCurve);
 
                plotInfo.ConfidenceRangeInfos.ForEach(curve=>plotter.DisplayMarkerCurve(curve, plotInfo.ConfidenceRangeInfos.IndexOf(curve)));
                _elementsPanel.Children.Add(plotter.Plotter);
            }
            CreateSeparator();
        }

        public void UpdateCharts(Histogram hist)
        {
            var plotter = new PlotDrawer(hist, _parent);
            plotter.DisplayHistogram(hist);
            _elementsPanel.Children.Add(plotter.Plotter);
            CreateSeparator(35);
        }

        public void Print()
        {
            try
            {
                var dlg = new PrintDialog();
                if (dlg.ShowDialog().GetValueOrDefault())
                {
                    var elementList = (from object ele in _elementsPanel.Children select ele as UIElement).ToList();
                    _elementsPanel.Children.Clear();
                    dlg.UserPageRangeEnabled = true;
                    using (var paginator = new BannerDocumentPaginator())
                    {
                        paginator.PageSize = new Size(dlg.PrintableAreaWidth, dlg.PrintableAreaHeight);
                        foreach (var element in elementList)
                        {
                            if (typeof(Table) == element.GetType() || typeof(ChartPlotter) == element.GetType() || typeof(Separator) == element.GetType())
                            {
                                var printElement = new PrintElement { Content = element };
                                paginator.AddElement(printElement);
                            }
                        }
                        dlg.PrintDocument(paginator, "ReguStats");
                    }
                    foreach (var element in elementList)
                    {
                        _elementsPanel.Children.Add(element);
                    }
                }
            }
            catch (System.Exception ex)
            {
                SYPHU.ViewModels.MainFrameVM.OutputWinVM.ShowInformation(ex.Message);
            }
        }

        public void CopyTable()
        {
            try
            {
                _usrOpMgr.CopyTable();
            }
            catch (System.Exception ex)
            {
                SYPHU.ViewModels.MainFrameVM.OutputWinVM.ShowInformation(ex.Message);
            }
        }

        public void PasteTable()
        {
            try
            {
                _usrOpMgr.PasteTable();
            }
            catch (System.Exception ex)
            {
                SYPHU.ViewModels.MainFrameVM.OutputWinVM.ShowInformation(ex.Message);
            }
        }
    }
}
