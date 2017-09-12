using System;
using System.ComponentModel;
using SYPHU.Assay.Annotations;

namespace SYPHU.Assay.Tables
{
    /// <summary>
    /// 表格基本单元格
    /// </summary>
    [Serializable]
    public class TableCellBase :VMBase
    {
        /// <summary>
        /// 所在行
        /// </summary>
        public int Row;

        /// <summary>
        /// 所在列
        /// </summary>
        public int Col;

        /// <summary>
        /// 行跨度
        /// </summary>
        public int RowSpan = 1;

        /// <summary>
        /// 列跨度
        /// </summary>
        public int ColSpan = 1;

        /// <summary>
        /// 是否只读
        /// </summary>
        public bool IsReadOnly;

        /// <summary>
        /// 是否自动换行
        /// </summary>
        public bool IsTextWrapping;

        private String _content;

        /// <summary>
        /// 文本内容
        /// </summary>
        public String Content
        {
            get { return _content; }
            set
            {
                _content = value;
                OnPropertyChanged("Content");
                //System.Diagnostics.Debug.WriteLine(value);
            }
        }

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="rowSpan"></param>
        /// <param name="colSpan"></param>
        /// <param name="isReadOnly"></param>
        /// <param name="content"></param>
        public void SetValues(int row, int col, int rowSpan, int colSpan, bool isReadOnly, String content = "")
        {
            Row = row;
            Col = col;
            RowSpan = rowSpan;
            ColSpan = colSpan;
            IsReadOnly = isReadOnly;
            Content = content;
        }

 
    }
}
