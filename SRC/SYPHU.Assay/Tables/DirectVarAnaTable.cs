using System.Collections.Generic;
using SYPHU.Assay.CalculationInfo;

namespace SYPHU.Assay.Tables
{
    /// <summary>
    /// 直接法方差分析表
    /// </summary>
    public class DirectVarAnaTable
    {
        public TableDesc Table;

        public void InitTable(OutLang lang)
        {
            Table = new TableDesc
                {
                    Category = TableCategory.VarianceAnalysisTable,
                    IsSeparator = true,
                    IsShown = true,
                    RowNum = 2,
                    ColNum = TableHeaders.DirectVarAnaVerticalTH[lang].Length,
                    DataStartCol = 0,
                    DataStartRow = 1,
                    Cells = new List<List<TableCellBase>>()
                };
            for (int i = 0; i < Table.ColNum; i++)
            {
                Table.Cells.Add(new List<TableCellBase>());
                for (int j = 0; j < Table.RowNum; j++)
                {
                    Table.Cells[i].Add(new TableCellBase());
                    Table.Cells[i][j].SetValues(i, j, 1, 1, true, (i == 0 ? TableHeaders.DirectVarAnaVerticalTH[lang][j] : ""));
                }
            }
            Table.CalcColumnWidthInPixel();
        }
    }
}
