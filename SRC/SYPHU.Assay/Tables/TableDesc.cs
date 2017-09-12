using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using SYPHU.Assay.Data;
using SYPHU.Utilities;

namespace SYPHU.Assay.Tables
{
    /// <summary>
    /// 表格描述
    /// </summary>
    [Serializable]
    public class TableDesc
    {
        /// <summary>
        /// 类别
        /// </summary>
        public TableCategory Category;

        /// <summary>
        /// 行数
        /// </summary>
        public int RowNum;

        /// <summary>
        /// 列数
        /// </summary>
        public int ColNum;

        /// <summary>
        /// 有效数据行数
        /// </summary>
        public int DataRowNum;

        /// <summary>
        /// 有效数据列数
        /// </summary>
        public int DataColNum;

        public const int MaxDataCol = 8;

        public int SplitNum
        { get { return (DataColNum - 1) / MaxDataCol; } }

        public bool IsSingleCalcTable = true;
        
        /// <summary>
        /// 是否换行
        /// </summary>
        public bool IsSeparator;

        /// <summary>
        /// 是否显示
        /// </summary>
        public bool IsShown = true;

        /// <summary>
        /// 是否自动调整大小以适应文字长度
        /// </summary>
        public bool IsAutoAdjustment;

        /// <summary>
        /// 表格内容
        /// </summary>
        public List<List<TableCellBase>> Cells;

        /// <summary>
        /// 数据部分起始行
        /// </summary>
        public int DataStartRow;

        /// <summary>
        /// 数据部分起始列
        /// </summary>
        public int DataStartCol;
        
        /// <summary>
        /// 宽度（像素）
        /// </summary>
        public int WidthInPixel
        {
            get { return ActualColumnWidthInPixel == null ? 0 : ActualColumnWidthInPixel.Sum(); }
        }

        /// <summary>
        /// 高度（像素）
        /// </summary>
        public int HeightInPixel
        {
            get { return RowNum * DefaultRowSizeInPixel; }
        }

        private int _defaultColumnSizeInPixel = 70;

        /// <summary>
        /// 默认列宽
        /// </summary>
        public int DefaultColumnSizeInPixel
        {
            get { return _defaultColumnSizeInPixel; }
            set { _defaultColumnSizeInPixel = value; }
        }

        public List<int> ActualColumnWidthInPixel;

        public void CalcColumnWidthInPixel()
        {
            ActualColumnWidthInPixel = new List<int>();
            for (int i = 0; i < ColNum; i++)
            {
                ActualColumnWidthInPixel.Add(GetCurrentColumnWidthInPixel(i));
            }
        }

        /// <summary>
        /// 计算当前列的宽度
        /// </summary>
        /// <param name="columnId">列的索引</param>
        /// <returns></returns>
        private int GetCurrentColumnWidthInPixel(int columnId)
        {
            if (ColNum <= 0 || RowNum <= 0 || columnId >= ColNum)
            {
                return 0;
            }
            if (IsAutoAdjustment)
            {
                int curMaxLen = DefaultColumnSizeInPixel;
                for (int i = 0; i < RowNum; i++)
                {
                    int curId = Cells[i].Count <= columnId ? Cells[i].Count - 1 : columnId;
                    int curLen = Cells[i][curId].Content != null
                                     ? (int)
                                       (UIControls.CalculateTextWidth(Cells[i][curId].Content)/Cells[i][curId].ColSpan) + 1
                                     : DefaultColumnSizeInPixel;
                    if (curLen > curMaxLen)
                    {
                        curMaxLen = curLen;
                    }
                }
                return curMaxLen;
            }
            return DefaultColumnSizeInPixel;
        }

        private int _defaultRowSizeInPixel = 25;


        /// <summary>
        /// 默认行宽
        /// </summary>
        public int DefaultRowSizeInPixel
        {
            get { return _defaultRowSizeInPixel; }
            set { _defaultRowSizeInPixel = value; }
        }

        /// <summary>
        /// 表格间横向间隔大小
        /// </summary>
        public int HorizontalSpaceInPixel = 23;

        /// <summary>
        /// 表格间纵向间隔大小
        /// </summary>
        public int VerticalSpaceInPixel = 23;

        public CopyData Copy()
        {
            var cpData = new CopyData();
            CopyAss(cpData);
            CopyDil(cpData);
            CopyDoses(cpData);
            CopyAssData(cpData);
            return cpData;
        }

        #region copy

        private void CopyAss(CopyData cpData)
        {
            if (Category == TableCategory.LatinConvertTable)
            {
                return;
            }
            cpData.Ass = IsSingleCalcTable ? Cells[2][1].Content : Cells[0][2].Content;
        }

        private void CopyDil(CopyData cpData)
        {
            if (Category == TableCategory.LatinConvertTable)
            {
                return;
            }
            cpData.Dil = IsSingleCalcTable ? Cells[3][1].Content : "";
        }

        private void CopyDoses(CopyData cpData)
        {
            if (Category == TableCategory.LatinConvertTable)
            {
                return;
            }
            if (IsSingleCalcTable)
            {
                cpData.DoseList = new List<string>();
                int curDataCol = 0;
                for (int isplit = 0; isplit <= SplitNum; isplit++)
                {
                    for (int i = 1; i < ColNum; i++)
                    {
                        curDataCol++;
                        if (curDataCol <= DataColNum)
                        {
                            //cpData.DoseList.Add(IsSingleCalcTable ? Cells[4 + (DataRowNum + 1) * isplit][i].Content : "");
                            cpData.DoseList.Add(Cells[4 + (DataRowNum + 1)*isplit][i].Content);
                        }
                    }
                }
            }
        }

        private void CopyAssData(CopyData cpData)
        {
            cpData.DataListList = new List<List<string>>();
            if (Category == TableCategory.LatinConvertTable)
            {
                for (int i = 0; i < DataRowNum; i++)
                {
                    cpData.DataListList.Add(new List<string>());
                    for (int j = 0; j < DataColNum; j++)
                    {
                        cpData.DataListList[i].Add(Cells[i + 1][j].Content);
                    }
                }
                return;
            }

            for (int i = 0; i < DataRowNum; i++)
            {
                cpData.DataListList.Add(new List<string>());
            }
            for (int isplit = 0; isplit <= SplitNum; isplit++)
            {
                for (int i = 0; i < DataRowNum; i++)
                {
                    for (int j = 0; j < ColNum - 1; j++)
                    {
                        int curCol = isplit*ColNum + j;
                        if (curCol <= DataColNum)
                        {
                            cpData.DataListList[i].Add(
                                Cells[DataStartRow + i + isplit*(DataRowNum + 1)][DataStartCol + j].Content);
                        }
                    }
                }
            }
        }

        #endregion
        
        public void Paste(CopyData cpData)
        {
            if (cpData == null)
            {
                return;
            }
            PasteAss(cpData);
            PasteDil(cpData);
            PasteDoses(cpData);
            PasteAssData(cpData);
        }

        #region paste

        private void PasteAss(CopyData cpData)
        {
            if (Category == TableCategory.LatinConvertTable)
            {
                return;
            }
            if (IsSingleCalcTable)
            {
                Cells[2][1].Content = cpData.Ass;
            }
            else
            {
                Cells[0][2].Content = cpData.Ass;
            }
        }

        private void PasteDil(CopyData cpData)
        {
            if (Category == TableCategory.LatinConvertTable)
            {
                return;
            }
            if (IsSingleCalcTable)
            {
                Cells[3][1].Content = cpData.Dil;
            }
        }

        private void PasteDoses(CopyData cpData)
        {
            if (Category == TableCategory.LatinConvertTable)
            {
                return;
            }
            if (IsSingleCalcTable)
            {
                int pos = 0;
                for (int isplit = 0; isplit <= SplitNum; isplit++)
                {
                    for (int i = 1; i < ColNum; i++)
                    {
                        if (pos < DataColNum)
                        {
                            Cells[4 + (DataRowNum + 1)*isplit][i].Content = cpData.DoseList[pos];
                            pos++;
                        }
                    }
                }
            }
        }

        private void PasteAssData(CopyData cpData)
        {
            if (Category == TableCategory.LatinConvertTable)
            {
                for (int i = 0; i < DataRowNum; i++)
                {
                    for (int j = 0; j < DataColNum; j++)
                    {
                        Cells[DataStartRow + i][DataStartCol + j].Content = cpData.DataListList[i][j];
                    }
                }
                return;
            }
            int curCol = 0;
            for (int isplit = 0; isplit <= SplitNum; isplit++)
            {
                for (int j = 0; j < ColNum - 1; j++)
                {
                    if (curCol < DataColNum)
                    {
                        for (int i = 0; i < DataRowNum; i++)
                        {
                            Cells[DataStartRow + i + isplit*(DataRowNum + 1)][DataStartCol + j].Content =
                                cpData.DataListList[i][curCol];
                        }
                        curCol++;
                    }
                }
            }
        }

        #endregion

        public void Paste(List<List<String>> cpData, int startCol = 0, int startRow = 0)
        {
            if (cpData == null || cpData.Count == 0)
            {
                return;
            }
            if (Category == TableCategory.OrigDataTable)
            {
                for (int i = 0; i < cpData.Count; i++)
                {
                    for (int j = 0; j < cpData[i].Count; j++)
                    {
                        int curRow = startRow + i;
                        int curCol = startCol + j;
                        if (!IsSingleCalcTable && startRow < DataStartRow && curRow >= DataStartRow)
                        {
                            curCol -= 2;
                        }
                        //if (curRow < RowNum && curCol <= DataColNum)
                        if (curRow < DataStartRow + DataRowNum && curCol <= DataColNum)
                        {
                            List<int> transIndex = IndexTrans(curRow, curCol);
                            if (transIndex[0] < Cells.Count && transIndex[1] < Cells[transIndex[0]].Count)
                            {
                                Cells[transIndex[0]][transIndex[1]].Content = cpData[i][j];
                            }
                        }
                    }
                }
            }
            else if (Category == TableCategory.LatinConvertTable)
            {
                for (int i = 0; i < cpData.Count; i++)
                {
                    for (int j = 0; j < cpData[i].Count; j++)
                    {
                        int curRow = startRow + i;
                        int curCol = startCol + j;
                        if (curRow < DataStartRow + DataRowNum && curCol < DataStartCol + DataColNum)
                        {
                            Cells[curRow][curCol].Content = cpData[i][j];
                        }
                    }
                }
            }
            
        }

        private List<int> IndexTrans(int curRow, int curCol)
        {
            var retIndex = new List<int>();
            int split = 0;
            int surplus = 0;
            int actualRow;
            int actualCol;
            if (IsSingleCalcTable)
            {
                if (curRow < DataStartRow - 1)
                {
                    actualRow = curRow;
                    actualCol = curCol;
                }
                else
                {
                    split = (curCol - DataStartCol/* - 1*/) / MaxDataCol;
                    surplus = (curCol - DataStartCol) % MaxDataCol;
                    actualRow = curRow + split * (1 + DataRowNum);
                    actualCol = surplus + DataStartCol;
                }
            }
            else
            {
                if (curRow < DataStartRow)
                {
                    actualRow = curRow;
                    actualCol = curCol - 1;
                }
                else
                {
                    split = (curCol - DataStartCol/* - 1*/) / MaxDataCol;
                    surplus = (curCol - DataStartCol) % MaxDataCol;
                    actualRow = curRow + split * DataRowNum;
                    actualCol = surplus + DataStartCol;
                }
            }
            retIndex.Add(actualRow);
            retIndex.Add(actualCol);
            return retIndex;
        }
    }
}