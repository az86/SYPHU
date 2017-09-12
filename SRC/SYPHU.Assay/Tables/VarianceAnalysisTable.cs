using System;
using System.Collections.Generic;
using System.Globalization;
using SYPHU.Assay.CalculationInfo;
using SYPHU.Assay.Results.ReliabilityCheck;

namespace SYPHU.Assay.Tables
{
    /// <summary>
    /// 方差分析表格
    /// </summary>
    public class VarianceAnalysisTable
    {
        public TableDesc Table;

        /// <summary>
        /// 横向表头
        /// </summary>
        private List<String> _horizontalStringList; 

        /// <summary>
        /// 纵向表头
        /// </summary>
        private List<String> _verticalStringList;

        public void InitTable(InitCalculationInfo calculationInfo)
        {
            Table = new TableDesc();
            GetHorizontalTH(calculationInfo);
            GetVerticalTH(calculationInfo);
            Table.Category = TableCategory.VarianceAnalysisTable;
            Table.IsSeparator = true;
            Table.IsShown = true;
            Table.RowNum = _verticalStringList.Count + 1;
            Table.ColNum = _horizontalStringList.Count;
            Table.DefaultColumnSizeInPixel = 95;

            // 英文输出时自动调整
            Table.IsAutoAdjustment = calculationInfo.Lang == OutLang.English;

            Table.DataStartRow = 1;
            Table.DataStartCol = 1;
            Table.Cells = new List<List<TableCellBase>>();
            //第一行表头
            Table.Cells.Add(new List<TableCellBase>());
            for (int i = 0; i < Table.ColNum; i++)
            {
                Table.Cells[0].Add(new TableCellBase());
                Table.Cells[0][i].SetValues(0, i, 1, 1, true, _horizontalStringList[i]);
            }
            //方差分析数据
            for (int i = 1; i < Table.RowNum; i++)
            {
                Table.Cells.Add(new List<TableCellBase>());
                for (int j = 0; j < Table.ColNum; j++)
                {
                    Table.Cells[i].Add(new TableCellBase());
                    String header = j == 0 ? _verticalStringList[i - 1] : "";
                    Table.Cells[i][j].SetValues(i, j, 1, 1, true, header);
                }
            }
            Table.CalcColumnWidthInPixel();
        }

        private void GetHorizontalTH(InitCalculationInfo calculationInfo)
        {
            _horizontalStringList = new List<string>();
            switch (calculationInfo.Method)
            {
                case Methods.SigmoidCurve:
                    foreach (var t in TableHeaders.SCVarAnaHorizontalTH[calculationInfo.Lang])
                    {
                        _horizontalStringList.Add(t);
                    }
                    break;
                case Methods.ED:
                    goto case Methods.SigmoidCurve;
                default:
                    foreach (var t in TableHeaders.VarAnaHorizontalTH[calculationInfo.Lang])
                    {
                        _horizontalStringList.Add(t);
                    }
                    break;
            }
        }

        private void GetVerticalTH(InitCalculationInfo calculationInfo)
        {
            _verticalStringList = new List<string>();
            switch (calculationInfo.Method)
            {
                case Methods.ParallelLine:
                    for (int i = 0; i < TableHeaders.VarAnaVerticalTH[calculationInfo.Lang][calculationInfo.Method][calculationInfo.Design].Length; i++)
                    {
                        _verticalStringList.Add(TableHeaders.VarAnaVerticalTH[calculationInfo.Lang][calculationInfo.Method][calculationInfo.Design][i]);
                    }
                    if (calculationInfo.DataSize.DoseNum <= 2)
                    {
                        _verticalStringList.Remove(ConstStrings.VariationSourcesDict[calculationInfo.Lang][VariationSources.Lin]);
                    }
                    else
                    {
                        for (int i = 0; i < calculationInfo.DataSize.PreparationNum; i++)
                        {
                            String mainTableHeader = i == 0
                                                         ? ConstStrings.DataFirstLineString[calculationInfo.Lang][0]
                                                         : ConstStrings.DataFirstLineString[calculationInfo.Lang][1] + i.ToString(CultureInfo.InvariantCulture);
                            _verticalStringList.Insert(4 + i, "  " + mainTableHeader);
                        }
                    }
                    break;
                case Methods.SlopeRatio:
                    for (int i = 0; i < TableHeaders.VarAnaVerticalTH[calculationInfo.Lang][calculationInfo.Method][calculationInfo.Design].Length; i++)
                    {
                        _verticalStringList.Add(TableHeaders.VarAnaVerticalTH[calculationInfo.Lang][calculationInfo.Method][calculationInfo.Design][i]);
                    }
                    if (calculationInfo.DataSize.DoseNum <= 2)
                    {
                        _verticalStringList.Remove(ConstStrings.VariationSourcesDict[calculationInfo.Lang][VariationSources.Lin]);
                    }
                    else
                    {
                        for (int i = 0; i < calculationInfo.DataSize.PreparationNum; i++)
                        {
                            String mainTableHeader = i == 0
                                                         ? ConstStrings.DataFirstLineString[calculationInfo.Lang][0]
                                                         : ConstStrings.DataFirstLineString[calculationInfo.Lang][1] + i.ToString(CultureInfo.InvariantCulture);
                            _verticalStringList.Insert(3 + i, "  " + mainTableHeader);
                        }
                    }
                    break;
                case Methods.SigmoidCurve:
                    for (int i = 0; i < TableHeaders.VarAnaVerticalTH[calculationInfo.Lang][calculationInfo.Method][calculationInfo.Design].Length; i++)
                    {
                        _verticalStringList.Add(TableHeaders.VarAnaVerticalTH[calculationInfo.Lang][calculationInfo.Method][calculationInfo.Design][i]);
                    }
                    if (calculationInfo.DataSize.DoseNum <= 2)
                    {
                        _verticalStringList.Remove(ConstStrings.VariationSourcesDict[calculationInfo.Lang][VariationSources.Lin]);
                    }
                    else
                    {
                        for (int i = 0; i < calculationInfo.DataSize.PreparationNum; i++)
                        {
                            String mainTableHeader = i == 0
                                                         ? ConstStrings.DataFirstLineString[calculationInfo.Lang][0]
                                                         : ConstStrings.DataFirstLineString[calculationInfo.Lang][1] + i.ToString(CultureInfo.InvariantCulture);
                            _verticalStringList.Insert(4 + i, "  " + mainTableHeader);
                        }
                    }
                    
                    break;
                case Methods.ED:
                    for (int i = 0; i < TableHeaders.VarAnaVerticalTH[calculationInfo.Lang][calculationInfo.Method][calculationInfo.Design].Length; i++)
                    {
                        _verticalStringList.Add(TableHeaders.VarAnaVerticalTH[calculationInfo.Lang][calculationInfo.Method][calculationInfo.Design][i]);
                    }
                    break;
            }
        }
    }
}