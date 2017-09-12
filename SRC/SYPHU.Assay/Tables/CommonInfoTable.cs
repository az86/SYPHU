using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SYPHU.Assay.CalculationInfo;

namespace SYPHU.Assay.Tables
{
    /// <summary>
    /// 基本信息表
    /// </summary>
    [Serializable]
    public class CommonInfoTable
    {
        public TableDesc Table;

        public void CreateTable(OutLang lang)
        {
            Table = new TableDesc
                {
                    Category = TableCategory.CommonInfoTable,
                    IsSeparator = true,
                    IsShown = true,
                    IsAutoAdjustment = true,
                    DefaultColumnSizeInPixel = 50,
                    RowNum = 1,
                    ColNum = 2*TableHeaders.CommonInfoTH[lang].Count(),
                    DataStartRow = 1,
                    DataStartCol = 1,
                    Cells = new List<List<TableCellBase>> {new List<TableCellBase>()}
                };

            for (int i = 0; i < Table.ColNum; i += 2)
            {
                Table.Cells[0].Add(new TableCellBase());
                Table.Cells[0][i].SetValues(0, i, 1, 1, true, TableHeaders.CommonInfoTH[lang][i / 2]);
                Table.Cells[0].Add(new TableCellBase());
            }
            AssayDepartment = "                                                ";
            Reporter = "               ";
            Reviewer = "               ";
            Table.Cells[0][1].SetValues(0, 1, 1, 1, false, AssayDepartment);
            Table.Cells[0][3].SetValues(0, 3, 1, 1, false, Reporter);
            Table.Cells[0][5].SetValues(0, 5, 1, 1, false, Reviewer);
            Table.Cells[0][7].SetValues(0, 7, 1, 1, true, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

            Table.CalcColumnWidthInPixel();
        }

        public String Reporter { get; set; }

        public String Reviewer { get; set; }

        public String AssayDepartment { get; set; }
    }
}
