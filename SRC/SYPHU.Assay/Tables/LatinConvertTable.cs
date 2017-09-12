using System;
using System.Collections.Generic;
using System.Globalization;
using SYPHU.Assay.CalculationInfo;

namespace SYPHU.Assay.Tables
{
    /// <summary>
    /// 拉丁方转换表
    /// </summary>
    [Serializable]
    public class LatinConvertTable
    {
        public TableDesc Table;

        /// <summary>
        /// 转换矩阵
        /// </summary>
        public List<List<List<List<int>>>> ConvertMatrix;

        private DataSize _dataSize;

        public void InitTable(InitCalculationInfo calculationInfo)
        {
            _dataSize = calculationInfo.DataSize;
            Table = new TableDesc
                {
                    Category = TableCategory.LatinConvertTable,
                    IsSeparator = true,
                    IsShown = true,
                    RowNum = _dataSize.ReplicateNum+1,
                    ColNum = _dataSize.PreparationNum*_dataSize.DoseNum,
                    DataStartRow = 1,
                    DataStartCol = 0,
                    DataRowNum = _dataSize.ReplicateNum,
                    DataColNum = _dataSize.ReplicateNum,
                    Cells = new List<List<TableCellBase>>()
                };

            Table.Cells.Add(new List<TableCellBase>());
            Table.Cells[0].Add(new TableCellBase());
            Table.Cells[0][0].SetValues(0,0,1,Table.ColNum,true,TableHeaders.LatinConvertTH[calculationInfo.Lang][0]);
            for (int i = 1; i < Table.RowNum; i++)
            {
                Table.Cells.Add(new List<TableCellBase>());
                for (int j = 0; j < Table.ColNum; j++)
                {
                    Table.Cells[i].Add(new TableCellBase());
                    Table.Cells[i][j].SetValues(i, j, 1, 1, false);
                }
            }
            Table.CalcColumnWidthInPixel();
        }

        public String Parse()
        {
            ConvertMatrix = new List<List<List<List<int>>>>();
            for (int i = 0; i < _dataSize.PreparationNum; i++)
            {
                ConvertMatrix.Add(new List<List<List<int>>>());
                for (int j = 0; j < _dataSize.DoseNum; j++)
                {
                    ConvertMatrix[i].Add(new List<List<int>>());
                    for (int k = 0; k < _dataSize.ReplicateNum; k++)
                    {
                        ConvertMatrix[i][j].Add(new List<int>());
                    }
                }
            }
            for (int i = 0; i < _dataSize.ReplicateNum; i++)
            {
                for (int j = 0; j < _dataSize.PreparationNum; j++)
                {
                    for (int k = 0; k < _dataSize.DoseNum; k++)
                    {
                        int cellCol = k + j*_dataSize.DoseNum;
                        int cellRow = i + 1;
                        int label;
                        if (Table.Cells[cellRow][cellCol].Content == "")
                        {
                            return "请将拉丁方转换表输入完整.";
                        }
                        if (!Int32.TryParse(Table.Cells[cellRow][cellCol].Content, out label))
                        {
                            return "拉丁方转换表应输入整数值.";
                        }
                        if (label <= 0 || label > _dataSize.ReplicateNum)
                        {
                            return "转换值越界，转换值应在1到" + _dataSize.ReplicateNum.ToString(CultureInfo.InvariantCulture) + "之间.";
                        }
                        ConvertMatrix[j][k][i].Add((label - 1) / _dataSize.DoseNum);
                        ConvertMatrix[j][k][i].Add((label - 1) % _dataSize.DoseNum);
                    }
                }
            }
            return null;
        }
    }
}
