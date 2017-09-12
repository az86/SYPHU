using System;
using System.Collections.Generic;
using SYPHU.Assay.CalculationInfo;
using SYPHU.Assay.Data;
using SYPHU.Assay.Plots;
using SYPHU.Assay.Tables;

namespace SYPHU.Calculators
{
    public class EDFullCalculator
    {
        private EDCalculator _calculator;

        public List<Models> ModelList = new List<Models> {Models.Probit, Models.Logit, Models.Gompit, Models.Angle};

        public PlotInfo PlotsInfo;

        /// <summary>
        /// 试验数据（校正后）
        /// </summary>
        private AssayData _assData = new AssayData();

        /// <summary>
        /// 计算信息
        /// </summary>
        private InitCalculationInfo _calculationInfo = new InitCalculationInfo();

        public void LoadCalcData(AssayData assData)
        {
            _assData = assData;
        }

        public void DoFullCalculate(InitCalculationInfo calculationInfo)
        {
            _calculationInfo = calculationInfo;
            DoSeperatorCalc();
        }

        private void DoSeperatorCalc()
        {
            PlotsInfo = new PlotInfo();
            PlotsInfo.CurveDescs = new List<CurveDesc>();
            double xRange = 0;
            double yRange = 0;
            for (int i = 0; i < ModelList.Count; i++)
            {
                _calculationInfo.Model = ModelList[i];
                _calculator = new EDCalculator();
                _calculator.IsLimitCurvePlotted = false;
                _calculator.LoadCalcData(_assData);
                _calculator.DoCalculate(_calculationInfo, 0, null);
                _calculator.CreatePlotInfo(_calculationInfo.DataSize.PreparationNum);
                PlotsInfo.DataPoints = _calculator.PlotsInfo[0].DataPoints;
                PlotsInfo.CurveDescs.Add(new CurveDesc());
                PlotsInfo.CurveDescs[i].CurveEquation = _calculationInfo.Model.ToString();
                PlotsInfo.CurveDescs[i].FitPoints =
                    _calculator.PlotsInfo[0].CurveDescs[0].FitPoints;
                PlotsInfo.CurveDescs[i].PlotType = PlotTypes.SolidLine;
                PlotsInfo.CurveDescs[i].Color = ConstStrings.ColorList[i];
                if (_calculator.PlotsInfo[0].XRange > xRange)
                {
                    xRange = _calculator.PlotsInfo[0].XRange;
                }
                if (_calculator.PlotsInfo[0].YRange > yRange)
                {
                    yRange = _calculator.PlotsInfo[0].YRange;
                }
            }
            String header = _calculationInfo.EDString + ConstStrings.CompFigure[_calculationInfo.Lang];
            PlotsInfo.SetValues(header, _calculator.PlotsInfo[0].XLabel,
                                _calculator.PlotsInfo[0].YLabel, xRange, yRange);
        }
    }
}
