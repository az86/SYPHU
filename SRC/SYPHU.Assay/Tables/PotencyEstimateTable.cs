using System;
using System.Collections.Generic;
using SYPHU.Assay.CalculationInfo;

namespace SYPHU.Assay.Tables
{
    /// <summary>
    /// 效价估计表格
    /// </summary>
    public class PotencyEstimateTable
    {
        public TableDesc Table;

        private OutLang _lang;

        public void InitTable(int groupNum, int groupId, InitCalculationInfo calculationInfo)
        {
            _lang = calculationInfo.Lang;
            if (calculationInfo.CalcCase == CalcCases.Merge)
            {
                InitMergeCalcTable(groupId);
            }
            else
            {
                switch (calculationInfo.Method)
                {
                    case Methods.ED:
                        InitEDTable(groupNum, groupId);
                        break;
                    case Methods.Direct:
                        InitDirectTable(groupNum, groupId);
                        break;
                    default:
                        InitDefaultTable(groupNum, groupId);
                        break;
                }
            }

            Table.CalcColumnWidthInPixel();
        }

        private void InitDefaultTable(int groupNum, int groupId)
        {
            Table = new TableDesc
            {
                Category = TableCategory.PotencyEstimateTable,
                IsSeparator = true,
                IsShown = true,
                RowNum = TableHeaders.PEVerticalTH[_lang].Length,
                ColNum = ConstStrings.PotencyEstThirdLineString[_lang].Length + 1,
                DefaultColumnSizeInPixel = 110,
                DataStartRow = 3,
                DataStartCol = 1,
                Cells = new List<List<TableCellBase>> { new List<TableCellBase>() }
            };

            //第一行，表头：实验组
            Table.Cells[0].Add(new TableCellBase());
            Table.Cells[0][0].SetValues(0, 0, 1, 4, true, TableHeaders.PEVerticalTH[_lang][0]);
            //第二行：组别，T、U...
            Table.Cells.Add(new List<TableCellBase>());
            Table.Cells[1].Add(new TableCellBase());
            Table.Cells[1][0].SetValues(1, 0, 1, 1, true, TableHeaders.PEVerticalTH[_lang][1]);
            Table.Cells[1].Add(new TableCellBase());
            Table.Cells[1][1].SetValues(1, 1, 1, 3, true, ConstStrings.GetGroupIdString(groupNum, groupId));
            //第三行：单位
            Table.Cells.Add(new List<TableCellBase>());
            Table.Cells[2].Add(new TableCellBase());
            Table.Cells[2][0].SetValues(2, 0, 1, 1, false, TableHeaders.PEVerticalTH[_lang][2]);
            for (int i = 0; i < ConstStrings.PotencyEstThirdLineString[_lang].Length; i++)
            {
                Table.Cells[2].Add(new TableCellBase());
                Table.Cells[2][i + 1].SetValues(2, i + 1, 1, 1, true, ConstStrings.PotencyEstThirdLineString[_lang][i]);
            }
            //第四-六行，效价、Rel. to Ass、Rel. to Est
            for (int i = 3; i < Table.RowNum - 2; i++)
            {
                Table.Cells.Add(new List<TableCellBase>());
                for (int j = 0; j < Table.ColNum; j++)
                {
                    Table.Cells[i].Add(new TableCellBase());
                    String header = j == 0 ? TableHeaders.PEVerticalTH[_lang][i] : "";
                    Table.Cells[i][j].SetValues(i, j, 1, 1, true, header);
                }
            }
            //第七行，Sm
            Table.Cells.Add(new List<TableCellBase>());
            Table.Cells[6].Add(new TableCellBase());
            Table.Cells[6][0].SetValues(6, 0, 1, 1, true, TableHeaders.PEVerticalTH[_lang][6]);
            Table.Cells[6].Add(new TableCellBase());
            Table.Cells[6][1].SetValues(6, 1, 1, 3, true);

            //第八行，可信限率
            Table.Cells.Add(new List<TableCellBase>());
            Table.Cells[7].Add(new TableCellBase());
            Table.Cells[7][0].SetValues(7, 0, 1, 1, true, TableHeaders.PEVerticalTH[_lang][7]);
            Table.Cells[7].Add(new TableCellBase());
            Table.Cells[7][1].SetValues(7, 1, 1, 3, true);
        }

        private void InitEDTable(int groupNum, int groupId)
        {
            Table = new TableDesc
            {
                Category = TableCategory.PotencyEstimateTable,
                IsSeparator = true,
                IsShown = true,
                RowNum = TableHeaders.EDPEVerticalTH[_lang].Length,
                ColNum = ConstStrings.PotencyEstThirdLineString[_lang].Length + 1,
                DefaultColumnSizeInPixel = 110,
                DataStartRow = 3,
                DataStartCol = 1,
                Cells = new List<List<TableCellBase>> { new List<TableCellBase>() }
            };

            //第一行，表头：实验组
            Table.Cells[0].Add(new TableCellBase());
            Table.Cells[0][0].SetValues(0, 0, 1, 4, true, TableHeaders.EDPEVerticalTH[_lang][0]);
            //第二行：组别，T、U...
            Table.Cells.Add(new List<TableCellBase>());
            Table.Cells[1].Add(new TableCellBase());
            Table.Cells[1][0].SetValues(1, 0, 1, 1, true, TableHeaders.EDPEVerticalTH[_lang][1]);
            Table.Cells[1].Add(new TableCellBase());
            Table.Cells[1][1].SetValues(1, 1, 1, 3, true, ConstStrings.GetGroupIdString(groupNum, groupId));
            //第三行
            Table.Cells.Add(new List<TableCellBase>());
            Table.Cells[2].Add(new TableCellBase());
            Table.Cells[2][0].SetValues(2, 0, 1, 1, false, TableHeaders.EDPEVerticalTH[_lang][2]);
            for (int i = 0; i < ConstStrings.PotencyEstThirdLineString[_lang].Length; i++)
            {
                Table.Cells[2].Add(new TableCellBase());
                Table.Cells[2][i + 1].SetValues(2, i + 1, 1, 1, true, ConstStrings.PotencyEstThirdLineString[_lang][i]);
            }

            //第四-七行，log ED、ED、//Rel. to Ass、Rel. to Est
            for (int i = 3; i < Table.RowNum - 1; i++)
            {
                Table.Cells.Add(new List<TableCellBase>());
                for (int j = 0; j < Table.ColNum; j++)
                {
                    Table.Cells[i].Add(new TableCellBase());
                    Table.Cells[i][j].SetValues(i, j, 1, 1, true, TableHeaders.EDPEVerticalTH[_lang][i]);
                }
            }
            //第八行，Sm
            Table.Cells.Add(new List<TableCellBase>());
            Table.Cells[5].Add(new TableCellBase());
            Table.Cells[5][0].SetValues(5, 0, 1, 1, true, TableHeaders.EDPEVerticalTH[_lang][5]);
            Table.Cells[5].Add(new TableCellBase());
            Table.Cells[5][1].SetValues(5, 1, 1, 3, true);
        }

        private void InitDirectTable(int groupNum, int groupId)
        {
            Table = new TableDesc
            {
                Category = TableCategory.PotencyEstimateTable,
                IsSeparator = true,
                IsShown = true,
                RowNum = TableHeaders.DirectPEVerticalTH[_lang].Length,
                ColNum = ConstStrings.PotencyEstThirdLineString[_lang].Length + 1,
                DefaultColumnSizeInPixel = 110,
                DataStartRow = 3,
                DataStartCol = 1,
                Cells = new List<List<TableCellBase>> { new List<TableCellBase>() }
            };

            //第一行，表头：实验组
            Table.Cells[0].Add(new TableCellBase());
            Table.Cells[0][0].SetValues(0, 0, 1, 4, true, TableHeaders.DirectPEVerticalTH[_lang][0]);
            //第二行：组别，T、U...
            Table.Cells.Add(new List<TableCellBase>());
            Table.Cells[1].Add(new TableCellBase());
            Table.Cells[1][0].SetValues(1, 0, 1, 1, true, TableHeaders.DirectPEVerticalTH[_lang][1]);
            Table.Cells[1].Add(new TableCellBase());
            Table.Cells[1][1].SetValues(1, 1, 1, 3, true, ConstStrings.GetGroupIdString(groupNum, groupId));
            //第三行：单位
            Table.Cells.Add(new List<TableCellBase>());
            Table.Cells[2].Add(new TableCellBase());
            Table.Cells[2][0].SetValues(2, 0, 1, 1, false, TableHeaders.DirectPEVerticalTH[_lang][2]);
            for (int i = 0; i < ConstStrings.PotencyEstThirdLineString[_lang].Length; i++)
            {
                Table.Cells[2].Add(new TableCellBase());
                Table.Cells[2][i + 1].SetValues(2, i + 1, 1, 1, true, ConstStrings.PotencyEstThirdLineString[_lang][i]);
            }
            //第四行，效价
            Table.Cells.Add(new List<TableCellBase>());
            for (int j = 0; j < Table.ColNum; j++)
            {
                Table.Cells[3].Add(new TableCellBase());
                String header = j == 0 ? TableHeaders.DirectPEVerticalTH[_lang][3] : "";
                Table.Cells[3][j].SetValues(3, j, 1, 1, true, header);
            }
            //第五行，Sm
            Table.Cells.Add(new List<TableCellBase>());
            Table.Cells[4].Add(new TableCellBase());
            Table.Cells[4][0].SetValues(4, 0, 1, 1, true, TableHeaders.DirectPEVerticalTH[_lang][4]);
            Table.Cells[4].Add(new TableCellBase());
            Table.Cells[4][1].SetValues(4, 1, 1, 3, true);

            //第六行，可信限率
            Table.Cells.Add(new List<TableCellBase>());
            Table.Cells[5].Add(new TableCellBase());
            Table.Cells[5][0].SetValues(5, 0, 1, 1, true, TableHeaders.DirectPEVerticalTH[_lang][5]);
            Table.Cells[5].Add(new TableCellBase());
            Table.Cells[5][1].SetValues(5, 1, 1, 3, true);
        }

        private void InitMergeCalcTable(int groupId)
        {
            Table = new TableDesc
            {
                Category = TableCategory.PotencyEstimateTable,
                IsSeparator = true,
                IsShown = true,
                RowNum = TableHeaders.MergeCalcPEVerticalTH[_lang].Length,
                ColNum = ConstStrings.PotencyEstThirdLineString[_lang].Length + 1,
                DefaultColumnSizeInPixel = 110,
                DataStartRow = 3,
                DataStartCol = 1,
                Cells = new List<List<TableCellBase>> { new List<TableCellBase>() }
            };

            //第一行，表头：实验组
            Table.Cells[0].Add(new TableCellBase());
            Table.Cells[0][0].SetValues(0, 0, 1, 4, true, TableHeaders.MergeCalcPEVerticalTH[_lang][0]);
            //第二行：合并类型
            Table.Cells.Add(new List<TableCellBase>());
            Table.Cells[1].Add(new TableCellBase());
            Table.Cells[1][0].SetValues(1, 0, 1, 1, true, TableHeaders.MergeCalcPEVerticalTH[_lang][1]);
            Table.Cells[1].Add(new TableCellBase());
            Table.Cells[1][1].SetValues(1, 1, 1, 3, true, ConstStrings.MergeCalcTypes[_lang][groupId]);
            //第三行：单位
            Table.Cells.Add(new List<TableCellBase>());
            Table.Cells[2].Add(new TableCellBase());
            Table.Cells[2][0].SetValues(2, 0, 1, 1, false, TableHeaders.MergeCalcPEVerticalTH[_lang][2]);
            for (int i = 0; i < ConstStrings.PotencyEstThirdLineString[_lang].Length; i++)
            {
                Table.Cells[2].Add(new TableCellBase());
                Table.Cells[2][i + 1].SetValues(2, i + 1, 1, 1, true, ConstStrings.PotencyEstThirdLineString[_lang][i]);
            }
            //第四-六行，效价、Rel. to Ass、Rel. to Est
            for (int i = 3; i < Table.RowNum - 2; i++)
            {
                Table.Cells.Add(new List<TableCellBase>());
                for (int j = 0; j < Table.ColNum; j++)
                {
                    Table.Cells[i].Add(new TableCellBase());
                    String header = j == 0 ? TableHeaders.MergeCalcPEVerticalTH[_lang][i] : "";
                    Table.Cells[i][j].SetValues(i, j, 1, 1, true, header);
                }
            }
            //第七行，Sm
            Table.Cells.Add(new List<TableCellBase>());
            Table.Cells[6].Add(new TableCellBase());
            Table.Cells[6][0].SetValues(6, 0, 1, 1, true, TableHeaders.MergeCalcPEVerticalTH[_lang][6]);
            Table.Cells[6].Add(new TableCellBase());
            Table.Cells[6][1].SetValues(6, 1, 1, 3, true);

            //第八行，可信限率
            Table.Cells.Add(new List<TableCellBase>());
            Table.Cells[7].Add(new TableCellBase());
            Table.Cells[7][0].SetValues(7, 0, 1, 1, true, TableHeaders.MergeCalcPEVerticalTH[_lang][7]);
            Table.Cells[7].Add(new TableCellBase());
            Table.Cells[7][1].SetValues(7, 1, 1, 3, true);
        }
    }
}
