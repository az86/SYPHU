using System;
using System.Collections.Generic;
using SYPHU.Assay.CalculationInfo;

namespace SYPHU.Assay.Tables
{
    /// <summary>
    /// 可靠性结论表
    /// </summary>
    public class ReliabilityConclusionTable
    {
        public TableDesc Table;

        public void CreateTable(List<String> conclusionList, String finalConclusion, OutLang lang)
        {
            InitTable(lang);
            FillValues(conclusionList, finalConclusion);
        }

        private void InitTable(OutLang lang)
        {
            Table = new TableDesc
                {
                    Category = TableCategory.ReliabilityConclusionTable,
                    IsSeparator = true,
                    DefaultColumnSizeInPixel = 440,
                    ColNum = 1,
                    RowNum = 3,
                    Cells = new List<List<TableCellBase>> {new List<TableCellBase>()}
                };
            //第一行，表头
            Table.Cells[0].Add(new TableCellBase());
            Table.Cells[0][0].SetValues(0, 0, 1, 1, true, TableHeaders.VarAnaConclusionTH[lang][0]);
            //第二行，每个变异来源的判断结果
            Table.Cells.Add(new List<TableCellBase>());
            Table.Cells[1].Add(new TableCellBase());
            Table.Cells[1][0].SetValues(1,0,1,1,true);
            Table.Cells[1][0].IsTextWrapping = true;
            //第三行，总结论
            Table.Cells.Add(new List<TableCellBase>());
            Table.Cells[2].Add(new TableCellBase());
            Table.Cells[2][0].SetValues(2,0,1,1,true);
            Table.Cells[2][0].IsTextWrapping = true;

            Table.CalcColumnWidthInPixel();
        }

        private void FillValues(List<String> conclusionList, String finalConclusion)
        {
            Table.IsShown = (conclusionList != null && conclusionList.Count > 0);
            if (Table.IsShown)
            {
                if (conclusionList != null)
                    foreach (string t in conclusionList)
                    {
                        Table.Cells[1][0].Content += t;
                    }
                Table.Cells[2][0].Content = finalConclusion;
            }
        }
    }
}
