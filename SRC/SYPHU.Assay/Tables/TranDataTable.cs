using System.Collections.Generic;
using SYPHU.Assay.CalculationInfo;
using SYPHU.Assay.Data;

namespace SYPHU.Assay.Tables
{
    /// <summary>
    /// 转换后数据表格类
    /// </summary>
    public class TranDataTable
    {
        public List<DataTable> Tables = new List<DataTable>();

        public AssayData TranData = new AssayData();

        public void CreateTable(InitCalculationInfo calculationInfo, OrigDataTable origDataTable)
        {
            if (calculationInfo.CalcCase == CalcCases.Single)
            {
                CreateSingleCalcTable(calculationInfo, origDataTable);
            }
            else if (calculationInfo.CalcCase == CalcCases.Merge)
            {
                CreateMergeCalcTable(calculationInfo, origDataTable);
            }
        }

        private void CreateSingleCalcTable(InitCalculationInfo calculationInfo, OrigDataTable origDataTable)
        {
            for (int i = 0; i < calculationInfo.DataSize.PreparationNum; i++)
            {
                Tables.Add(new DataTable());
                int groupId = (calculationInfo.Method == Methods.ED) ? i + 1 : i;
                Tables[i].InitTable(calculationInfo, groupId, false);
                Tables[i].Table.Category = TableCategory.TranDataTable;
                Tables[i].Table.IsSeparator = (i == calculationInfo.DataSize.PreparationNum - 1);
                Tables[i].Table.IsShown = (calculationInfo.DataTransFormula != DataTransformationFormula.Null);
                Tables[i].Table.IsSeparator &= Tables[i].Table.IsShown;
                Tables[i].Table.DataColNum = calculationInfo.DataSize.DoseNum;
                Tables[i].Table.DataRowNum = calculationInfo.DataSize.ReplicateNum;

                Tables[i].Table.Cells[0][0].Content =
                    ConstStrings.DataTableCategoryString[calculationInfo.Lang][Tables[i].Table.Category] + "--" +
                    Tables[i].Table.Cells[0][0].Content;

                Tables[i].Table.IsSingleCalcTable = calculationInfo.CalcCase == CalcCases.Single;
                Tables[i].Table.Paste(origDataTable.Tables[i].Table.Copy());

                if (Tables[i].Table.IsShown)
                {
                    int curCol = 0;
                    for (int isplit = 0; isplit <= Tables[i].SplitNum; isplit++)
                    {
                        for (int j = 0; j < Tables[i].Table.ColNum - 1; j++)
                        {
                            if (curCol < calculationInfo.DataSize.DoseNum)
                            {
                                for (int k = 0; k < calculationInfo.DataSize.ReplicateNum; k++)
                                {
                                    Tables[i].Table.Cells[
                                        Tables[i].Table.DataStartRow + k +
                                        isplit * (calculationInfo.DataSize.ReplicateNum + 1)][
                                            Tables[i].Table.DataStartCol + j]
                                        .Content = TranData.Data[i][curCol][k].ToString("F4");
                                }
                                curCol++;
                            }
                        }
                    }
                }
            }
        }

        private void CreateMergeCalcTable(InitCalculationInfo calculationInfo, OrigDataTable origDataTable)
        {
            for (int i = 0; i < calculationInfo.DataSize.PreparationNum; i++)
            {
                Tables.Add(new DataTable());
                Tables[i].InitTable(calculationInfo, i, false);
                Tables[i].Table.Category = TableCategory.TranDataTable;
                Tables[i].Table.IsSeparator = (i == calculationInfo.DataSize.PreparationNum - 1);
                Tables[i].Table.IsShown = (calculationInfo.DataTransFormula != DataTransformationFormula.Null);
                Tables[i].Table.IsSeparator &= Tables[i].Table.IsShown;
                Tables[i].Table.DataColNum = calculationInfo.DataSize.DoseNum;
                Tables[i].Table.DataRowNum = calculationInfo.DataSize.ReplicateNum;

                Tables[i].Table.Cells[0][0].Content = ConstStrings.DataTableCategoryString[calculationInfo.Lang][Tables[i].Table.Category];

                Tables[i].Table.IsSingleCalcTable = calculationInfo.CalcCase == CalcCases.Single;
                Tables[i].Table.Paste(origDataTable.Tables[i].Table.Copy());

                if (Tables[i].Table.IsShown)
                {
                    for (int j = 0; j < calculationInfo.DataSize.DoseNum; j++)
                    {
                        for (int k = 0; k < calculationInfo.DataSize.ReplicateNum; k++)
                        {
                            if ((calculationInfo.DataSize.DoseNum == 5 && (j >= 1 && j <= 3)) 
                                || (calculationInfo.DataSize.DoseNum == 4 && j == 1))
                            {
                                Tables[i].Table.Cells[Tables[i].Table.DataStartRow + k][
                                    Tables[i].Table.DataStartCol + j].Content = TranData.Data[i][j][k].ToString("F4");
                            }
                        }
                    }
                }
            }
        }
    }
}
