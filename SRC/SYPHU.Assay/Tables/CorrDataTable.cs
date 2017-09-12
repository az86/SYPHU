using System.Collections.Generic;
using System.Linq;
using SYPHU.Assay.CalculationInfo;
using SYPHU.Assay.Data;

namespace SYPHU.Assay.Tables
{
    /// <summary>
    /// 校正后数据表格
    /// </summary>
    public class CorrDataTable
    {
        public List<DataTable> Tables = new List<DataTable>();

        public AssayData CorrData = new AssayData();

        public void CreateTable(List<List<int>> extremeAbnormalDataLocationList, InitCalculationInfo calculationInfo, TranDataTable tranDataTable)
        {
            if (calculationInfo.CalcCase == CalcCases.Single)
            {
                CreateSingleCalcTable(extremeAbnormalDataLocationList, calculationInfo, tranDataTable);
            }
            else if (calculationInfo.CalcCase == CalcCases.Merge)
            {
                CreateMergeCalcTable(extremeAbnormalDataLocationList, calculationInfo, tranDataTable);
            }
        }

        private void CreateSingleCalcTable(List<List<int>> extremeAbnormalDataLocationList,
                                           InitCalculationInfo calculationInfo, TranDataTable tranDataTable)
        {
            for (int i = 0; i < calculationInfo.DataSize.PreparationNum; i++)
            {
                Tables.Add(new DataTable());
                int groupId = (calculationInfo.Method == Methods.ED) ? i + 1 : i;
                Tables[i].InitTable(calculationInfo, groupId, false);
                Tables[i].Table.Category = TableCategory.CorrDataTable;
                Tables[i].Table.IsSeparator = (i == calculationInfo.DataSize.PreparationNum - 1);
                Tables[i].Table.IsShown = extremeAbnormalDataLocationList.Count > 0;
                Tables[i].Table.IsSeparator &= Tables[i].Table.IsShown;
                Tables[i].Table.DataColNum = calculationInfo.DataSize.DoseNum;
                Tables[i].Table.DataRowNum = calculationInfo.DataSize.ReplicateNum;
                Tables[i].Table.IsSingleCalcTable = calculationInfo.CalcCase == CalcCases.Single;
                if (Tables[i].Table.IsShown)
                {
                    Tables[i].Table.Cells[0][0].Content = ConstStrings.DataTableCategoryString[calculationInfo.Lang][Tables[i].Table.Category] + "--" + Tables[i].Table.Cells[0][0].Content;

                    Tables[i].Table.Paste(tranDataTable.Tables[i].Table.Copy());
                }
            }
            foreach (List<int> t in extremeAbnormalDataLocationList)
            {
                int actualCol = t[1] % TableDesc.MaxDataCol;
                int actualRow = t[2] + (t[1] / TableDesc.MaxDataCol) * (calculationInfo.DataSize.ReplicateNum + 1);
                Tables[t[0]].Table.Cells[Tables[t[0]].Table.DataStartRow + actualRow][
Tables[t[0]].Table.DataStartCol + actualCol].Content = CorrData.Data[t[0]][t[1]][t[2]].ToString("F6");
            }
        }

        private void CreateMergeCalcTable(List<List<int>> extremeAbnormalDataLocationList,
                                           InitCalculationInfo calculationInfo, TranDataTable tranDataTable)
        {
            for (int i = 0; i < calculationInfo.DataSize.PreparationNum; i++)
            {
                Tables.Add(new DataTable());
                Tables[i].InitTable(calculationInfo, i, false, extremeAbnormalDataLocationList.Count);
                Tables[i].Table.Category = TableCategory.CorrDataTable;
                Tables[i].Table.IsSeparator = (i == calculationInfo.DataSize.PreparationNum - 1);
                Tables[i].Table.IsShown = extremeAbnormalDataLocationList.Count > 0;
                Tables[i].Table.IsSeparator &= Tables[i].Table.IsShown;
                Tables[i].Table.DataColNum = calculationInfo.DataSize.DoseNum;
                Tables[i].Table.DataRowNum = calculationInfo.DataSize.ReplicateNum;
                Tables[i].Table.IsSingleCalcTable = calculationInfo.CalcCase == CalcCases.Single;
                if (Tables[i].Table.IsShown)
                {
                    Tables[i].Table.Cells[0][0].Content = ConstStrings.DataTableCategoryString[calculationInfo.Lang][Tables[i].Table.Category];
                    Tables[i].Table.Cells[0][2].Content = tranDataTable.Tables[i].Table.Cells[0][2].Content;
                    List<int> ridLoc = extremeAbnormalDataLocationList.Select(t => t[2]).ToList();
                    for (int j = 0; j < calculationInfo.DataSize.DoseNum; j++)
                    {
                        int actualLoc = 0;
                        for (int k = 0; k < calculationInfo.DataSize.ReplicateNum - extremeAbnormalDataLocationList.Count; k++)
                        {
                            if (ridLoc.Contains(k))
                            {
                                actualLoc++;
                            }
                            Tables[i].Table.Cells[Tables[i].Table.DataStartRow + k][
                                Tables[i].Table.DataStartCol + j].Content =
                                tranDataTable.Tables[i].Table.Cells[Tables[i].Table.DataStartRow + actualLoc][
                                    Tables[i].Table.DataStartCol + j].Content;
                            actualLoc++;
                        }
                    }
                }
            }
        }
    }
}
