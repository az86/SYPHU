using System;
using System.Collections.Generic;
using SYPHU.Assay.CalculationInfo;

namespace SYPHU.Assay.Tables
{
    public class SRCompletelyRandomisedVarAnaTable : VarianceAnalysisTable
    {
        public String[] TableVerticalHeader = new[] {"回归", "交叉", "非线性", "处理组", "残差", "总和"};

        public override void InitTable()
        {
            VarAnaTable.IsSeparator = true;
            VarAnaTable.RowNum = 7 + DataSizeInfo.Instance.PreparationNum;
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
            for (int i = 1; i <= 3; i++)
            {
                VarAnaTable.Cells.Add(new List<TableCellBase>());
                for (int j = 0; j < VarAnaTable.ColNum; j++)
                {
                    VarAnaTable.Cells[i].Add(new TableCellBase());
                    String header = j == 0 ? TableVerticalHeader[i - 1] : "";
                    VarAnaTable.Cells[i][j].SetValues(i, j, 1, 1, true, header);
                }
            }
            for (int i = 0; i < DataSizeInfo.Instance.PreparationNum; i++)
            {
                VarAnaTable.Cells.Add(new List<TableCellBase>());
                for (int j = 0; j < VarAnaTable.ColNum; j++)
                {
                    VarAnaTable.Cells[i + 4].Add(new TableCellBase());
                    String header = j == 0 ? (i == 0 ? "Standard" : "Sample" + i.ToString()) : "";
                    VarAnaTable.Cells[i + 4][j].SetValues(i + 4, j, 1, 1, true, header);
                }
            }
            for (int i = 4 + DataSizeInfo.Instance.PreparationNum; i < VarAnaTable.RowNum; i++)
            {
                VarAnaTable.Cells.Add(new List<TableCellBase>());
                for (int j = 0; j < VarAnaTable.ColNum; j++)
                {
                    VarAnaTable.Cells[i].Add(new TableCellBase());
                    String header = j == 0 ? TableVerticalHeader[i - DataSizeInfo.Instance.PreparationNum - 1] : "";
                    VarAnaTable.Cells[i][j].SetValues(i, j, 1, 1, true, header);
                }
            }
        }
    }
}