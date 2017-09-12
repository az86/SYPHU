using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SYPHU.Data
{
    public class PotencyAndConfidenceLimitTable
    {
        public TableDesc PCTable = new TableDesc();
        public String[] TableHorizontalHeader = new string[] { "(ugHA/dose)", "下限", "估计值", "上限"};
        public String[] TableVerticalHeader = new string[] { "效价", "Rel. to Ass", "Rel. to Est"};
        public void InitTable(int groupID)
        {
            PCTable.IsSeparator = true;
            PCTable.RowNum = 7;
            PCTable.ColNum = 4;
            PCTable.DataStartRow = 3;
            PCTable.DataStartCol = 1;
            PCTable.Cells = new List<List<TableCellBase>>();
            //第一行，表头：实验组
            PCTable.Cells.Add(new List<TableCellBase>());
            PCTable.Cells[0].Add(new TableCellBase());
            PCTable.Cells[0][0].SetValues(0, 0, 1, 4, true, "实验组");
            //第二行：组别，T1、T2...
            PCTable.Cells.Add(new List<TableCellBase>());
            PCTable.Cells[1].Add(new TableCellBase());
            PCTable.Cells[1][0].SetValues(1, 0, 1, 1, true, "组别");
            PCTable.Cells[1].Add(new TableCellBase());
            String groupLabel = "T" + Convert.ToString(groupID + 1);
            PCTable.Cells[1][1].SetValues(1, 1, 1, 3, true, groupLabel);
            //第三行
            PCTable.Cells.Add(new List<TableCellBase>());
            for (int i = 0; i < PCTable.ColNum; i++)
            {
                PCTable.Cells[2].Add(new TableCellBase());
                PCTable.Cells[2][i].SetValues(2, i, 1, 1, true, TableHorizontalHeader[i]);
            }
            //第四-六行，效价、Rel. to Ass、Rel. to Est
            for (int i = 3; i < PCTable.RowNum - 1; i++)
            {
                PCTable.Cells.Add(new List<TableCellBase>());
                for (int j = 0; j < PCTable.ColNum; j++)
                {
                    PCTable.Cells[i].Add(new TableCellBase());
                    String header = j == 0 ? TableVerticalHeader[i - 3] : "";
                    PCTable.Cells[i][j].SetValues(i, j, 1, 1, true, header);
                }
            }
            //第七行，可信限率
            PCTable.Cells.Add(new List<TableCellBase>());
            PCTable.Cells[6].Add(new TableCellBase());
            PCTable.Cells[6][0].SetValues(6,0,1,1,true, "可信限率");
            PCTable.Cells[6].Add(new TableCellBase());
            PCTable.Cells[6][1].SetValues(6, 1, 1, 3, true);
        }
    }
}
