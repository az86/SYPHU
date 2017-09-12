using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using SYPHU.Assay.CalculationInfo;
using SYPHU.Assay.Plots;
using SYPHU.Assay.Results.PotencyEstimateResults;
using SYPHU.Assay.Results.VarianceAnalysisResults;
using SYPHU.Assay.Tables;
using SYPHU.Utilities;

namespace SYPHU.Calculators
{
    /// <summary>
    ///     平行线检定法计算基类
    /// </summary>
    public class ParallelLineMensurationCalculator : AbstractCalculator
    {
        /// <summary>
        /// 方差分析结果
        /// </summary>
        protected ParallelLinesVarAnaResult VAResult = new ParallelLinesVarAnaResult();

        /// <summary>
        /// 效价估计结果
        /// </summary>
        protected readonly ParallelLinePEResult PEResult = new ParallelLinePEResult();

        /// <summary>
        /// 拉丁方B值
        /// </summary>
        protected List<List<double>> BList;

        /// <summary>
        /// 稀释倍数校正值
        /// </summary>
        private List<double> _corrValue;

        public override void DoCalculate(InitCalculationInfo calculationInfo, int extremeAbnormalDataNum, List<List<double>> bList)
        {
            CalcInfo = calculationInfo;
            ExtremeAbnormalDataNum = extremeAbnormalDataNum;
            BList = bList;
            VarAnaPreCalc();
            VarAnaCalc();
            PEPreCalc();
            PECalc(CalcInfo.DataSize.PreparationNum);
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
            FillPETable(CalcInfo.DataSize.PreparationNum);
            ConclusionTable.CreateTable(ReliabilityChecker.ConclusionList, ReliabilityChecker.FinalConclusion, CalcInfo.Lang);
        }

        public override void CreatePlotInfo(int h)
        {
            #region 坐标

            //计算散点X值
            var dList = new List<List<double>>();
            var xList = new List<double>();
            for (int i = 0; i < h; i++)
            {
                dList.Add(new List<double>());
                for (int j = 0; j < CalcInfo.DataSize.DoseNum; j++)
                {
                    dList[i].Add(CalcInfo.UseDilutionsTimes
                                     ? Math.Log(CalcInfo.Unitd[i][j].Val*CalcInfo.UnitA[i].Val*CalcInfo.UnitDil[i].Val)
                                     : Math.Log(CalcInfo.Unitd[i][j].Val));
                }
                xList.Add(dList[i].Average());
            }

            //计算散点Y值
            var yList = new List<double>();
            for (int i = 0; i < h; i++)
            {
                yList.Add(VAResult.SampleAverage[i].Average());
            }

            #endregion

            #region 实线

            //平行线法，公共的b，计算每组试验的a
            var aList = new List<double>();
            for (int i = 0; i < h; i++)
            {
                aList.Add(yList[i] - PEResult.b*xList[i]);
            }

            #endregion

            #region 虚线
            
            List<double> bDashed = new List<double>();
            List<double> aDashed = new List<double>();

            for (int i = 0; i < h; i++)
            {
                bDashed.Add(VAResult.HL*VAResult.L[i]/PEResult.I/CalcInfo.DataSize.ReplicateNum);
                aDashed.Add(yList[i] - bDashed[i]*xList[i]);
            }

            #endregion

            #region Plot

            double yRange = VectorCalcMethodExt.List3DExtremum(AssData.Data) -
                            VectorCalcMethodExt.List3DExtremum(AssData.Data, false);

            PlotsInfo = new List<PlotInfo>();
            //所有图的集合
            PlotsInfo.Add(new PlotInfo());

            //子图
            for (int i = 1; i <= h; i++)
            {
                #region 曲线信息

                //添加拟合直线
                PlotsInfo.Add(new PlotInfo());

                PlotsInfo[i].CurveDescs = new List<CurveDesc>();

                #region 实线信息

                PlotsInfo[i].CurveDescs.Add(new CurveDesc());

                //直线，取起点、终点即可
                PlotsInfo[i].CurveDescs[0].FitPoints.Add(new Point(dList[i - 1][0],
                                                                   aList[i - 1] + PEResult.b*dList[i - 1][0]));
                PlotsInfo[i].CurveDescs[0].FitPoints.Add(new Point(dList[i - 1][CalcInfo.DataSize.DoseNum - 1],
                                                                   aList[i - 1] +
                                                                   PEResult.b*
                                                                   dList[i - 1][CalcInfo.DataSize.DoseNum - 1]));

                //直线方程
                PlotsInfo[i].CurveDescs[0].CurveEquation = "y' = " + aList[i - 1].ToString("F4") +
                                                           (PEResult.b < 0 ? "-" : "+") +
                                                           (Math.Abs(PEResult.b)).ToString("F4") + "*x";

                //颜色
                PlotsInfo[i].CurveDescs[0].Color = ConstStrings.ColorList[(i - 1)%ConstStrings.ColorList.Count()];

                //实线
                PlotsInfo[i].CurveDescs[0].PlotType = PlotTypes.SolidLine;

                #endregion

                #region 虚线信息

                PlotsInfo[i].CurveDescs.Add(new CurveDesc());

                //直线，取起点、终点即可
                PlotsInfo[i].CurveDescs[1].FitPoints.Add(new Point(dList[i - 1][0],
                                                                   aDashed[i - 1] + bDashed[i-1]*dList[i - 1][0]));
                PlotsInfo[i].CurveDescs[1].FitPoints.Add(new Point(dList[i - 1][CalcInfo.DataSize.DoseNum - 1],
                                                                   aDashed[i - 1] +bDashed[i-1]*dList[i - 1][CalcInfo.DataSize.DoseNum - 1]));

                //直线方程
                PlotsInfo[i].CurveDescs[1].CurveEquation = "1";

                //颜色
                PlotsInfo[i].CurveDescs[1].Color = ConstStrings.ColorList[(i - 1)%ConstStrings.ColorList.Count()];

                //虚线
                PlotsInfo[i].CurveDescs[1].PlotType = PlotTypes.DashedLine;

                #endregion


                #endregion

                #region 散点信息

                //添加原始数据
                PlotsInfo[i].DataPoints.Add(new PointGroup());
                PlotsInfo[i].DataPoints[0].Color = PlotsInfo[i].CurveDescs[0].Color;
                for (int j = 0; j < CalcInfo.DataSize.DoseNum; j++)
                {
                    PlotsInfo[i].DataPoints[0].Points.Add(new List<Point>());
                    for (int k = 0; k < CalcInfo.DataSize.ReplicateNum; k++)
                    {
                        PlotsInfo[i].DataPoints[0].Points[j].Add(new Point(dList[i-1][j], AssData.Data[i - 1][j][k]));
                    }
                }

                #endregion

                #region 图像信息

                PlotsInfo[i].SetValues(ConstStrings.GetGroupIdString(h, i - 1),
                                       ConstStrings.GetXLabel(DataTransformationFormula.LogE, CalcInfo.UseDilutionsTimes),
                                       ConstStrings.GetYLabel(CalcInfo.Type), dList[i - 1].MathRange(), yRange);

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
                                   ConstStrings.GetXLabel(DataTransformationFormula.LogE, CalcInfo.UseDilutionsTimes),
                                   ConstStrings.GetYLabel(CalcInfo.Type), xRange, yRange);

            #endregion

            #endregion
        }
        
        #region 填充表格

        protected virtual void FillVATable()
        {
        }

        protected void FillPETable(int h)
        {
            for (int i = 0; i < h - 1; i++)
            {
                if (CalcInfo.UnitA[i + 1].Unit != "")
                {
                    PETableList[i].Table.Cells[3][0].Content +="(" + CalcInfo.UnitA[i + 1].Unit + ")";
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
        protected virtual void VarAnaPreCalc()
        {
            CalcAverage();
            CalcAverageSum();
            CalcOthers();
        }

        protected void CalcAverage()
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

        private void CalcOthers()
        {
            double n = Convert.ToDouble(CalcInfo.DataSize.ReplicateNum);
            double d = Convert.ToDouble(CalcInfo.DataSize.DoseNum);
            double h = Convert.ToDouble(CalcInfo.DataSize.PreparationNum);
            VAResult.L = new List<double>(CalcInfo.DataSize.PreparationNum);
            for (int i = 0; i < CalcInfo.DataSize.PreparationNum; i++)
            {
                double tmp = 0.0;
                for (int j = 0; j < CalcInfo.DataSize.DoseNum; j++)
                {
                    tmp += VAResult.SampleAverage[i][j] * (j + 1.0);
                }
                VAResult.L.Add(tmp - 0.5 * (d + 1) * VAResult.SampleAverageSum[i]);
            }

            VAResult.Hp = n/d;
            VAResult.HL = 12*n/(d*d*d - d);
            VAResult.K = n*VAResult.SampleAverageSum.Sum()*VAResult.SampleAverageSum.Sum()/(h*d);

            VAResult.KList = new List<double>();
            VAResult.SStreatList = new List<double>();
            VAResult.SSregList = new List<double>();
            for (int i = 0; i < CalcInfo.DataSize.PreparationNum; i++)
            {
                VAResult.KList.Add(n * VAResult.SampleAverageSum[i] * VAResult.SampleAverageSum[i]/d);
                VAResult.SStreatList.Add(n * VAResult.SampleAverage[i].SquareSum() - VAResult.KList[i]);
                VAResult.SSregList.Add(VAResult.HL * VAResult.L[i] * VAResult.L[i]);
            }
        }

        #endregion

        #region 方差分析表格计算内容

        /// <summary>
        ///     方差分析表格计算
        /// </summary>
        protected void VarAnaCalc()
        {
            CalcFreedom();
            CalcSquareSum();
            CalcFValues();
            CalcPValues();
            DoReliabilityCheck();
        }

        /// <summary>
        ///     计算自由度
        /// </summary>
        protected virtual void CalcFreedom()
        {
            VAResult.PrepValues.FreedomDegree = CalcInfo.DataSize.PreparationNum - 1;
            VAResult.RegValues.FreedomDegree = 1;
            VAResult.ParValues.FreedomDegree = CalcInfo.DataSize.PreparationNum - 1;
            VAResult.TotalValues.FreedomDegree = CalcInfo.DataSize.PreparationNum*CalcInfo.DataSize.DoseNum*
                                                     CalcInfo.DataSize.ReplicateNum - 1 - ExtremeAbnormalDataNum;
        }

        /// <summary>
        ///     计算平方和
        /// </summary>
        protected virtual void CalcSquareSum()
        {
            VAResult.PrepValues.SquareSum = VAResult.Hp*VAResult.SampleAverageSum.SquareSum() -
                                                VAResult.K;
            VAResult.RegValues.SquareSum = VAResult.HL*VAResult.L.Sum()*VAResult.L.Sum()/
                                               CalcInfo.DataSize.PreparationNum;
            VAResult.ParValues.SquareSum = VAResult.HL*VAResult.L.SquareSum() -
                                               VAResult.RegValues.SquareSum;

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
            //VarAnaResult.ResValues.SquareSum = VarAnaResult.TotalValues.SquareSum - VarAnaResult.TreatValues.SquareSum;
        }

        /// <summary>
        ///     计算F值
        /// </summary>
        protected virtual void CalcFValues()
        {
        }

        /// <summary>
        ///     计算P值
        /// </summary>
        protected virtual void CalcPValues()
        {
        }

        /// <summary>
        /// 可靠性判断
        /// </summary>
        protected virtual void DoReliabilityCheck()
        {
        }

        #endregion
        
        #region 可信限预计算内容

        /// <summary>
        ///     可信限预计算
        /// </summary>
        protected void PEPreCalc()
        {
            CalcCorrValue();
            CalcI();
            Calcb();
            CalcM();
            CalcC();
            CalcV();
        }


        private void CalcCorrValue()
        {
            _corrValue = new List<double>();
            var tmpd = new List<List<double>>();
            for (int i = 0; i < CalcInfo.DataSize.PreparationNum; i++)
            {
                tmpd.Add(new List<double>());
                for (int j = 0; j < CalcInfo.DataSize.DoseNum; j++)
                {
                    tmpd[i].Add(CalcInfo.Unitd[i][j].Val);
                }
            }

            for (int i = 1; i < CalcInfo.DataSize.PreparationNum; i++)
            {
                _corrValue.Add(Math.Log(tmpd[0].Max() / tmpd[i].Max()));
            }
        }

        /// <summary>
        ///     计算I值
        /// </summary>
        private void CalcI()
        {
            PEResult.I = Math.Abs(CalcInfo.Unitd[0][0].Val) < ConstantsExt.Eps()
                             ? 0
                             : Math.Log(CalcInfo.Unitd[0][1].Val / CalcInfo.Unitd[0][0].Val);
        }

        /// <summary>
        ///     计算b值
        /// </summary>
        protected virtual void Calcb()
        {
            PEResult.b = VAResult.HL*VAResult.L.Sum()/
                         (PEResult.I*CalcInfo.DataSize.ReplicateNum*CalcInfo.DataSize.PreparationNum);
        }

        /// <summary>
        ///     计算M值
        /// </summary>
        protected virtual void CalcM()
        {
            for (int i = 0; i < CalcInfo.DataSize.PreparationNum - 1; i++)
            {
                PEResult.MList.Add((VAResult.SampleAverageSum[i + 1] - VAResult.SampleAverageSum[0])/
                               (CalcInfo.DataSize.DoseNum*PEResult.b));
            }
        }

        /// <summary>
        ///     计算C值
        /// </summary>
        protected virtual void CalcC()
        {
        }

        /// <summary>
        ///     计算V值
        /// </summary>
        protected virtual void CalcV()
        {
            PEResult.V = VAResult.RegValues.SquareSum/
                         (PEResult.b*PEResult.b*CalcInfo.DataSize.DoseNum*CalcInfo.DataSize.ReplicateNum);
        }

        #endregion

        #region 计算效价及可信限

        /// <summary>
        ///     计算效价及可信限
        /// </summary>
        protected void PECalc(int h)
        {
            for (int i = 0; i < h - 1; i++)
            {
                PEResult.PEValues.Add(new TreatPEValues());
                //计算Rel. to Ass
                PCalc(i);
                //计算效价
                PEResult.PEValues[i].Potency.Lower = PEResult.PEValues[i].RelToAss.Lower * CalcInfo.UnitA[i + 1].Val;
                PEResult.PEValues[i].Potency.Est = PEResult.PEValues[i].RelToAss.Est * CalcInfo.UnitA[i + 1].Val;
                PEResult.PEValues[i].Potency.Upper = PEResult.PEValues[i].RelToAss.Upper * CalcInfo.UnitA[i + 1].Val;
                //计算Rel. to Est
                PEResult.PEValues[i].RelToEst.Lower = PEResult.PEValues[i].RelToAss.Lower/
                                                      PEResult.PEValues[i].RelToAss.Est;
                PEResult.PEValues[i].RelToEst.Est = PEResult.PEValues[i].RelToAss.Est/PEResult.PEValues[i].RelToAss.Est;
                PEResult.PEValues[i].RelToEst.Upper = PEResult.PEValues[i].RelToAss.Upper/
                                                      PEResult.PEValues[i].RelToAss.Est;
            }
        }

        private void PCalc(int i)
        {
            //计算效价
            double corrValue = CalcInfo.UnitDil.Any(t => t.Val < 0) ? 1 :
            (CalcInfo.UnitDil[0].Val * CalcInfo.UnitA[0].Val) /
                               (CalcInfo.UnitDil[i + 1].Val * CalcInfo.UnitA[i + 1].Val);
            double cm = PEResult.C*PEResult.MList[i];
            double secItem = Math.Sqrt((PEResult.C - 1)*(cm*PEResult.MList[i] + 2*PEResult.V));
            PEResult.PEValues[i].RelToAss.Lower =
                Math.Exp(cm - secItem + _corrValue[i] +Math.Log(corrValue));
            PEResult.PEValues[i].RelToAss.Est =
                Math.Exp(PEResult.MList[i] + _corrValue[i] +Math.Log(corrValue));
            PEResult.PEValues[i].RelToAss.Upper =
                Math.Exp(cm + secItem + _corrValue[i] +Math.Log(corrValue));

            CalcSM(i, secItem);
        }

        protected override void CalcSM(int i, double secItem)
        {
        }

        #endregion
    }
}
