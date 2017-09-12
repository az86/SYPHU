using System.Collections.Generic;
using SYPHU.Assay.CalculationInfo;
using SYPHU.Assay.Data;

namespace SYPHU.Assay.Tables
{
    /// <summary>
    /// 拉丁方数据表格
    /// </summary>
    public class LatinDataTable
    {
        public List<DataTable> Tables = new List<DataTable>();

        public AssayData LatinData = new AssayData();

        public void CreateTable(InitCalculationInfo calculationInfo, OrigDataTable origDataTable)
        {
            for (int i = 0; i < calculationInfo.DataSize.PreparationNum; i++)
            {
                Tables.Add(new DataTable());
                Tables[i].InitTable(calculationInfo, i, false);
                Tables[i].Table.Category = TableCategory.LatinDataTable;
                Tables[i].Table.IsSeparator = (i == calculationInfo.DataSize.PreparationNum - 1);
                Tables[i].Table.IsShown = true;
                Tables[i].Table.IsSeparator &= Tables[i].Table.IsShown;
                Tables[i].Table.Cells[2][1].Content = origDataTable.Tables[i].Table.Cells[2][1].Content;
                Tables[i].Table.Cells[3][1].Content = origDataTable.Tables[i].Table.Cells[3][1].Content;
                Tables[i].Table.Cells[0][0].Content = ConstStrings.DataTableCategoryString[calculationInfo.Lang][Tables[i].Table.Category] + "--" + Tables[i].Table.Cells[0][0].Content;
                Tables[i].Table.IsSingleCalcTable = calculationInfo.CalcCase == CalcCases.Single;
                for (int j = 0; j < calculationInfo.DataSize.DoseNum; j++)
                {
                    Tables[i].Table.Cells[4][j + 1].Content = origDataTable.Tables[i].Table.Cells[4][j + 1].Content;
                    for (int k = 0; k < calculationInfo.DataSize.ReplicateNum; k++)
                    {
                        Tables[i].Table.Cells[Tables[i].Table.DataStartRow + k][
                            Tables[i].Table.DataStartCol + j].Content = LatinData.Data[i][j][k].ToString("F4");
                    }
                }
            }
        }
    }
}
