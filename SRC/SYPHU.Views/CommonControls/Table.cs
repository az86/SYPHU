using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using SYPHU.Assay.Tables;

namespace SYPHU.Views.CommonControls
{
    /// <summary>
    /// 表格类
    /// </summary>
    class Table : Border
    {
        /// <summary>
        /// 相关的表描述
        /// </summary>
        public TableDesc Description { get; private set; }

        /// <summary>
        /// 用户操作管理器
        /// </summary>
        public UserOperationMgr UsrOpMgr { get; set; }

        /// <summary>
        /// 当前焦点元素，用于上下左右的移动
        /// </summary>
        private Border _focusBorder;

        public void CreateTable(TableDesc tableDesc)
        {
            System.Diagnostics.Debug.WriteLine("CreateTable start");
            var grid = CreateGrid(tableDesc.RowNum, tableDesc.ColNum, tableDesc.ActualColumnWidthInPixel);
            foreach (var border in tableDesc.Cells.SelectMany(rowCells => rowCells.Select(cell => CreateCellView(cell))))
            {
                grid.Children.Add(border);
            }
            Child = grid;
            Description = tableDesc;
            //Height = tableDesc.HeightInPixel;
            Width = tableDesc.WidthInPixel;
            BorderThickness = new Thickness(0, 0, 1, 1);
            BorderBrush = Brushes.Black;
            Margin = new Thickness(tableDesc.HorizontalSpaceInPixel, tableDesc.VerticalSpaceInPixel, 0, 0);
            System.Diagnostics.Debug.WriteLine("CreateTable end");
        }

        private Grid CreateGrid(int rowNum, int colNum, List<int> widths)
        {
            var grid = new Grid();
            for (var colIndex = 0; colIndex != colNum; ++colIndex)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(widths[colIndex]) });
            }
            for (var rowIndex = 0; rowIndex != rowNum; ++rowIndex)
            {
                grid.RowDefinitions.Add(new RowDefinition());
            }
            grid.PreviewKeyDown += grid_KeyDown;
            return grid;
        }

        private Border CreateCellView(TableCellBase cell)
        {
            System.Diagnostics.Debug.WriteLine("CreateCellView start");
            var border = new Border { BorderThickness = new Thickness(1, 1, 0, 0), BorderBrush = Brushes.Black };
            border.SetValue(Grid.RowProperty, cell.Row);
            border.SetValue(Grid.ColumnProperty, cell.Col);
            border.SetValue(Grid.RowSpanProperty, cell.RowSpan);
            border.SetValue(Grid.ColumnSpanProperty, cell.ColSpan);

            if (cell.IsReadOnly)
            {
                border.Child = new TextBlock
                {
                    Text = cell.Content,
                    TextWrapping = cell.IsTextWrapping ? TextWrapping.Wrap : TextWrapping.NoWrap
                };
            }
            else
            {
                var tb = new TextBox { BorderThickness = new Thickness(0) };
                var bind = new Binding { Source = cell, Mode = BindingMode.TwoWay, Path = new PropertyPath("Content") };
                tb.SetBinding(TextBox.TextProperty, bind);
                border.Child = tb;
                border.GotFocus += border_GotFocus;
            }
            System.Diagnostics.Debug.WriteLine("CreateCellView end");
            return border;
        }

        void grid_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var grid = sender as Grid;
            switch (e.Key)
            {
                case Key.Enter:
                case Key.Down:
                    FocusNextElement(grid, _focusBorder);
                    e.Handled = true;
                    break;
                case Key.Up:
                    FocusFrontElement(grid, _focusBorder);
                    e.Handled = true;                   
                    break;
            }
        }

        void FocusNextElement(Grid grid, Border focusBorder)
        {
            if (focusBorder == null || grid == null)
                return;
            var curCol = (int)_focusBorder.GetValue(Grid.ColumnProperty);
            for (var index = grid.Children.IndexOf(focusBorder) + 1; index < grid.Children.Count; ++index)
            {
                var border = grid.Children[index] as Border;
                if (border != null && (int)border.GetValue(Grid.ColumnProperty) == curCol)
                {
                    border.Child.Focus();
                    break;
                }
            }
        }

        void FocusFrontElement(Grid grid, Border focusBorder)
        {
            if (focusBorder == null || grid == null)
                return;
            var curCol = (int)_focusBorder.GetValue(Grid.ColumnProperty);
            for (var index = grid.Children.IndexOf(focusBorder) - 1; index >= 0; --index)
            {
                var border = grid.Children[index] as Border;
                if (border != null && (int)border.GetValue(Grid.ColumnProperty) == curCol)
                {
                    border.Child.Focus();
                    break;
                }
            }
        }

        void border_GotFocus(object sender, RoutedEventArgs e)
        {
            _focusBorder = sender as Border;
            if (UsrOpMgr == null || _focusBorder == null)
                return;
            var tb = _focusBorder.Child as TextBox;
            if (tb == null)
                return;
            UsrOpMgr.AddOpTextBox(tb);
            tb.SelectAll();
        }

        public void CopyTable()
        {
            Description.Copy().CopyToClipBoard();
        }

        public void PasteTable()
        {
            var modifiedTextBoxs = new List<TextBox>();
            var grid = _focusBorder.Parent as Grid;
            var startRow = (int)_focusBorder.GetValue(Grid.RowProperty);
            var startCol = (int)_focusBorder.GetValue(Grid.ColumnProperty);
            if (grid.RowDefinitions.Count < 4)
                return;
            var rowStrs = Clipboard.GetText().Replace("\r\n", "\n").Split('\n');
            rowStrs = rowStrs.Where(str => !String.IsNullOrEmpty(str)).ToArray();
            //var matrixStrs = rowStrs.Select(tmpStr => tmpStr.Split('\t').ToList()).ToList();
            var matrixStrs = new List<List<string>>();
            for (int i = 0; i < rowStrs.Length; i++)
            {
                matrixStrs.Add(new List<string>());
                matrixStrs[i] = rowStrs[i].Split('\t').ToList();
                while (matrixStrs[i].Count > 0 && matrixStrs[i].Last() == "")
                {
                    matrixStrs[i].RemoveAt(matrixStrs[i].Count - 1);
                }
            }

            Description.Paste(matrixStrs, startCol, startRow);
            UsrOpMgr.AddOpTextBox(modifiedTextBoxs);
        }
    }
}
