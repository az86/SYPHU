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
    /// <summary>
    /// 斜率比法
    /// </summary>
    public class SlopeRatioMensurationCalculator : AbstractCalculator
    {
        private double _d;
        private double _h;
        private double _n;

        /// <summary>
        /// 方差分析结果
        /// </summary>
        protected SlopeRatioVarAnaResult VAResult = new SlopeRatioVarAnaResult();

        /// <summary>
        /// 效价估计结果
        /// </summary>
        protected readonly SlopeRatioPEResult PEResult = new SlopeRatioPEResult();

        /// <summary>
        /// 拉丁方B值
        /// </summary>
        protected List<List<double>> BList;

        /// <summary>
        /// 稀释倍数校正系数
        /// </summary>
        private List<double> _corrValue;

        public override void DoCalculate(InitCalculationInfo calculationInfo, int extremeAbnormalDataNum, List<List<double>> bList)
        {
            CalcInfo = calculationInfo;
            _d = CalcInfo.DataSize.DoseNum;
            _h = CalcInfo.DataSize.PreparationNum;
            _n = CalcInfo.DataSize.ReplicateNum;
            ExtremeAbnormalDataNum = extremeAbnormalDataNum;
            BList = bList;
            VarAnaPreCalc();
            VarAnaCalc();
            PEPreCalc();
            PECalc();
        }

        public override void CreateTable()
        {
            VATable.InitTable(CalcInfo);
            for (int i = 0; i < CalcInfo.DataSize.PreparationNum - 1; i++)
            {
                PETableList.Add(new PotencyEstimateTable());
                PETableList[i].InitTable(CalcInfo.DataSize.PreparationNum, i + 1, CalcInfo);
            }
            FillVATable();
            FillPETable();
            ConclusionTable.CreateTable(ReliabilityChecker.ConclusionList, ReliabilityChecker.FinalConclusion, CalcInfo.Lang);
        }

        public override void CreatePlotInfo(int h)
        {
            #region 计算a、b

            //计算散点X值
            var dList = new List<List<double>>();
            var xList = new List<double>();
            for (int i = 0; i < h; i++)
            {
                dList.Add(new List<double>());
                for (int j = 0; j < CalcInfo.DataSize.DoseNum; j++)
                {
                    dList[i].Add(CalcInfo.UnitDil.Any(t => Math.Abs(t.Val + 1) < ConstantsExt.Eps())
                                     ? CalcInfo.Unitd[i][j].Val
                                     : CalcInfo.Unitd[i][j].Val * CalcInfo.UnitA[i].Val * CalcInfo.UnitDil[i].Val);
                }
                xList.Add(dList[i].Average());
            }

            //计算散点Y值
            var yList = new List<double>();
            for (int i = 0; i < h; i++)
            {
                yList.Add(VAResult.SampleAverage[i].Average());
            }

            //斜率比法，公共的a，计算每组试验的b
            var bList = new List<double>();
            for (int i = 0; i < h; i++)
            {
                bList.Add((VAResult.SampleAverage[i].Average() - VAResult.a) / dList[i].Average());
            }

            #endregion

            #region 虚线

            List<double> bDashed = new List<double>();
            List<double> aDashed = new List<double>();

            double d = CalcInfo.DataSize.DoseNum;

            for (int i = 0; i < h; i++)
            {
                aDashed.Add(VAResult.aList[i] / (d * d - d));
                bDashed.Add((VAResult.SampleAverage[i].Average() - aDashed[i]) / dList[i].Average());
            }

            #endregion

            #region Plot

            double yRange = VectorCalcMethodExt.List3DExtremum(AssData.Data, true) -
                            VectorCalcMethodExt.List3DExtremum(AssData.Data, false);

            PlotsInfo = new List<PlotInfo>();
            //所有图的集合
            PlotsInfo.Add(new PlotInfo());

            
            for (int i = 1; i <= CalcInfo.DataSize.PreparationNum; i++)
            {
                //添加拟合直线
                PlotsInfo.Add(new PlotInfo());

                PlotsInfo[i].CurveDescs = new List<CurveDesc>();

                #region 实线信息

                PlotsInfo[i].CurveDescs.Add(new CurveDesc());

                //直线，取起点、终点即可
                PlotsInfo[i].CurveDescs[0].FitPoints.Add(new Point(0.0, VAResult.a));
                PlotsInfo[i].CurveDescs[0].FitPoints.Add(new Point(dList[i - 1][CalcInfo.DataSize.DoseNum - 1],
                                                                   VAResult.a +
                                                                   bList[i - 1]*
                                                                   dList[i - 1][CalcInfo.DataSize.DoseNum - 1]));

                //直线方程
                PlotsInfo[i].CurveDescs[0].CurveEquation = "y' = " + VAResult.a.ToString("F4") +
                                                           (bList[i - 1] < 0 ? "-" : "+") +
                                                           (Math.Abs(bList[i - 1])).ToString("F4") + "*x";

                //颜色
                PlotsInfo[i].CurveDescs[0].Color = ConstStrings.ColorList[(i - 1)%ConstStrings.ColorList.Count()];

                //添加原始数据
                PlotsInfo[i].DataPoints.Add(new PointGroup());
                PlotsInfo[i].DataPoints[0].Color = PlotsInfo[i].CurveDescs[0].Color;
                for (int j = 0; j < CalcInfo.DataSize.DoseNum; j++)
                {
                    PlotsInfo[i].DataPoints[0].Points.Add(new List<Point>());
                    for (int k = 0; k < CalcInfo.DataSize.ReplicateNum; k++)
                    {
                        PlotsInfo[i].DataPoints[0].Points[j].Add(new Point(dList[i - 1][j], AssData.Data[i - 1][j][k]));
                    }
                }

                #endregion
                
                #region 虚线信息

                PlotsInfo[i].CurveDescs.Add(new CurveDesc());

                //直线，取起点、终点即可
                PlotsInfo[i].CurveDescs[1].FitPoints.Add(new Point(0.0, aDashed[i-1]));
                PlotsInfo[i].CurveDescs[1].FitPoints.Add(new Point(dList[i - 1][CalcInfo.DataSize.DoseNum - 1],
                                                                   aDashed[i-1] +
                                                                   bDashed[i - 1] *
                                                                   dList[i - 1][CalcInfo.DataSize.DoseNum - 1]));

                //直线方程
                PlotsInfo[i].CurveDescs[1].CurveEquation = "1";

                //颜色
                PlotsInfo[i].CurveDescs[1].Color = ConstStrings.ColorList[(i - 1) % ConstStrings.ColorList.Count()];

                //虚线
                PlotsInfo[i].CurveDescs[1].PlotType = PlotTypes.DashedLine;

                #endregion

                #region 图像信息

                PlotsInfo[i].SetValues(ConstStrings.GetGroupIdString(h, i - 1),
                                       ConstStrings.GetXLabel(DataTransformationFormula.Null, CalcInfo.UseDilutionsTimes),
                                       ConstStrings.GetYLabel(CalcInfo.Type),
                                       dList[i - 1].Max(), yRange);

                #endregion
            }

            //总图

            #region 合并子图信息

            PlotsInfo[0].CurveDescs = new List<CurveDesc>();
            double xRange = 0;
            for (int i = 1; i <= h; i++)
            {
                PlotsInfo[0].DataPoints.Add(PlotsInfo[i].DataPoints[0]);
                PlotsInfo[0].CurveDescs.Add(PlotsInfo[i].CurveDescs[0]);

                if (xRange < PlotsInfo[i].XRange)
                {
                    xRange = PlotsInfo[i].XRange;
                }
            }
            PlotsInfo[0].SetValues("All Samples",
                                   ConstStrings.GetXLabel(DataTransformationFormula.Null, CalcInfo.UseDilutionsTimes),
                                   ConstStrings.GetYLabel(CalcInfo.Type), xRange, yRange);

            #endregion

            #endregion
        }

        #region 填充表格

        protected virtual void FillVATable()
        {
        }

        private void FillPETable()
        {
            for (int i = 0; i < CalcInfo.DataSize.PreparationNum - 1; i++)
            {
                if (CalcInfo.UnitA[i + 1].Unit != "")
                {
                    PETableList[i].Table.Cells[3][0].Content += "(" + CalcInfo.UnitA[i + 1].Unit + ")";
                }
                FillPEValues(i, PETableList[i].Table.DataStartRow, PEResult.PEValues[i].Potency, "F4");
                FillPEValues(i, PETableList[i].Table.DataStartRow + 1, PEResult.PEValues[i].RelToAss);
                FillPEValues(i, PETableList[i].Table.DataStartRow + 2, PEResult.PEValues[i].RelToEst);
                PETableList[i].Table.Cells[PETableList[i].Table.DataStartRow + 3][1].Content = PEResult.PEValues[i].SM.ToString("F10");
                PETableList[i].Table.Cells[PETableList[i].Table.DataStartRow + 4][1].Content =
                    PEResult.PEValues[i].CLPercent.ToString("P2");
            }
        }
        
        #endregion

        #region 方差分析预计算内容

        /// <summary>
        ///     方差分析预计算
        /// </summary>
        private void VarAnaPreCalc()
        {
            CalcAverage();
            CalcAverageSum();
            CalcL();
            CalcaList();
            CalcbList();
            CalcGList();
            CalcJList();
            CalcH();
            Calca();
            CalcK();
        }

        private void CalcAverage()
        {
            VAResult.SampleAverage = new List<List<double>>(CalcInfo.DataSize.PreparationNum);
            for (int i = 0; i < CalcInfo.DataSize.PreparationNum; i++)
            {
                VAResult.SampleAverage.Add(new List<double>(CalcInfo.DataSize.DoseNum));
                for (int j = 0; j < CalcInfo.DataSize.DoseNum; j++)
                {
                    VAResult.SampleAverage[i].Add(AssData.Data[i][j].Average());
                }
            }
        }

        private void CalcAverageSum()
        {
            VAResult.SampleAverageSum = new List<double>(CalcInfo.DataSize.PreparationNum);
            for (int i = 0; i < CalcInfo.DataSize.PreparationNum; i++)
            {
                VAResult.SampleAverageSum.Add(VAResult.SampleAverage[i].Sum());
            }
        }

        private void CalcL()
        {
            VAResult.L = new List<double>(CalcInfo.DataSize.PreparationNum);
            for (int i = 0; i < CalcInfo.DataSize.PreparationNum; i++)
            {
                double tmp = 0.0;
                for (int j = 0; j < CalcInfo.DataSize.DoseNum; j++)
                {
                    tmp += VAResult.SampleAverage[i][j]*Convert.ToDouble(j + 1);
                }
                VAResult.L.Add(tmp);
            }
        }

        private void CalcaList()
        {
            VAResult.aList = new List<double>();
            for (int i = 0; i < CalcInfo.DataSize.PreparationNum; i++)
            {
                VAResult.aList.Add((4*_d + 2)*VAResult.SampleAverageSum[i] -
                                       6*VAResult.L[i]);
            }
        }

        private void CalcbList()
        {
            VAResult.bList = new List<double>();
            for (int i = 0; i < CalcInfo.DataSize.PreparationNum; i++)
            {
                VAResult.bList.Add(2*VAResult.L[i] - (_d + 1)*VAResult.SampleAverageSum[i]);
            }
        }

        private void CalcGList()
        {
            VAResult.GList = new List<double>();
            for (int i = 0; i < CalcInfo.DataSize.PreparationNum; i++)
            {
                VAResult.GList.Add(VAResult.SampleAverage[i].SquareSum());
            }
        }

        private void CalcJList()
        {
            VAResult.JList = new List<double>();
            for (int i = 0; i < CalcInfo.DataSize.PreparationNum; i++)
            {
                VAResult.JList.Add(VAResult.GList[i] -
                                       VAResult.SampleAverageSum[i]*VAResult.SampleAverageSum[i]/_d -
                                       3*VAResult.bList[i]*VAResult.bList[i]/(_d*_d*_d - _d));
            }
        }

        private void CalcH()
        {
            VAResult.HB = _n*_h*_d*(_d - 1)/(_d*(_h*_d - _h + 4) + 2);
            VAResult.HI = _n/(2*_d*(2*_d*_d - _d - 1));
        }

        private void Calca()
        {
            VAResult.a = VAResult.aList.Sum()/(_h*(_d*_d - _d));
        }

        private void CalcK()
        {
            VAResult.K = _n * VAResult.SampleAverageSum.Sum() * VAResult.SampleAverageSum.Sum() / (_h * _d);
        }

        #endregion

        #region 方差分析计算

        /// <summary>
        ///     方差分析表格计算
        /// </summary>
        private void VarAnaCalc()
        {
            for (int i = 0; i < CalcInfo.DataSize.PreparationNum; i++)
            {
                VAResult.LinsValues.Add(new BasicVarianceAnalysisValues());
            }
            CalcFreedom();
            CalcSquareSum();
            CalcFValues();
            CalcPValues();
            DoReliabilityCheck();
        }

        protected virtual void CalcFreedom()
        {
            VAResult.IntersValues.FreedomDegree = CalcInfo.DataSize.PreparationNum - 1;
            if (CalcInfo.DataSize.DoseNum > 2)
            {
                VAResult.LinValues.FreedomDegree = CalcInfo.DataSize.PreparationNum *
                                                   (CalcInfo.DataSize.DoseNum - 2);
                for (int i = 0; i < CalcInfo.DataSize.PreparationNum; i++)
                {
                    VAResult.LinsValues[i].FreedomDegree = CalcInfo.DataSize.DoseNum - 2;
                }
            }
            
            VAResult.TreatValues.FreedomDegree = CalcInfo.DataSize.PreparationNum*CalcInfo.DataSize.DoseNum - 1;
            VAResult.RegValues.FreedomDegree = CalcInfo.DataSize.PreparationNum;

            VAResult.TotalValues.FreedomDegree = CalcInfo.DataSize.PreparationNum*CalcInfo.DataSize.DoseNum*
                                                     CalcInfo.DataSize.ReplicateNum - 1 - ExtremeAbnormalDataNum;
        }

        protected virtual void CalcSquareSum()
        {
            VAResult.IntersValues.SquareSum = VAResult.HI*
                                                  (VAResult.aList.SquareSum() -
                                                   _h*(_d*_d - _d)*(_d*_d - _d)*VAResult.a*VAResult.a);

            if (CalcInfo.DataSize.DoseNum > 2)
            {
                VAResult.LinValues.SquareSum = _n * VAResult.JList.Sum();
                for (int i = 0; i < CalcInfo.DataSize.PreparationNum; i++)
                {
                    VAResult.LinsValues[i].SquareSum = _n * VAResult.JList[i];
                }
            }

            VAResult.TreatValues.SquareSum = _n*VAResult.GList.Sum() - VAResult.K;

            VAResult.RegValues.SquareSum = VAResult.TreatValues.SquareSum - VAResult.IntersValues.SquareSum -
                                               VAResult.LinValues.SquareSum;

            VAResult.TotalValues.SquareSum = 0;
            double total = 0;
            for (int i = 0; i < CalcInfo.DataSize.PreparationNum; i++)
            {
                for (int j = 0; j < CalcInfo.DataSize.DoseNum; j++)
                {
                    total += AssData.Data[i][j].Sum();
                }
            }
            double ave = total/(CalcInfo.DataSize.PreparationNum*CalcInfo.DataSize.DoseNum*
                                CalcInfo.DataSize.ReplicateNum);
            for (int i = 0; i < CalcInfo.DataSize.PreparationNum; i++)
            {
                for (int j = 0; j < CalcInfo.DataSize.DoseNum; j++)
                {
                    for (int k = 0; k < CalcInfo.DataSize.ReplicateNum; k++)
                    {
                        VAResult.TotalValues.SquareSum += (AssData.Data[i][j][k] - ave)*
                                                              (AssData.Data[i][j][k] - ave);
                    }
                }
            }
        }

        protected virtual void CalcFValues()
        {
            VAResult.RegValues.FValue = VAResult.RegValues.MeanSquare/VAResult.ResValues.MeanSquare;
            VAResult.IntersValues.FValue = VAResult.IntersValues.MeanSquare/VAResult.ResValues.MeanSquare;
            if (CalcInfo.DataSize.DoseNum > 2)
            {
                VAResult.LinValues.FValue = VAResult.LinValues.MeanSquare/VAResult.ResValues.MeanSquare;
                for (int i = 0; i < CalcInfo.DataSize.PreparationNum; i++)
                {
                    VAResult.LinsValues[i].FValue = VAResult.LinsValues[i].MeanSquare/
                                                    VAResult.ResValues.MeanSquare;
                }
            }

            VAResult.TreatValues.FValue = VAResult.TreatValues.MeanSquare/VAResult.ResValues.MeanSquare;
        }

        protected virtual void CalcPValues()
        {
            VAResult.RegValues.PValue = Distributions.Dist_F(VAResult.RegValues.FreedomDegree,
                                                                 VAResult.ResValues.FreedomDegree,
                                                                 VAResult.RegValues.FValue);
            VAResult.IntersValues.PValue = Distributions.Dist_F(VAResult.IntersValues.FreedomDegree,
                                                                    VAResult.ResValues.FreedomDegree,
                                                                    VAResult.IntersValues.FValue);

            if (CalcInfo.DataSize.DoseNum > 2)
            {
                VAResult.LinValues.PValue = Distributions.Dist_F(VAResult.LinValues.FreedomDegree,
                                                                 VAResult.ResValues.FreedomDegree,
                                                                 VAResult.LinValues.FValue);
                for (int i = 0; i < CalcInfo.DataSize.PreparationNum; i++)
                {
                    VAResult.LinsValues[i].PValue = Distributions.Dist_F(VAResult.LinsValues[i].FreedomDegree,
                                                                         VAResult.ResValues.FreedomDegree,
                                                                         VAResult.LinsValues[i].FValue);
                }
            }
            VAResult.TreatValues.PValue = Distributions.Dist_F(VAResult.TreatValues.FreedomDegree,
                                                                   VAResult.ResValues.FreedomDegree,
                                                                   VAResult.TreatValues.FValue);
        }

        /// <summary>
        /// 可靠性判断
        /// </summary>
        private void DoReliabilityCheck()
        {
            VariationList.Clear();
            VariationList.Add(ConstStrings.VariationSourcesDict[CalcInfo.Lang][VariationSources.Reg], VAResult.RegValues.PValue);
            VariationList.Add(ConstStrings.VariationSourcesDict[CalcInfo.Lang][VariationSources.Inters], VAResult.IntersValues.PValue);

            if (CalcInfo.DataSize.DoseNum > 2)
            {
                VariationList.Add(ConstStrings.VariationSourcesDict[CalcInfo.Lang][VariationSources.Lin], VAResult.LinValues.PValue);
                VariationList.Add(ConstStrings.VariationSourcesDict[CalcInfo.Lang][VariationSources.LinS], VAResult.LinsValues[0].PValue);
                for (int i = 1; i < CalcInfo.DataSize.PreparationNum; i++)
                {
                    VariationList.Add(ConstStrings.VariationSourcesDict[CalcInfo.Lang][VariationSources.LinT] + i.ToString(CultureInfo.InvariantCulture), VAResult.LinsValues[i].PValue);
                }
            }

            ReliabilityChecker.DoCheck(VariationList, CalcInfo.Lang);
        }

        #endregion

        #region 可信限预计算内容

        /// <summary>
        ///     可信限预计算
        /// </summary>
        private void PEPreCalc()
        {
            CalcCorrValue();
            CalcV();
            CalcbListPC();
            CalcR();
            CalcC();
            CalcKPE();
        }

        private void CalcCorrValue()
        {
            _corrValue = new List<double>();
            for (int i = 1; i < _h; i++)
            {
                _corrValue.Add((CalcInfo.Unitd[0][1].Val - CalcInfo.Unitd[0][0].Val)/(CalcInfo.Unitd[i][1].Val - CalcInfo.Unitd[i][0].Val));
            }
        }

        private void CalcV()
        {
            PEResult.V1 = 6/(_n*_d*(2*_d + 1))*(1/(_d + 1) + 3/(_h*(_d - 1)));
            PEResult.V2 = 3*(_d + 1)/(3*(_d + 1) + _h*(_d - 1));
        }

        private void CalcbListPC()
        {
            for (int i = 0; i < _h; i++)
            {
                PEResult.bList.Add((6*VAResult.L[i] - 3*_d*(_d + 1)*VAResult.a)/(2*_d*_d*_d + 3*_d*_d + _d));
            }
        }

        private void CalcR()
        {
            for (int i = 0; i < _h - 1; i++)
            {
                PEResult.RList.Add(PEResult.bList[i + 1]/PEResult.bList[0]);
            }
        }

        private void CalcC()
        {
            double tmp = PEResult.bList[0]*PEResult.bList[0];
            double t = Distributions.Dist_t(VAResult.ResValues.FreedomDegree);
            PEResult.C = tmp/(tmp - VAResult.ResValues.MeanSquare*t*t*PEResult.V1);
        }

        private void CalcKPE()
        {
            PEResult.K = (PEResult.C - 1)*PEResult.V2;
        }

        #endregion

        #region 可信限计算

        private void PECalc()
        {
            for (int i = 0; i < CalcInfo.DataSize.PreparationNum - 1; i++)
            {
                PEResult.PEValues.Add(new TreatPEValues());
                //double corrValue = CalcInfo.DilutionsTimes.Any(t => t < 0) ? 1 : (CalcInfo.DilutionsTimes[0] * CalcInfo.A[0]) /
                //                (CalcInfo.DilutionsTimes[i + 1] * CalcInfo.A[i + 1]);
                double corrValue = CalcInfo.UnitDil.Any(t => t.Val < 0) ? 1 : (CalcInfo.UnitDil[0].Val * CalcInfo.UnitA[0].Val) /
                                (CalcInfo.UnitDil[i + 1].Val * CalcInfo.UnitA[i + 1].Val);
                double cr = PEResult.C*PEResult.RList[i];
                double firstItem = cr - PEResult.K;
                double secItem = Math.Sqrt((PEResult.C - 1)*(cr*PEResult.RList[i] + 1) + PEResult.K*(PEResult.K - 2*cr));
                //计算Rel. to Ass
                PEResult.PEValues[i].RelToAss.Est = PEResult.RList[i]*_corrValue[i]*corrValue;
                PEResult.PEValues[i].RelToAss.Lower = (firstItem - secItem)*_corrValue[i]*corrValue;
                PEResult.PEValues[i].RelToAss.Upper = (firstItem + secItem)*_corrValue[i]*corrValue;

                //计算Rel. to Est
                PEResult.PEValues[i].RelToEst.Lower = PEResult.PEValues[i].RelToAss.Lower/
                                                      PEResult.PEValues[i].RelToAss.Est;
                PEResult.PEValues[i].RelToEst.Est = PEResult.PEValues[i].RelToAss.Est/PEResult.PEValues[i].RelToAss.Est;
                PEResult.PEValues[i].RelToEst.Upper = PEResult.PEValues[i].RelToAss.Upper/
                                                      PEResult.PEValues[i].RelToAss.Est;

                //计算效价
                PCalc(i);
                CalcSM(i, secItem);
            }
        }

        private void PCalc(int i)
        {
            PEResult.PEValues[i].Potency.Lower = CalcInfo.UnitA[i + 1].Val * PEResult.PEValues[i].RelToAss.Lower;
            PEResult.PEValues[i].Potency.Est = CalcInfo.UnitA[i + 1].Val * PEResult.PEValues[i].RelToAss.Est;
            PEResult.PEValues[i].Potency.Upper = CalcInfo.UnitA[i + 1].Val * PEResult.PEValues[i].RelToAss.Upper;
        }

        #endregion

        protected override void CalcSM(int i, double secItem)
        {
            PEResult.PEValues[i].SM = secItem / Distributions.Dist_t(VAResult.ResValues.FreedomDegree) / Math.Log(10);
        }
    }
}