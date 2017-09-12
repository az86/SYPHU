using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using SYPHU.Assay.CalculationInfo;
using SYPHU.Assay.Plots;
using SYPHU.Assay.Results.PotencyEstimateResults;
using SYPHU.Assay.Results.ReliabilityCheck;
using SYPHU.Assay.Results.VarianceAnalysisResults;
using SYPHU.Assay.Tables;
using SYPHU.Utilities;

namespace SYPHU.Calculators
{
    public class EDCalculator : SCQuantalCalculator
    {
        private readonly EDVarAnaResult VAResult = new EDVarAnaResult();

        public List<PEResult> PEResultList;

        public bool IsLimitCurvePlotted = true;

        #region 重写M、V算法

        private void CalcMList(int iList)
        {
            IterResult.MList = new List<double>();
            double p = CalcInfo.EDPercentList[iList] / 100.0;
            for (int i = 0; i < CalcInfo.DataSize.PreparationNum; i++)
            {
                IterResult.MList.Add(CalcM(p, i));
            }
        }

        private double CalcM(double p, int i)
        {
            switch (CalcInfo.Model)
            {
                case Models.Probit:
                    return (Distributions.Dist_Phi_Inv(p) - IterResult.aList[i])/IterResult.b;
                case Models.Logit:
                    return (Math.Log(p/(1.0 - p)) - IterResult.aList[i])/IterResult.b;
                case Models.Gompit:
                    return (Math.Log(Math.Log(1.0/(1.0 - p))) - IterResult.aList[i])/IterResult.b;
                case Models.Angle:
                    return (Math.Asin(2*p - 1.0) - IterResult.aList[i])/IterResult.b;
                default:
                    return 0.0;
            }
        }

        private void CalcV()
        {
            IterResult.V = new List<double>();
            IterResult.V.Add(1.0/IterResult.wList.Sum());
        }

        #endregion
        
        #region 重写可信限计算

        protected override void PECalc()
        {
            CalcC();
            CalcV();
            PEResultList = new List<PEResult>();
            for (int iList = 0; iList < CalcInfo.EDPercentList.Count; iList++)
            {
                CalcMList(iList);

                PEResults = new PEResult();
                PEResults.PEValues = new List<TreatPEValues>();
                for (int i = 0; i < CalcInfo.DataSize.PreparationNum; i++)
                {
                    PEResults.PEValues.Add(new TreatPEValues());
                    //计算效价
                    double cm = IterResult.C * IterResult.MList[i];
                    double firItem = cm - (IterResult.C - 1) * IterResult.xAveList[i];
                    double secItem =
                        Math.Sqrt((IterResult.C - 1) *
                                  (IterResult.V[i] * IterResult.SxxList.Sum() +
                                   IterResult.C * (IterResult.MList[i] - IterResult.xAveList[i]) *
                                   (IterResult.MList[i] - IterResult.xAveList[i])));

                    if (CalcInfo.UnitDil[i].Val > 0)
                    {
                        PEResults.PEValues[i].Potency.Upper = (-Math.Log(CalcInfo.UnitDil[i].Val) - firItem + secItem) / Math.Log(10);
                        PEResults.PEValues[i].Potency.Est = (-Math.Log(CalcInfo.UnitDil[i].Val) - IterResult.MList[i]) / Math.Log(10);
                        PEResults.PEValues[i].Potency.Lower = (-Math.Log(CalcInfo.UnitDil[i].Val) - firItem - secItem) / Math.Log(10);
                    }
                    else
                    {
                        PEResults.PEValues[i].Potency.Upper = (firItem + secItem) / Math.Log(10);
                        PEResults.PEValues[i].Potency.Est = IterResult.MList[i] / Math.Log(10);
                        PEResults.PEValues[i].Potency.Lower = (firItem - secItem) / Math.Log(10);
                    }
                    CalcSM(i, secItem);
                }
                PEResultList.Add(PEResults);
            }
        }

        #endregion

        #region 重写方差分析计算

        protected override void CalcFreedom()
        {
            VAResult.RegValues.FreedomDegree = 1;
            if (CalcInfo.DataSize.DoseNum > 2)
            {
                VAResult.LinValues.FreedomDegree = CalcInfo.DataSize.PreparationNum * (CalcInfo.DataSize.DoseNum - 2);
            }
            
            VAResult.TreatValues.FreedomDegree = CalcInfo.DataSize.PreparationNum*CalcInfo.DataSize.DoseNum - 1;
            //VAResult.ResValues.FreedomDegree = CalcInfo.DataSize.PreparationNum*CalcInfo.DataSize.DoseNum*(nValue - 1);
            VAResult.TotalValues.FreedomDegree = CalcInfo.DataSize.PreparationNum*CalcInfo.DataSize.DoseNum - 1;
        }

        protected override void CalcSquareSum()
        {
            VAResult.RegValues.SquareSum = IterResult.SxyList.Sum()*IterResult.SxyList.Sum()/IterResult.SxxList.Sum();
            if (CalcInfo.DataSize.DoseNum > 2)
            {
                VAResult.LinValues.SquareSum = 0;
                for (int i = 0; i < CalcInfo.DataSize.PreparationNum; i++)
                {
                    VAResult.LinValues.SquareSum += IterResult.SyyList[i] -
                                                       IterResult.SxyList[i] * IterResult.SxyList[i] / IterResult.SxxList[i];
                }
            }

            VAResult.TreatValues.SquareSum = IterResult.SyyList.Sum();
            VAResult.TotalValues.SquareSum = 0;
            for (int i = 0; i < IterResult.yList.Count; i++)
            {
                VAResult.TotalValues.SquareSum += (IterResult.yList[i] - IterResult.yAveList[i/CalcInfo.DataSize.DoseNum])*
                                                  (IterResult.yList[i] - IterResult.yAveList[i/CalcInfo.DataSize.DoseNum])*IterResult.wList[i];
            }
            //VAResult.ResValues.SquareSum = VAResult.TotalValues.SquareSum - VAResult.TreatValues.SquareSum;
        }

        protected override void CalcFValues()
        {
            VAResult.RegValues.FValue = VAResult.RegValues.SquareSum;
            if (CalcInfo.DataSize.DoseNum > 2)
            {
                VAResult.LinValues.FValue = VAResult.LinValues.SquareSum;
            }
            
            VAResult.TreatValues.FValue = VAResult.TreatValues.SquareSum;
            //VAResult.ResValues.FValue = VAResult.ResValues.SquareSum;
            VAResult.TotalValues.FValue = VAResult.TotalValues.SquareSum;
        }

        protected override void CalcPValues()
        {
            VAResult.RegValues.PValue = Distributions.Dist_X2(VAResult.RegValues.FreedomDegree,
                                                               VAResult.RegValues.FValue);
            if (CalcInfo.DataSize.DoseNum > 2)
            {
                VAResult.LinValues.PValue = Distributions.Dist_X2(VAResult.LinValues.FreedomDegree,
                                                              VAResult.LinValues.FValue);
            }
            VAResult.TreatValues.PValue = Distributions.Dist_X2(VAResult.TreatValues.FreedomDegree,
                                                               VAResult.TreatValues.FValue);
        }

        #endregion

        protected override void DoReliabilityCheck()
        {
            VariationList.Clear();
            VariationList.Add(ConstStrings.VariationSourcesDict[CalcInfo.Lang][VariationSources.Reg], VAResult.RegValues.PValue);

            if (CalcInfo.DataSize.DoseNum > 2)
            {
                VariationList.Add(ConstStrings.VariationSourcesDict[CalcInfo.Lang][VariationSources.Lin], VAResult.LinValues.PValue);
            }

            ReliabilityChecker.DoCheck(VariationList, CalcInfo.Lang);
        }

        public override void CreateTable()
        {
            VATable.InitTable(CalcInfo);
            FillVATable();
            ConclusionTable.CreateTable(ReliabilityChecker.ConclusionList, ReliabilityChecker.FinalConclusion, CalcInfo.Lang);

            for (int i = 0; i < CalcInfo.EDPercentList.Count; i++)
            {
                for (int j = 0; j < CalcInfo.DataSize.PreparationNum; j++)
                {
                    PETableList.Add(new PotencyEstimateTable());
                    PETableList[i+j].InitTable(CalcInfo.DataSize.PreparationNum, j + 1, CalcInfo);
                }
                FillPETable(i);
            }
        }

        public override void CreatePlotInfo(int h)
        {
            base.CreatePlotInfo(h);

            #region 上下限曲线

            if (IsLimitCurvePlotted)
            {
                AddLimitCurveToPlotInfo(CalcLimitCurve());
            }

            #endregion

        }

        private List<List<double>> CalcLimitCurve()
        {
            var limits = new List<List<double>>();
            //lower
            limits.Add(new List<double>());
            //upper
            limits.Add(new List<double>());
            //prob
            limits.Add(new List<double>());

            for (int i = 1; i < 50; i++)
            {
                limits[2].Add(0.0002 * i);
            }
            for (int i = 1; i <= 99; i++)
            {
                limits[2].Add(i / 100.0);
            }
            for (int i = 1; i < 50; i++)
            {
                limits[2].Add(0.99 + 0.0002 * i);
            }

            for (int i = 0; i < limits[2].Count; i++)
            {
                double m = CalcM(limits[2][i], 0);
                //计算效价
                double cm = IterResult.C * m;
                double firItem = cm - (IterResult.C - 1) * IterResult.xAveList[0];
                double secItem =
                    Math.Sqrt((IterResult.C - 1) *
                              (IterResult.V[0] * IterResult.SxxList.Sum() +
                               IterResult.C * (m - IterResult.xAveList[0]) *
                               (m - IterResult.xAveList[0])));
                if (CalcInfo.UnitDil[0].Val > 0)
                {
                    limits[0].Add(-firItem - secItem);
                    limits[1].Add(-firItem + secItem);
                }
                else
                {
                    limits[0].Add(firItem - secItem);
                    limits[1].Add(firItem + secItem);
                }
            }
            return limits;
        }

        private void AddLimitCurveToPlotInfo(List<List<double>> limits)
        {
            //左限
            PlotsInfo[0].CurveDescs.Add(new CurveDesc());
            //右限
            PlotsInfo[0].CurveDescs.Add(new CurveDesc());
            double offset = Math.Log(CalcInfo.UnitA[0].Val * CalcInfo.UnitDil[0].Val);
            for (int i = 0; i < limits[0].Count; i++)
            {
                double xPointLower = limits[0][i];
                double xPointUpper = limits[1][i];

                if (CalcInfo.UseDilutionsTimes)
                {
                    xPointLower -= offset;
                    xPointUpper -= offset;
                    xPointLower *= -1;
                    xPointUpper *= -1;
                }
                PlotsInfo[0].CurveDescs[1].FitPoints.Add(new Point(xPointLower, limits[2][i]));
                PlotsInfo[0].CurveDescs[2].FitPoints.Add(new Point(xPointUpper, limits[2][i]));
            }

            PlotsInfo[0].CurveDescs[1].Color = PlotsInfo[0].CurveDescs[0].Color;
            PlotsInfo[0].CurveDescs[2].Color = PlotsInfo[0].CurveDescs[0].Color;

            PlotsInfo[0].CurveDescs[1].PlotType = PlotTypes.DashedLine;
            PlotsInfo[0].CurveDescs[2].PlotType = PlotTypes.DashedLine;

            //重新计算x范围
            PlotsInfo[0].XRange = limits[1].Max() - limits[0].Min();

        }

        #region 填充表格

        private void FillVATable()
        {
            FillVAValues(1, VAResult.RegValues);
            if (CalcInfo.DataSize.DoseNum <= 2)
            {
                FillVAValues(2, VAResult.TreatValues);
                VATable.Table.Cells[3][3].Content = "1.00000";
                FillVAValues(4, VAResult.TotalValues, 3);
            }
            else
            {
                FillVAValues(2, VAResult.LinValues);
                FillVAValues(3, VAResult.TreatValues);
                VATable.Table.Cells[4][3].Content = "1.00000";
                FillVAValues(5, VAResult.TotalValues, 3);
            }
        }

        private void FillPETable(int iList)
        {
            for (int i = 0; i < CalcInfo.DataSize.PreparationNum; i++)
            {
                String unit = CalcInfo.UnitA[i].Unit != "" ? "(" + CalcInfo.UnitA[i + 1].Unit + ")" : "";
                PETableList[i+iList].Table.Cells[3][0].Content = "Log10 " + CalcInfo.EDString + CalcInfo.EDPercentList[iList].ToString(CultureInfo.InvariantCulture) + unit;
                PETableList[i + iList].Table.Cells[4][0].Content = CalcInfo.EDString + CalcInfo.EDPercentList[iList].ToString(CultureInfo.InvariantCulture) + unit;
                FillPEValues(i+iList, PETableList[i + iList].Table.DataStartRow, PEResultList[iList].PEValues[i].Potency, "F4");
                var edValue = new BasicPEValues
                    {
                        Lower = VectorCalcMethodExt.Antilg(PEResultList[iList].PEValues[i].Potency.Lower),
                        Est = VectorCalcMethodExt.Antilg(PEResultList[iList].PEValues[i].Potency.Est),
                        Upper = VectorCalcMethodExt.Antilg(PEResultList[iList].PEValues[i].Potency.Upper)
                    };
                FillPEValues(i+iList, PETableList[i + iList].Table.DataStartRow + 1, edValue, "F4");
                PETableList[i + iList].Table.Cells[PETableList[i + iList].Table.DataStartRow + 2][1].Content = PEResultList[iList].PEValues[0].SM.ToString("F10");
            }
        }

        #endregion

        protected override void CalcSM(int i, double secItem)
        {
            PEResults.PEValues[0].SM = secItem / 1.96 / Math.Log(10);
        }
    }
}
