using System;
using System.Collections.Generic;

namespace SYPHU.Assay.Tables
{
    public class PLRandomisedBlockVarAnaTable : VarianceAnalysisTable
    {
        public String[] TableVerticalHeader = new string[] { "组内", "组间", "非平行", "非线性", "处理组", "行", "残差", "总和" };
        public override void InitTable()
        {
            VarAnaTable.IsSeparator = true;
            VarAnaTable.RowNum = 9;
            VarAnaTable.ColNum = 6;
            VarAnaTable.DataStartRow = 1;
            VarAnaTable.DataStartCol = 1;
            VarAnaTable.Cells = new List<List<TableCellBase>>();
            //第一行表头
            VarAnaTable.Cells.Add(new List<TableCellBase>());
            for (int i = 0; i < VarAnaTable.ColNum; i++)
            {
                VarAnaTable.Cells[0].Add(new TableCellBase());
                VarAnaTable.Cells[0][i].SetValues(0, i, 1, 1, true, TableHorizontalHeader[i]);
            }
            //方差分析数据
            for (int i = 1; i < VarAnaTable.RowNum; i++)
            {
                VarAnaTable.Cells.Add(new List<TableCellBase>());
                for (int j = 0; j < VarAnaTable.ColNum; j++)
                {
                    VarAnaTable.Cells[i].Add(new TableCellBase());
                    String header = j == 0 ? TableVerticalHeader[i - 1] : "";
                    VarAnaTable.Cells[i][j].SetValues(i, j, 1, 1, true, header);
                }
            }
        }
    }
}
