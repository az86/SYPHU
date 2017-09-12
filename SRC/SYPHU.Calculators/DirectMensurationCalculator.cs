using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using SYPHU.Assay.CalculationInfo;
using SYPHU.Assay.Results.PotencyEstimateResults;
using SYPHU.Assay.Results.VarianceAnalysisResults;
using SYPHU.Assay.Tables;
using SYPHU.Utilities;

namespace SYPHU.Calculators
{
    /// <summary>
    /// 直接检定法，groupNum=Column, doseNum = 1, assayNum = Row
    /// </summary>
    public class DirectMensurationCalculator : AbstractCalculator
    {
        /// <summary>
        /// 直接法方差分析结果
        /// </summary>
        private readonly DirectVarAnaResult _directVarAnaResult = new DirectVarAnaResult();

        /// <summary>
        /// 直接法效价估计结果
        /// </summary>
        private readonly DirectPEResult _directPEResult = new DirectPEResult();

        /// <summary>
        /// 直接法方差分析表格
        /// </summary>
        public readonly DirectVarAnaTable VarAnaTable = new DirectVarAnaTable();

        public override void DoCalculate(InitCalculationInfo calculationInfo, int extremeAbnormalDataNum, List<List<double>> bList)
        {
            CalcInfo = calculationInfo;
            ExtremeAbnormalDataNum = extremeAbnormalDataNum;
            VarianceAnalysis();
            ConfidenceLimitCalc();
        }

        public override void CreateTable()
        {
            CreateVATable();
            CreatePETable();
        }

        public override void CreatePlotInfo(int  h)
        {
        }

        #region 填充表格内容

        private void CreateVATable()
        {
            VarAnaTable.InitTable(CalcInfo.Lang);
            FillVATable();
        }

        private void CreatePETable()
        {
            for (int i = 0; i < CalcInfo.DataSize.PreparationNum - 1; i++)
            {
                PETableList.Add(new PotencyEstimateTable());
                PETableList[i].InitTable(CalcInfo.DataSize.PreparationNum, i + 1, CalcInfo);
            }
            FillPETable();
        }

        private void FillVATable()
        {
            VarAnaTable.Table.Cells[1][0].Content = _directVarAnaResult.FreedomDegree.ToString(CultureInfo.InvariantCulture);
            VarAnaTable.Table.Cells[1][1].Content = _directVarAnaResult.t.ToString("F4");
        }

        private void FillPETable()
        {
            for (int i = 0; i < CalcInfo.DataSize.PreparationNum - 1; i++)
            {
                if (CalcInfo.UnitA[i + 1].Unit != "")
                {
                    PETableList[i].Table.Cells[3][0].Content += "(" + CalcInfo.UnitA[i + 1].Unit + ")";
                }
                FillPEValues(i, PETableList[i].Table.DataStartRow, _directPEResult.PEValues[i].Potency, "F4");
                PETableList[i].Table.Cells[PETableList[i].Table.DataStartRow + 1][1].Content =
                    _directPEResult.PEValues[i].SM.ToString("F10");
                PETableList[i].Table.Cells[PETableList[i].Table.DataStartRow + 2][1].Content =
                    _directPEResult.PEValues[i].CLPercent.ToString("P2");
            }
        }

        #endregion

        #region 私有算法

        private void VarianceAnalysis()
        {
            _directVarAnaResult.FreedomDegree = 0;
            _directVarAnaResult.s2 = 0;
            for (int i = 0; i < CalcInfo.DataSize.PreparationNum; i++)
            {
                _directVarAnaResult.FreedomDegree += AssData.Data[i][0].Freedom();
                _directVarAnaResult.s2 += AssData.Data[i][0].Error();
            }
            //自由度需要减去异常值的个数
            _directVarAnaResult.FreedomDegree -= ExtremeAbnormalDataNum;
            _directVarAnaResult.s2 /= _directVarAnaResult.FreedomDegree;
            _directVarAnaResult.t = Distributions.Dist_t(_directVarAnaResult.FreedomDegree);
        }

        private void ConfidenceLimitCalc()
        {
            for (int i = 0; i < CalcInfo.DataSize.PreparationNum - 1; i++)
            {
                _directPEResult.MList.Add(AssData.Data[0][0].Average() - AssData.Data[i+1][0].Average());
                _directPEResult.PEValues.Add(new TreatPEValues());
                _directPEResult.PEValues[i].SM = Math.Sqrt(_directVarAnaResult.s2*2.0/CalcInfo.DataSize.ReplicateNum);
                _directPEResult.PEValues[i].Potency.Lower = CalcInfo.UnitA[i+1].Val/CalcInfo.UnitA[0].Val*
                                                            VectorCalcMethodExt.Antilg(_directPEResult.MList[i] -
                                                                                       _directVarAnaResult.t*
                                                                                       _directPEResult.PEValues[i].SM);
                _directPEResult.PEValues[i].Potency.Est = CalcInfo.UnitA[i+1].Val/CalcInfo.UnitA[0].Val*
                                                          VectorCalcMethodExt.Antilg(_directPEResult.MList[i]);
                _directPEResult.PEValues[i].Potency.Upper = CalcInfo.UnitA[i+1].Val / CalcInfo.UnitA[0].Val *
                                            VectorCalcMethodExt.Antilg(_directPEResult.MList[i] +
                                                                       _directVarAnaResult.t *
                                                                       _directPEResult.PEValues[i].SM);
            }
        }

        protected override void CalcSM(int i, double secItem)
        {
            
        }

        #endregion
    }
}
