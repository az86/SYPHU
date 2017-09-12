using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using SYPHU.Assay.Tables;
using SYPHU.Configs;

namespace SYPHU.Views.CommonControls
{
    class PrintElement
    {
        public UIElement Content;
    }

    class BannerDocumentPaginator : DocumentPaginator, IDisposable
    {
        private double _rowHeight;

        private readonly List<DrawingVisual> _drawingVisuals = new List<DrawingVisual>();

        private DrawingVisual _currentDv = new DrawingVisual();

        private Point _startPt = new Point(SizeDefines.PageMargin.Left, SizeDefines.PageMargin.Top);

        public BannerDocumentPaginator()
        {
            var header = new PageHeader();
            _currentDv.Children.Add(header);
            header.Arrange(new Rect(new Point(0, 0), SizeDefines.DefaultPageSize));
            _drawingVisuals.Add(_currentDv);
        }

        public void AddElement(PrintElement element)
        {
            //如果横向放不下，则放在纵向
            if (PageSize.Width < _startPt.X + element.Content.DesiredSize.Width + SizeDefines.PageMargin.Right)
            {
                _startPt.X = SizeDefines.PageMargin.Left;
                _startPt.Y += _rowHeight;
                _rowHeight = 0;
            }
            //如果纵向放不下，则换页
            if (PageSize.Height < _startPt.Y + element.Content.DesiredSize.Height + SizeDefines.PageMargin.Bottom)
            {
                _currentDv = new DrawingVisual();
                _drawingVisuals.Add(_currentDv);
                _startPt.X = SizeDefines.PageMargin.Left;
                _startPt.Y = SizeDefines.PageMargin.Top;

                var header = new PageHeader();
                _currentDv.Children.Add(header);
                header.Arrange(new Rect(new Point(0, 0), SizeDefines.DefaultPageSize));
            }
            _currentDv.Children.Add(element.Content);
            element.Content.Arrange(new Rect(_startPt, element.Content.DesiredSize));
            _startPt.X += element.Content.DesiredSize.Width;
            _rowHeight = element.Content.DesiredSize.Height > _rowHeight ? element.Content.DesiredSize.Height : _rowHeight;
        }

        public override bool IsPageCountValid { get { return true; } }

        public override int PageCount { get { return _drawingVisuals.Count; } }

        public override Size PageSize { get; set; }

        public override IDocumentPaginatorSource Source { get { return null; } }

        public override DocumentPage GetPage(int pageNumber)
        {
            var pageHeader = _drawingVisuals[pageNumber].Children[0] as PageHeader;
            pageHeader.tail.Text = (pageNumber + 1).ToString() + " / " + PageCount.ToString();
            pageHeader.UpdateLayout();
            return new DocumentPage(_drawingVisuals[pageNumber]);
        }

        public void Dispose()
        {
            foreach (var dv in _drawingVisuals)
            {
                dv.Children.Clear();
            }
            _drawingVisuals.Clear();
        }
    }
}
