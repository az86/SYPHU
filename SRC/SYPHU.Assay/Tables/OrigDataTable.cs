using System;
using System.Collections.Generic;
using System.Linq;
using SYPHU.Assay.CalculationInfo;
using SYPHU.Assay.Data;
using SYPHU.Utilities;

namespace SYPHU.Assay.Tables
{
    /// <summary>
    /// 原始数据表格类
    /// </summary>
    [Serializable]
    public class OrigDataTable
    {
        public List<DataTable> Tables;

        [NonSerialized] 
        public AssayData OrigData;

        public void CreateTable(InitCalculationInfo calculationInfo)
        {
            Tables = new List<DataTable>();
            for (int i = 0; i < calculationInfo.DataSize.PreparationNum; i++)
            {
                Tables.Add(new DataTable());
                if (calculationInfo.CalcCase == CalcCases.Single)
                {
                    int groupId = (calculationInfo.Method == Methods.ED) ? i + 1 : i;

                    Tables[i].InitTable(calculationInfo, groupId, true);
                    Tables[i].Table.Category = TableCategory.OrigDataTable;
                    Tables[i].Table.IsSeparator = (i == calculationInfo.DataSize.PreparationNum - 1);
                    Tables[i].Table.IsShown = true;
                    Tables[i].Table.DataColNum = calculationInfo.DataSize.DoseNum;
                    Tables[i].Table.DataRowNum = calculationInfo.DataSize.ReplicateNum;
                    Tables[i].Table.IsSingleCalcTable = calculationInfo.CalcCase == CalcCases.Single;
                    Tables[i].Table.Cells[0][0].Content = ConstStrings.DataTableCategoryString[calculationInfo.Lang][Tables[i].Table.Category] + "--" + Tables[i].Table.Cells[0][0].Content;
                }
                else
                {
                    Tables[i].InitTable(calculationInfo, i, true);
                    Tables[i].Table.Category = TableCategory.OrigDataTable;
                    Tables[i].Table.IsSeparator = (i == calculationInfo.DataSize.PreparationNum - 1);
                    Tables[i].Table.IsShown = true;
                    Tables[i].Table.DataColNum = calculationInfo.DataSize.DoseNum;
                    Tables[i].Table.DataRowNum = calculationInfo.DataSize.ReplicateNum;
                    Tables[i].Table.IsSingleCalcTable = calculationInfo.CalcCase == CalcCases.Single;
                    Tables[i].Table.Cells[0][0].Content = ConstStrings.DataTableCategoryString[calculationInfo.Lang][Tables[i].Table.Category];
                }
            }
        }

        /// <summary>
        /// 解析
        /// </summary>
        public String Parse(InitCalculationInfo calculationInfo)
        {
            if (calculationInfo == null)
            {
                return "参数初始化失败，未进行参数设置.";
            }
            String errorMsg;
            try
            {
                errorMsg = ParseUnitA(calculationInfo);
            }
            catch (Exception)
            {
                return "效价值解析失败.";
            }

            if (errorMsg != null)
            {
                return errorMsg;
            }
            if (calculationInfo.CalcCase == CalcCases.Single)
            {
                try
                {
                    errorMsg = ParseUnitD(calculationInfo);
                }
                catch (Exception)
                {
                    return "剂量值解析失败.";
                }

                if (errorMsg != null)
                {
                    return errorMsg;
                }
                if (calculationInfo.Method != Methods.Direct)
                {
                    try
                    {
                        errorMsg = ParseUnitDil(calculationInfo);
                    }
                    catch (Exception)
                    {
                        return "稀释倍数解析失败.";
                    }

                    if (errorMsg != null)
                    {
                        return errorMsg;
                    }
                }
            }

            OrigData = new AssayData();
            try
            {
                OrigData.InitData(calculationInfo.DataSize);
            }
            catch (Exception)
            {
                return "原始数据初始化解析失败.";
            }
            try
            {
                errorMsg = ParseAssayData(calculationInfo);
            }
            catch (Exception)
            {
                return "原始数据解析失败.";
            }
            
            if (errorMsg != null)
            {
                return errorMsg;
            }
            return null;
        }

        /// <summary>
        /// 解析计算参数A
        /// </summary>
        private String ParseUnitA(InitCalculationInfo calculationInfo)
        {
            calculationInfo.UnitA = new List<UnitData>();
            for (int i = 0; i < calculationInfo.DataSize.PreparationNum; i++)
            {
                calculationInfo.UnitA.Add(new UnitData(true, 1));
                string errorMsg = calculationInfo.UnitA[i].Parse(calculationInfo.CalcCase == CalcCases.Single ? Tables[i].Table.Cells[2][1].Content : Tables[i].Table.Cells[0][2].Content);

                if (errorMsg != null)
                {
                    return "标示效价值" + errorMsg;
                }
                if (Math.Abs(calculationInfo.UnitA[i].Val) < ConstantsExt.Eps())
                {
                    return "标示效价不能为0";
                }
                if (Tables[i].Table.Cells[2][1].Content.Trim() != "" && calculationInfo.UnitA[i].Val < 0)
                {
                    return "标示效价不能小于0";
                }
            }
            return null;
        }

        /// <summary>
        /// 解析计算参数d
        /// </summary>
        private String ParseUnitD(InitCalculationInfo calculationInfo)
        {
            if (calculationInfo.CalcCase == CalcCases.Merge)
            {
                return null;
            }
            calculationInfo.Unitd = new List<List<UnitData>>();
            for (int i = 0; i < calculationInfo.DataSize.PreparationNum; i++)
            {
                calculationInfo.Unitd.Add(new List<UnitData>());
                for (int isplit = 0; isplit <= Tables[i].SplitNum; isplit++)
                {
                    for (int j = 0; j < Tables[i].Table.ColNum - 1; j++)
                    {
                        if (calculationInfo.Unitd[i].Count < calculationInfo.DataSize.DoseNum)
                        {
                            calculationInfo.Unitd[i].Add(new UnitData());
                            String errorMsg = calculationInfo.Unitd[i][j + isplit * (Tables[i].Table.ColNum - 1)].Parse(Tables[i].Table.Cells[4 + (calculationInfo.DataSize.ReplicateNum + 1) * isplit][j + 1].Content);
                            if (errorMsg != null)
                            {
                                return "剂量值" + errorMsg;
                            }
                            if (Math.Abs(calculationInfo.Unitd[i][j + isplit * (Tables[i].Table.ColNum - 1)].Val) < ConstantsExt.Eps())
                            {
                                return "剂量值不能为0";
                            }
                            if (Tables[i].Table.Cells[4][j + 1].Content.Trim() != "" && calculationInfo.Unitd[i][j + isplit * (Tables[i].Table.ColNum - 1)].Val < 0)
                            {
                                return "剂量值不能小于0";
                            }
                        }
                    }
                }
            }
            return DoseValueChecher(calculationInfo);
        }

        /// <summary>
        /// 检测dose值输入的正确性
        /// </summary>
        /// <param name="calculationInfo"></param>
        /// <returns></returns>
        private static String DoseValueChecher(InitCalculationInfo calculationInfo)
        {
            var compareValue = new List<double>();
            if (calculationInfo.CalcCase == CalcCases.Merge)
            {
                return null;
            }
            switch (calculationInfo.Method)
            {
                case Methods.ParallelLine:
                    switch (calculationInfo.Design)
                    {
                        case Designs.CrossOver:
                            compareValue.Add(calculationInfo.Unitd[1][0].Val / calculationInfo.Unitd[0][0].Val);
                            compareValue.Add(calculationInfo.Unitd[0][1].Val / calculationInfo.Unitd[1][1].Val);
                            compareValue.Add(calculationInfo.Unitd[2][1].Val / calculationInfo.Unitd[3][1].Val);
                            compareValue.Add(calculationInfo.Unitd[3][0].Val / calculationInfo.Unitd[2][0].Val);
                            break;
                        default:
                            for (int i = 0; i < calculationInfo.DataSize.PreparationNum; i++)
                            {
                                for (int j = 0; j < calculationInfo.DataSize.DoseNum - 1; j++)
                                {
                                    compareValue.Add(calculationInfo.Unitd[i][j + 1].Val / calculationInfo.Unitd[i][j].Val);
                                }
                            }
                            break;
                    }
                    if (compareValue.Any(d => Math.Abs(d - compareValue[0]) > ConstantsExt.Eps(-6)))
                    {
                        return "平行线法剂量比应该相等.";
                    }
                    return null;
                case Methods.SlopeRatio:
                    for (int i = 0; i < calculationInfo.DataSize.PreparationNum; i++)
                    {
                        compareValue.Clear();
                        for (int j = 0; j < calculationInfo.DataSize.DoseNum - 1; j++)
                        {
                            compareValue.Add(calculationInfo.Unitd[i][j + 1].Val - calculationInfo.Unitd[i][j].Val);
                        }
                        if (compareValue.Any(d => Math.Abs(d - compareValue[0]) > ConstantsExt.Eps()))
                        {
                            return "斜率比法剂量差应该相等.";
                        }
                    }
                    return null;
                default:
                    return null;
            }
        }

        /// <summary>
        /// 解析稀释倍数
        /// </summary>
        /// <param name="calculationInfo"></param>
        private String ParseUnitDil(InitCalculationInfo calculationInfo)
        {
            calculationInfo.UnitDil = new List<UnitData>();
            for (int i = 0; i < calculationInfo.DataSize.PreparationNum; i++)
            {
                calculationInfo.UnitDil.Add(new UnitData(true));
                String errorMsg = calculationInfo.UnitDil[i].Parse(Tables[i].Table.Cells[3][1].Content);
                if (errorMsg != null)
                {
                    return "稀释倍数" + errorMsg;
                }
                if (Math.Abs(calculationInfo.UnitDil[i].Val) < ConstantsExt.Eps())
                {
                    return "稀释倍数不能为0";
                }
                if (Tables[i].Table.Cells[3][1].Content.Trim() != "" && calculationInfo.UnitDil[i].Val < 0)
                {
                    return "稀释倍数不能小于0";
                }
            }
            return null;
        }

        /// <summary>
        /// 解析数据
        /// </summary>
        private String ParseAssayData(InitCalculationInfo calculationInfo)
        {
            var rpn = new ReversePolishNotation();
            for (int i = 0; i < calculationInfo.DataSize.PreparationNum; i++)
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
                                String curData =
                                    Tables[i].Table.Cells[
                                        Tables[i].Table.DataStartRow + k +
                                        isplit*(calculationInfo.DataSize.ReplicateNum + 1)][
                                            Tables[i].Table.DataStartCol + j]
                                        .Content;
                                if (curData == "")
                                {
                                    return "试验数据不能为空.";
                                }
                                if (calculationInfo.CalcCase == CalcCases.Merge)
                                {
                                    if (rpn.IsValid(curData) && rpn.Parse())
                                    {
                                        OrigData.Data[i][curCol][k] = rpn.Evaluate();
                                        ParsePrecision(calculationInfo, curData, curCol);
                                    }
                                    else
                                    {
                                        return "输入的试验数据无效.";
                                    }
                                }
                                else
                                {
                                    if (calculationInfo.Type == Types.Quantal)
                                    {
                                        int val;
                                        if (Int32.TryParse(curData, out val))
                                        {
                                            OrigData.Data[i][curCol][k] = val;
                                        }
                                        else
                                        {
                                            return "定性反应的输入数据应为整数值.";
                                        }
                                    }
                                    else if (calculationInfo.Type == Types.Graded)
                                    {
                                        if (rpn.IsValid(curData) && rpn.Parse())
                                        {
                                            OrigData.Data[i][curCol][k] = rpn.Evaluate();
                                            ParsePrecision(calculationInfo, curData, curCol);
                                        }
                                        else
                                        {
                                            return "输入的试验数据无效.";
                                        }
                                    }
                                    if (OrigData.Data[i][curCol][k] < 0)
                                    {
                                        return "试验数据不能小于0.";
                                    }
                                }
                            }
                        }
                        curCol++;
                    }
                }
            }
            return AssayDataValueChecker(OrigData, calculationInfo);
        }

        private String AssayDataValueChecker(AssayData data, InitCalculationInfo calculationInfo)
        {
            if (calculationInfo.CalcCase == CalcCases.Merge)
            {
                if (calculationInfo.DataSize.DoseNum == 5)
                {
                    for (int i = 0; i < calculationInfo.DataSize.ReplicateNum; i++)
                    {
                        if (data.Data[0][1][i] > data.Data[0][2][i])
                        {
                            return "用于合并计算的下限不应高于效价值.";
                        }
                        if (data.Data[0][2][i] > data.Data[0][3][i])
                        {
                            return "用于合并计算的效价值不应高于上限.";
                        }
                    }
                }
                return null;
            }

            switch (calculationInfo.Type)
            {
                case Types.Quantal:
                    if (calculationInfo.DataSize.ReplicateNum != 2)
                    {
                        return "定性反应的数据行数应为2.";
                    }
                    for (int i = 0; i < calculationInfo.DataSize.PreparationNum; i++)
                    {
                        for (int j = 0; j < calculationInfo.DataSize.DoseNum; j++)
                        {
                            //定性反应第一行为试验次数、第二行为频数
                            if (data.Data[i][j][0] < data.Data[i][j][1])
                            {
                                return "定性反应第一行为试验次数、第二行为频数，频数应不大于试验次数.";
                            }
                        }
                    }
                    OrigData.Precision = 1;
                    return null;
                default:
                    return null;
            }
        }

        private void ParsePrecision(InitCalculationInfo calculationInfo, String curData, int curCol)
        {
            if (calculationInfo.CalcCase == CalcCases.Single
                || (calculationInfo.CalcCase == CalcCases.Merge && calculationInfo.DataSize.DoseNum == 5 && curCol == 2)
                || (calculationInfo.CalcCase == CalcCases.Merge && calculationInfo.DataSize.DoseNum == 4 && curCol == 1))
            {
                double curPrecision = VectorCalcMethodExt.Precision(curData);
                if (OrigData.Precision > curPrecision)
                {
                    OrigData.Precision = curPrecision;
                }
            }
        }
    }
}
