using System;
using System.Collections.Generic;
using System.Globalization;
using SYPHU.Assay.CalculationInfo;
using SYPHU.Assay.Data;

namespace SYPHU.Assay.Tables
{
    /// <summary>
    /// 单个数据表格
    /// </summary>
    [Serializable]
    public class DataTable
    {
        /// <summary>
        /// 数据表头
        /// </summary>
        public TableDesc Table;

        public int SplitNum { get; set; }

        public int SurplusNum { get; set; }

        public void InitTable(InitCalculationInfo calculationInfo, int groupId, bool isUserInputData, int extremeAbnormalDataNum = 0)
        {
            SplitNum = (calculationInfo.DataSize.DoseNum - 1) / TableDesc.MaxDataCol;
            SurplusNum = calculationInfo.DataSize.DoseNum % TableDesc.MaxDataCol;

            if (calculationInfo.CalcCase == CalcCases.Single)
            {
                InitSingleCalcTable(calculationInfo, groupId, isUserInputData);
            }
            else //if (calculationInfo.CalcCase == CalcCases.Merge)
            {
                InitMergeCalcTable(calculationInfo, isUserInputData, extremeAbnormalDataNum);
            }
        }

        private void InitSingleCalcTable(InitCalculationInfo calculationInfo, int groupId, bool isUserInputData)
        {
            Table = new TableDesc();
            Table.RowNum = calculationInfo.DataSize.ReplicateNum + TableHeaders.DataVerticalTH[calculationInfo.Lang].Length + SplitNum*(calculationInfo.DataSize.ReplicateNum + 1);
            Table.ColNum = calculationInfo.DataSize.DoseNum > TableDesc.MaxDataCol
                               ? (TableDesc.MaxDataCol + 1)
                               : (calculationInfo.DataSize.DoseNum + 1);
            Table.DataRowNum = calculationInfo.DataSize.ReplicateNum;
            Table.DataColNum = calculationInfo.DataSize.DoseNum;

            Table.DataStartRow = TableHeaders.DataVerticalTH[calculationInfo.Lang].Length;
            Table.DataStartCol = 1;
            Table.Cells = new List<List<TableCellBase>>();
            Table.IsAutoAdjustment = calculationInfo.Lang == OutLang.English;
            for (int i = 0; i < TableHeaders.DataVerticalTH[calculationInfo.Lang].Length - 1; i++)
            {
                Table.Cells.Add(new List<TableCellBase>());
                Table.Cells[i].Add(new TableCellBase());
                Table.Cells[i][0].SetValues(i, 0, 1, 1, true, TableHeaders.DataVerticalTH[calculationInfo.Lang][i]);
            }
            #region 第一行，表头：Standard、Sample 1、Sample 2、……

            String mainTableHeader;
            if (calculationInfo.Design == Designs.CrossOver)
            {
                mainTableHeader = ConstStrings.GetGroupString(groupId + 1)[calculationInfo.Lang];
            }
            else
            {
                mainTableHeader = groupId == 0
                                      ? ConstStrings.DataFirstLineString[calculationInfo.Lang][0]
                                      : ConstStrings.DataFirstLineString[calculationInfo.Lang][1] +
                                        groupId.ToString(CultureInfo.InvariantCulture);
            }
            Table.Cells[0][0].SetValues(0, 0, 1, Table.ColNum, true, mainTableHeader);

            #endregion

            #region 第二行，Id值

            if (calculationInfo.Design == Designs.CrossOver)
            {
                Table.Cells[1].Add(new TableCellBase());
                Table.Cells[1][1].SetValues(1, 1, 1, 1, true, ConstStrings.CrossOverDesignIdString[groupId*2]);
                Table.Cells[1].Add(new TableCellBase());
                Table.Cells[1][2].SetValues(1, 2, 1, 1, true, ConstStrings.CrossOverDesignIdString[groupId*2 + 1]);
            }
            else
            {
                Table.Cells[1].Add(new TableCellBase());
                Table.Cells[1][1].SetValues(1, 1, 1, Table.ColNum -1, true,
                                            ConstStrings.GetGroupIdString(calculationInfo.DataSize.PreparationNum, groupId));
            }

            #endregion

            #region 第三行，A值

            Table.Cells[2].Add(new TableCellBase());
            Table.Cells[2][1].SetValues(2, 1, 1, Table.ColNum-1, !isUserInputData);

            #endregion

            #region 第四行，稀释倍数

            Table.Cells[3].Add(new TableCellBase());
            Table.Cells[3][1].SetValues(3, 1, 1, Table.ColNum-1, !isUserInputData);

            #endregion


            for (int isplit = 0; isplit <= SplitNum; isplit++)
            {
                //第五行，d值
                Table.Cells.Add(new List<TableCellBase>());
                Table.Cells[4 + (calculationInfo.DataSize.ReplicateNum + 1) * isplit].Add(new TableCellBase());
                Table.Cells[4 + (calculationInfo.DataSize.ReplicateNum + 1) * isplit][0].SetValues(4 + (calculationInfo.DataSize.ReplicateNum + 1) * isplit, 0, 1, 1, true, TableHeaders.DataVerticalTH[calculationInfo.Lang][4]);
                for (int i = 0; i < Table.ColNum - 1; i++)
                {
                    Table.Cells[4 + (calculationInfo.DataSize.ReplicateNum + 1) * isplit].Add(new TableCellBase());
                    bool isValid = (i + isplit * TableDesc.MaxDataCol < calculationInfo.DataSize.DoseNum);
                    Table.Cells[4 + (calculationInfo.DataSize.ReplicateNum + 1) * isplit][i + 1].SetValues(4 + (calculationInfo.DataSize.ReplicateNum + 1) * isplit, i + 1, 1, 1, !isUserInputData || !isValid);
                }
                //实验数据
                for (int i = 0; i < calculationInfo.DataSize.ReplicateNum; i++)
                {
                    Table.Cells.Add(new List<TableCellBase>());
                    for (int j = 0; j < Table.ColNum; j++)
                    {
                        Table.Cells[i + 5 + (calculationInfo.DataSize.ReplicateNum + 1) * isplit].Add(new TableCellBase());
                        if (j == 0)
                        {
                            Table.Cells[i + 5 + (calculationInfo.DataSize.ReplicateNum + 1) * isplit][j].SetValues(i + 5 + (calculationInfo.DataSize.ReplicateNum + 1) * isplit, j, 1, 1, true, "(" + Convert.ToString(i + 1) + ")");
                        }
                        else
                        {
                            bool isValid = (j - 1 + isplit * TableDesc.MaxDataCol < calculationInfo.DataSize.DoseNum);
                            Table.Cells[i + 5 + (calculationInfo.DataSize.ReplicateNum + 1) * isplit][j].SetValues(i + 5 + (calculationInfo.DataSize.ReplicateNum + 1) * isplit, j, 1, 1, !isUserInputData || !isValid);
                        }
                    }
                }
            }

            Table.CalcColumnWidthInPixel();
        }

        private void InitMergeCalcTable(InitCalculationInfo calculationInfo, bool isUserInputData, int extremeAbnormalDataNum)
        {
            Table = new TableDesc
            {
                RowNum = calculationInfo.DataSize.ReplicateNum + 2 - extremeAbnormalDataNum,
                ColNum = calculationInfo.DataSize.DoseNum + 1,
                DataStartRow = 2,
                DataStartCol = 1,
                DataRowNum = calculationInfo.DataSize.ReplicateNum,
                DataColNum = calculationInfo.DataSize.DoseNum,
                Cells = new List<List<TableCellBase>>(),
                IsAutoAdjustment = calculationInfo.Lang == OutLang.English
            };
            //第一行，原始数据表
            Table.Cells.Add(new List<TableCellBase>());
            Table.Cells[0].Add(new TableCellBase());
            Table.Cells[0][0].SetValues(0, 0, 1, 2, true);
            Table.Cells[0].Add(new TableCellBase());
            Table.Cells[0][1].SetValues(0, 2, 1, 1, true, TableHeaders.DataVerticalTH[calculationInfo.Lang][2]);
            Table.Cells[0].Add(new TableCellBase());
            Table.Cells[0][2].SetValues(0, 3, 1, Table.ColNum - 3, !isUserInputData);

            //第二行，表头
            Table.Cells.Add(new List<TableCellBase>());
            String[] tableHeader;
            if (calculationInfo.DataSize.DoseNum == 5)
            {
                tableHeader = TableHeaders.MergeCalcFullHorizontalTH[calculationInfo.Lang];
            }
            else //if (calculationInfo.DataSize.DoseNum == 4)
            {
                tableHeader = TableHeaders.MergeCalcSmHorizontalTH[calculationInfo.Lang];
            }
            for (int i = 0; i < tableHeader.Length; i++)
            {
                Table.Cells[1].Add(new TableCellBase());
                Table.Cells[1][i].SetValues(1, i, 1, 1, true, tableHeader[i]);
            }

            //数据
            for (int i = 0; i < calculationInfo.DataSize.ReplicateNum - extremeAbnormalDataNum; i++)
            {
                Table.Cells.Add(new List<TableCellBase>());
                for (int j = 0; j < Table.ColNum; j++)
                {
                    Table.Cells[i + 2].Add(new TableCellBase());
                    if (j == 0)
                    {
                        Table.Cells[i + 2][j].SetValues(i + 2, j, 1, 1, true, (i + 1).ToString(CultureInfo.InvariantCulture));
                    }
                    else
                    {
                        Table.Cells[i + 2][j].SetValues(i + 2, j, 1, 1, !isUserInputData);
                    }
                }
            }
            Table.CalcColumnWidthInPixel();
        }
    }
}
