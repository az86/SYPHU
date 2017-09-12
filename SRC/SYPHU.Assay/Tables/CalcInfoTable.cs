using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using SYPHU.Assay.CalculationInfo;

namespace SYPHU.Assay.Tables
{
    /// <summary>
    /// 计算信息表格
    /// </summary>
    [Serializable]
    public class CalcInfoTable
    {
        public TableDesc Table;

        public void CreateTable(InitCalculationInfo calculationInfo)
        {
            InitTable(calculationInfo);
            FillValues(calculationInfo);
        }

        private void InitTable(InitCalculationInfo calculationInfo)
        {
            String[] calcInfoVerticalTH = calculationInfo.CalcCase == CalcCases.Single
                                              ? TableHeaders.SingleCalcInfoVerticalTH[calculationInfo.Lang]
                                              : TableHeaders.MergeCalcInfoVerticalTH[calculationInfo.Lang];
            Table = new TableDesc
                {
                    Category = TableCategory.CalcInfoTable,
                    IsShown = true,
                    IsSeparator = true,
                    IsAutoAdjustment = true,
                    DefaultColumnSizeInPixel = 50,
                    RowNum = calcInfoVerticalTH.Count(),
                    ColNum = 2,
                    DataStartRow = 0,
                    DataStartCol = 1,
                    Cells = new List<List<TableCellBase>>()
                };
            for (int i = 0; i < Table.RowNum; i++)
            {
                Table.Cells.Add(new List<TableCellBase>());
                Table.Cells[i].Add(new TableCellBase());
                Table.Cells[i][0].SetValues(i, 0, 1, 1, true, calcInfoVerticalTH[i]);
                Table.Cells[i].Add(new TableCellBase());
            }
        }

        private void FillValues(InitCalculationInfo calculationInfo)
        {
            Table.Cells[0][1].SetValues(0,1,1,1,true, calculationInfo.ProductName);
            String methodString = (calculationInfo.Method == Methods.ED)
                                      ? calculationInfo.EDString + " " + calculationInfo.EDPercent
                                      : ConstStrings.MethodsStringDict[calculationInfo.Lang][calculationInfo.Method];
            Table.Cells[1][1].SetValues(1, 1, 1, 1, true, methodString);
            if (calculationInfo.CalcCase == CalcCases.Single)
            {
                Table.Cells[2][1].SetValues(2, 1, 1, 1, true, ConstStrings.DesignsStringDict[calculationInfo.Lang][calculationInfo.Design]);
                Table.Cells[3][1].SetValues(3, 1, 1, 1, true, ConstStrings.ModelsStringDict[calculationInfo.Lang][calculationInfo.Model]);
                Table.Cells[4][1].SetValues(4, 1, 1, 1, true, ConstStrings.TypesStringDict[calculationInfo.Lang][calculationInfo.Type]);
                string formula = "y' = " + (calculationInfo.DataTransFormula == DataTransformationFormula.UserDefined
                                     ? calculationInfo.UserDefinedFormula
                                     : ConstStrings.FormulasStringDict[calculationInfo.DataTransFormula]);
                Table.Cells[5][1].SetValues(5, 1, 1, 1, true, formula);
                Table.Cells[6][1].SetValues(6, 1, 1, 1, true, ConstStrings.Undetermined[calculationInfo.Lang]);
            }
            else if (calculationInfo.CalcCase == CalcCases.Merge)
            {
                string formula = "y' = " + (calculationInfo.DataTransFormula == DataTransformationFormula.UserDefined
                     ? calculationInfo.UserDefinedFormula
                     : ConstStrings.FormulasStringDict[calculationInfo.DataTransFormula]);
                Table.Cells[2][1].SetValues(2, 1, 1, 1, true, formula);
                Table.Cells[3][1].SetValues(3, 1, 1, 1, true, ConstStrings.Undetermined[calculationInfo.Lang]);
            }
            Table.CalcColumnWidthInPixel();
        }

        /// <summary>
        /// 修改异常值检测方法显示内容
        /// </summary>
        /// <param name="calculationInfo"></param>
        public void UpdateAbnDataCheckMethod(InitCalculationInfo calculationInfo)
        {
            if (calculationInfo.CalcCase == CalcCases.Single)
            {
                Table.Cells[6][1].Content = ConstStrings.CheckMethodsStringDict[calculationInfo.Lang][calculationInfo.AbnDataCheckMethods[0]];
            }
            else if (calculationInfo.CalcCase == CalcCases.Merge)
            {
                Table.Cells[3][1].Content = ConstStrings.CheckMethodsStringDict[calculationInfo.Lang][calculationInfo.AbnDataCheckMethods[0]];
            }
            Table.CalcColumnWidthInPixel();
        }
    }
}
