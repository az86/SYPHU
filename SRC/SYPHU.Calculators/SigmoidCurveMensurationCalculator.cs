using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using SYPHU.Assay.CalculationInfo;
using SYPHU.Assay.Plots;
using SYPHU.Assay.Results.PotencyEstimateResults;
using SYPHU.Assay.Results.ReliabilityCheck;
using SYPHU.Assay.Results.SigmoidCurveIterResult;
using SYPHU.Assay.Results.VarianceAnalysisResults;
using SYPHU.Assay.Tables;
using SYPHU.Utilities;
using SYPHU.Assay.Data;

namespace SYPHU.Calculators
{
    /// <summary>
    /// S型曲线法基类
    /// </summary>
    public class SigmoidCurveMensurationCalculator : AbstractCalculator
    {
        private int _nValue;

        /// <summary>
        /// 方差分析结果
        /// </summary>
        private readonly SigmoidCurveVarAnaResult _vaResult = new SigmoidCurveVarAnaResult();

        /// <summary>
        /// 效价估计结果
        /// </summary>
        protected PEResult PEResults = new PEResult();

        /// <summary>
        /// 迭代结果
        /// </summary>
        protected SCIterResult IterResult = new SCIterResult();

        public override void DoCalculate(InitCalculationInfo calculationInfo, int extremeAbnormalDataNum, List<List<double>> bList)
        {
            CalcInfo = calculationInfo;
            ExtremeAbnormalDataNum = extremeAbnormalDataNum;
            MergeData();
            Iter();
            //PEPreCalc();
            PECalc();
            VarAnaCalc();
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
            PlotsInfo = new List<PlotInfo>();
            PlotsInfo.Add(new PlotInfo());

            var xMinGlob = new List<double>();
            var xMaxGlob = new List<double>();
            //取每组剂量对数的最小/大值=最小/大值的对数
            var tmpd = new List<List<double>>();
            for (int i = 0; i < h; i++)
            {
                tmpd.Add(new List<double>());
                for (int j = 0; j < CalcInfo.DataSize.DoseNum; j++)
                {
                    tmpd[i].Add(CalcInfo.Unitd[i][j].Val);
                }
            }

            for (int i = 0; i < h; i++)
            {
                xMinGlob.Add(Math.Log(tmpd[i].Min()));
                xMaxGlob.Add(Math.Log(tmpd[i].Max()));
            }
            //如果使用稀释倍数，再分别加上对应的ln(效价*稀释倍数)部分
            var dilOffSet = new List<double>();
            if (CalcInfo.UseDilutionsTimes)
            {
                for (int i = 0; i < h; i++)
                {
                    dilOffSet.Add(Math.Log(CalcInfo.UnitA[i].Val * CalcInfo.UnitDil[i].Val));
                    xMinGlob[i] += dilOffSet[i];
                    xMaxGlob[i] += dilOffSet[i];
                }
            }
            //根据全局最大、最小值计算画图区间
            double xMin = xMinGlob.Min();
            double xMax = xMaxGlob.Max();
            double xRange = xMax - xMin;
            
            for (int i = 1; i <= h; i++)
            {
                //添加拟合曲线
                PlotsInfo.Add(new PlotInfo());
                
                PlotsInfo[i].CurveDescs = new List<CurveDesc>();
                PlotsInfo[i].CurveDescs.Add(new CurveDesc());

                //取采样点
                double inc = xRange / PlotsInfo[i].CurveDescs[0].MaxPointNum;
                //如果使用稀释倍数，每组曲线的起始点为全局最小值-对应的ln(效价*稀释倍数)部分-----（因为上面算最小值的时候加上了，现在减回去）
                double xBegin = CalcInfo.UseDilutionsTimes
                                    ? xMin - dilOffSet[i-1]
                                    : xMin;

                //偏移10个点画图
                xBegin -= inc*10;
                
                //根据原始x坐标计算phi，前后各偏移10个点
                for (int j = 0; j <= PlotsInfo[i].CurveDescs[0].MaxPointNum+20; j++)
                {
                    double xCur = xBegin + j * inc;
                    double yCur = IterResult.aList[i - 1] + IterResult.b * xCur;
                    double phiCur = CalcPhiByY(yCur);
                    //如果有稀释倍数，需要根据平移后的坐标画图
                    if (CalcInfo.UseDilutionsTimes)
                    {
                        xCur += dilOffSet[i - 1];
                    }
                    PlotsInfo[i].CurveDescs[0].FitPoints.Add(new Point(xCur, phiCur));
                }
                //方程？？
                int groupId = (CalcInfo.Method == Methods.ED) ? i : i - 1;
                PlotsInfo[i].CurveDescs[0].CurveEquation = ConstStrings.GetGroupIdString(h, groupId);

                //颜色
                PlotsInfo[i].CurveDescs[0].Color = ConstStrings.ColorList[(i - 1) % ConstStrings.ColorList.Count()];

                //添加原始数据
                PlotsInfo[i].DataPoints.Add(new PointGroup());
                PlotsInfo[i].DataPoints[0].Color = PlotsInfo[i].CurveDescs[0].Color;
                for (int j = 0; j < CalcInfo.DataSize.DoseNum; j++)
                {
                    PlotsInfo[i].DataPoints[0].Points.Add(new List<Point>());
                    double xPoint = IterResult.xList[j + (i - 1)*CalcInfo.DataSize.DoseNum];
                    if (CalcInfo.UseDilutionsTimes)
                    {
                        xPoint += dilOffSet[i - 1];
                    }
                    PlotsInfo[i].DataPoints[0].Points[j].Add(new Point(xPoint,
                                                             IterResult.pList[j + (i - 1) * CalcInfo.DataSize.DoseNum]));
                }

                #region 图像信息

                PlotsInfo[i].SetValues(ConstStrings.GetGroupIdString(h, groupId),
                                       ConstStrings.GetXLabel(DataTransformationFormula.LogE, CalcInfo.UseDilutionsTimes),
                                       ConstStrings.GetYLabel(CalcInfo.Type),
                                       inc * (PlotsInfo[i].CurveDescs[0].MaxPointNum + 20), 1.0/*IterResult.pList.MathRange()*/);

                #endregion
            }

            #region 合并子图信息

            if (CalcInfo.Method != Methods.ED)
            {

                PlotsInfo[0].CurveDescs = new List<CurveDesc>();
                for (int i = 1; i <= h; i++)
                {
                    PlotsInfo[0].DataPoints.Add(PlotsInfo[i].DataPoints[0]);
                    PlotsInfo[0].CurveDescs.Add(PlotsInfo[i].CurveDescs[0]);
                }
                PlotsInfo[0].SetValues("All Samples",
                                       ConstStrings.GetXLabel(DataTransformationFormula.LogE, CalcInfo.UseDilutionsTimes),
                                       ConstStrings.GetYLabel(CalcInfo.Type),
                                       xRange, IterResult.pList.MathRange());

                if (h != 1)
                {
                    CalcDashedLineInfoByED();
                }
            }
            else
            {
                PlotsInfo.RemoveAt(0);
                PlotsInfo[0].CurveDescs[0].CurveEquation = CalcInfo.Model.ToString();
            }


            #endregion

        }

        private double CalcPhiByY(double yCur)
        {
            switch (CalcInfo.Model)
            {
                case Models.Probit:
                    return Distributions.Dist_Phi(yCur);
                case Models.Logit:
                    return 1.0 / (1.0 + Math.Exp(-yCur));
                case Models.Gompit:
                    return 1.0 - Math.Exp(-Math.Exp(yCur));
                case Models.Angle:
                    if (yCur < -0.5 * ConstantsExt.Pi)
                    {
                        return ConstantsExt.Eps(-6);
                    }
                    if (yCur > 0.5 * ConstantsExt.Pi)
                    {
                        return 1.0 - ConstantsExt.Eps(-6);
                    }
                    return 0.5 * Math.Sin(yCur) + 0.5;
            }
            return 0;
        }

        private void CalcDashedLineInfoByED()
        {
            InitCalculationInfo calculationInfo = new InitCalculationInfo();
            calculationInfo.Lang = CalcInfo.Lang;
            calculationInfo.DataSize.PreparationNum = 1;
            calculationInfo.DataSize.DoseNum = CalcInfo.DataSize.DoseNum;
            calculationInfo.DataSize.ReplicateNum = CalcInfo.DataSize.ReplicateNum;
            calculationInfo.CalcCase = CalcInfo.CalcCase;
            calculationInfo.Method = Methods.ED;
            calculationInfo.Design = CalcInfo.Design;
            calculationInfo.Model = CalcInfo.Model;
            calculationInfo.Type = CalcInfo.Type;
            calculationInfo.DataTransFormula = DataTransformationFormula.Null;
            calculationInfo.AbnDataCheckMethods = CalcInfo.AbnDataCheckMethods;
            calculationInfo.EDPercent = "50";

            for (int i = 0; i < CalcInfo.DataSize.PreparationNum; i++)
            {
                //补充A值
                calculationInfo.UnitA = new List<UnitData>();
                calculationInfo.UnitA.Add(CalcInfo.UnitA[i]);
                //补充dil值
                calculationInfo.UnitDil = new List<UnitData>();
                calculationInfo.UnitDil.Add(CalcInfo.UnitDil[i]);
                //补充d值
                calculationInfo.Unitd = new List<List<UnitData>>();
                calculationInfo.Unitd.Add(new List<UnitData>());
                for (int j = 0; j < calculationInfo.DataSize.DoseNum; j++)
                {
                    calculationInfo.Unitd[0].Add(CalcInfo.Unitd[i][j]);
                }

                //单组试验数据
                AssayData curData = new AssayData();
                curData.InitData(calculationInfo.DataSize);
                for (int j = 0; j < calculationInfo.DataSize.DoseNum; j++)
                {
                    for (int k = 0; k < calculationInfo.DataSize.ReplicateNum; k++)
                    {
                        curData.Data[0][j][k] = AssData.Data[i][j][k];
                    }
                }

                var calculator = new EDCalculator();
                calculator.LoadCalcData(curData);
                calculator.CalcInfo = calculationInfo;
                calculator.IsLimitCurvePlotted = false;
                calculator.MergeData();
                calculator.Iter();
                calculator.CreatePlotInfo(1);
                PlotsInfo[i + 1].CurveDescs.Add(calculator.PlotsInfo[0].CurveDescs[0]);
                PlotsInfo[i + 1].CurveDescs[1].PlotType = PlotTypes.DashedLine;
                PlotsInfo[i + 1].CurveDescs[1].Color = PlotsInfo[i + 1].CurveDescs[0].Color;
            }
        }

        #region 填充表格

        private void FillVATable()
        {
            FillVAValues(1, _vaResult.PrepValues);
            FillVAValues(2, _vaResult.RegValues);
            FillVAValues(3, _vaResult.ParValues);
            if (CalcInfo.DataSize.DoseNum <= 2)
            {
                FillVAValues(4, _vaResult.TreatValues);
                VATable.Table.Cells[5][3].Content = "1.00000";
                FillVAValues(6, _vaResult.TotalValues, 3);
            }
            else
            {
                FillVAValues(4, _vaResult.LinValues);
                for (int i = 0; i < CalcInfo.DataSize.PreparationNum; i++)
                {
                    FillVAValues(5 + i, _vaResult.LinsValues[i]);
                }
                FillVAValues(5 + CalcInfo.DataSize.PreparationNum, _vaResult.TreatValues);
                VATable.Table.Cells[6 + CalcInfo.DataSize.PreparationNum][3].Content = "1.00000";
                FillVAValues(7 + CalcInfo.DataSize.PreparationNum, _vaResult.TotalValues, 3);
            }
        }

        private void FillPETable()
        {
            for (int i = 0; i < CalcInfo.DataSize.PreparationNum - 1; i++)
            {
                if (CalcInfo.UnitA[i + 1].Unit != "")
                {
                    PETableList[i].Table.Cells[3][0].Content += "(" + CalcInfo.UnitA[i + 1].Unit + ")";
                }
                FillPEValues(i, PETableList[i].Table.DataStartRow, PEResults.PEValues[i].Potency, "F4");
                FillPEValues(i, PETableList[i].Table.DataStartRow + 1, PEResults.PEValues[i].RelToAss);
                FillPEValues(i, PETableList[i].Table.DataStartRow + 2, PEResults.PEValues[i].RelToEst);
                PETableList[i].Table.Cells[PETableList[i].Table.DataStartRow + 3][1].Content = PEResults.PEValues[i].SM.ToString("F10");
                PETableList[i].Table.Cells[PETableList[i].Table.DataStartRow + 4][1].Content =
                    PEResults.PEValues[i].CLPercent.ToString("P2");
            }
        }

        #endregion

        #region 可信限预计算内容

        /// <summary>
        /// 可信限预计算
        /// </summary>
        private void PEPreCalc()
        {
            CalcMList();
            CalcC();
            CalcV();
        }

        protected virtual void MergeData()
        {

        }

        private void Iter()
        {
            IterResult.Do_Cpt(CalcInfo.DataSize, CalcInfo.Model);
        }

        private void CalcMList()
        {
            for (int i = 0; i < CalcInfo.DataSize.PreparationNum - 1; i++)
            {
                IterResult.MList.Add((IterResult.aList[i + 1] - IterResult.aList[0]) / IterResult.b);
            }
        }

        protected void CalcC()
        {
            double tmp = IterResult.b * IterResult.b * IterResult.SxxList.Sum();
            const double s = 1;
            const double t = 1.96;
            IterResult.C = tmp / (tmp - s * s * t * t);
        }

        private void CalcV()
        {
            IterResult.V = new List<double>();
            for (int i = 0; i < CalcInfo.DataSize.PreparationNum - 1; i++)
            {
                IterResult.V.Add(1 / IterResult.wList.PartialSum(0, CalcInfo.DataSize.DoseNum - 1) +
                               1 /
                               IterResult.wList.PartialSum(CalcInfo.DataSize.DoseNum * (i + 1),
                                                         CalcInfo.DataSize.DoseNum * (i + 2) - 1));
            }
        }

        #endregion

        #region 可信限计算

        protected virtual void PECalc()
        {
            PEPreCalc();
            for (int i = 0; i < CalcInfo.DataSize.PreparationNum - 1; i++)
            {
                PEResults.PEValues.Add(new TreatPEValues());
                //计算效价

                //稀释比率
                double dilutionRatio = CalcInfo.UseDilutionsTimes
                                           ? (CalcInfo.UnitA[0].Val * CalcInfo.UnitDil[0].Val) /
                                             (CalcInfo.UnitA[i + 1].Val * CalcInfo.UnitDil[i + 1].Val)
                                           : 1;

                double cm = IterResult.C * IterResult.MList[i];
                double firItem = cm - (IterResult.C - 1) * (IterResult.xAveList[0] - IterResult.xAveList[i + 1]);
                double secItem =
                    Math.Sqrt((IterResult.C - 1) *
                              (IterResult.V[i] * IterResult.SxxList.Sum() +
                               IterResult.C * (IterResult.MList[i] - IterResult.xAveList[0] + IterResult.xAveList[i + 1]) *
                               (IterResult.MList[i] - IterResult.xAveList[0] + IterResult.xAveList[i + 1])));
                PEResults.PEValues[i].Potency.Lower = VectorCalcMethodExt.Antiln(firItem - secItem) * CalcInfo.UnitA[i + 1].Val * dilutionRatio;
                PEResults.PEValues[i].Potency.Est = VectorCalcMethodExt.Antiln(IterResult.MList[i]) * CalcInfo.UnitA[i + 1].Val * dilutionRatio;
                PEResults.PEValues[i].Potency.Upper = VectorCalcMethodExt.Antiln(firItem + secItem) * CalcInfo.UnitA[i + 1].Val * dilutionRatio;

                //计算Rel. to Ass
                PEResults.PEValues[i].RelToAss.Lower = VectorCalcMethodExt.Antiln(firItem - secItem) * dilutionRatio;
                PEResults.PEValues[i].RelToAss.Est = VectorCalcMethodExt.Antiln(IterResult.MList[i]) * dilutionRatio;
                PEResults.PEValues[i].RelToAss.Upper = VectorCalcMethodExt.Antiln(firItem + secItem) * dilutionRatio;
                //计算Rel. to Est
                PEResults.PEValues[i].RelToEst.Lower = PEResults.PEValues[i].RelToAss.Lower /
                                                      PEResults.PEValues[i].RelToAss.Est;
                PEResults.PEValues[i].RelToEst.Est = PEResults.PEValues[i].RelToAss.Est / PEResults.PEValues[i].RelToAss.Est;
                PEResults.PEValues[i].RelToEst.Upper = PEResults.PEValues[i].RelToAss.Upper /
                                                      PEResults.PEValues[i].RelToAss.Est;
                CalcSM(i, secItem);
            }
        }

        #endregion

        #region 方差分析计算

        /// <summary>
        ///     方差分析表格计算
        /// </summary>
        private void VarAnaCalc()
        {
            CalcFreedom();
            CalcSquareSum();
            CalcFValues();
            CalcPValues();
            DoReliabilityCheck();
        }

        protected virtual void CalcFreedom()
        {
            _nValue = CalcInfo.DataSize.ReplicateNum / 2;

            _vaResult.PrepValues.FreedomDegree = CalcInfo.DataSize.PreparationNum - 1;
            _vaResult.RegValues.FreedomDegree = 1;
            _vaResult.ParValues.FreedomDegree = CalcInfo.DataSize.PreparationNum - 1;
            _vaResult.LinValues.FreedomDegree = CalcInfo.DataSize.PreparationNum * (CalcInfo.DataSize.DoseNum - 2);
            for (int i = 0; i < CalcInfo.DataSize.PreparationNum; i++)
            {
                _vaResult.LinsValues.Add(new BasicVarianceAnalysisValues());
                _vaResult.LinsValues[i].FreedomDegree = CalcInfo.DataSize.DoseNum - 2;
            }
            _vaResult.TreatValues.FreedomDegree = CalcInfo.DataSize.PreparationNum * CalcInfo.DataSize.DoseNum - 1;
            _vaResult.TotalValues.FreedomDegree = CalcInfo.DataSize.PreparationNum * CalcInfo.DataSize.DoseNum *
                                                _nValue - 1 - ExtremeAbnormalDataNum;
        }

        protected virtual void CalcSquareSum()
        {
            _vaResult.RegValues.SquareSum = IterResult.SxyList.Sum() * IterResult.SxyList.Sum() / IterResult.SxxList.Sum();
            _vaResult.ParValues.SquareSum = IterResult.SxyList.Select((t, i) => t * t / IterResult.SxxList[i]).Sum() -
                                           _vaResult.RegValues.SquareSum;
            _vaResult.LinValues.SquareSum = 0;
            for (int i = 0; i < CalcInfo.DataSize.PreparationNum; i++)
            {
                _vaResult.LinsValues[i].SquareSum = IterResult.SyyList[i] -
                                                   IterResult.SxyList[i] * IterResult.SxyList[i] / IterResult.SxxList[i];
                _vaResult.LinValues.SquareSum += _vaResult.LinsValues[i].SquareSum;
            }

            _vaResult.TotalValues.SquareSum = IterResult.wy2SumList.Sum() -
                                             IterResult.wySumList.Sum() * IterResult.wySumList.Sum() / IterResult.wSumList.Sum();

            _vaResult.PrepValues.SquareSum = _vaResult.TotalValues.SquareSum - _vaResult.RegValues.SquareSum -
                                            _vaResult.ParValues.SquareSum - _vaResult.LinValues.SquareSum;

            _vaResult.TreatValues.SquareSum = _vaResult.TotalValues.SquareSum;
        }

        protected virtual void CalcFValues()
        {
            _vaResult.PrepValues.FValue = _vaResult.PrepValues.SquareSum;
            _vaResult.RegValues.FValue = _vaResult.RegValues.SquareSum;
            _vaResult.ParValues.FValue = _vaResult.ParValues.SquareSum;
            _vaResult.LinValues.FValue = _vaResult.LinValues.SquareSum;
            for (int i = 0; i < CalcInfo.DataSize.PreparationNum; i++)
            {
                _vaResult.LinsValues[i].FValue = _vaResult.LinsValues[i].SquareSum;
            }
            _vaResult.TreatValues.FValue = _vaResult.TreatValues.SquareSum;
            _vaResult.TotalValues.FValue = _vaResult.TotalValues.SquareSum;
        }

        protected virtual void CalcPValues()
        {
            _vaResult.PrepValues.PValue = Distributions.Dist_X2(_vaResult.PrepValues.FreedomDegree,
                                                               _vaResult.PrepValues.FValue);
            _vaResult.RegValues.PValue = Distributions.Dist_X2(_vaResult.RegValues.FreedomDegree,
                                                               _vaResult.RegValues.FValue);
            _vaResult.ParValues.PValue = Distributions.Dist_X2(_vaResult.ParValues.FreedomDegree,
                                                               _vaResult.ParValues.FValue);
            _vaResult.LinValues.PValue = Distributions.Dist_X2(_vaResult.LinValues.FreedomDegree,
                                                               _vaResult.LinValues.FValue);
            for (int i = 0; i < CalcInfo.DataSize.PreparationNum; i++)
            {
                _vaResult.LinsValues[i].PValue = Distributions.Dist_X2(_vaResult.LinsValues[i].FreedomDegree,
                                                                      _vaResult.LinsValues[i].FValue);
            }
            _vaResult.TreatValues.PValue = Distributions.Dist_X2(_vaResult.TreatValues.FreedomDegree,
                                                               _vaResult.TreatValues.FValue);
        }

        /// <summary>
        /// 可靠性判断
        /// </summary>
        protected virtual void DoReliabilityCheck()
        {
            VariationList.Clear();
            VariationList.Add(ConstStrings.VariationSourcesDict[CalcInfo.Lang][VariationSources.Prep], _vaResult.PrepValues.PValue);
            VariationList.Add(ConstStrings.VariationSourcesDict[CalcInfo.Lang][VariationSources.Reg], _vaResult.RegValues.PValue);

            if (CalcInfo.DataSize.PreparationNum >= 2)
            {
                VariationList.Add(ConstStrings.VariationSourcesDict[CalcInfo.Lang][VariationSources.Par], _vaResult.ParValues.PValue);
            }

            if (CalcInfo.DataSize.DoseNum > 2)
            {
                VariationList.Add(ConstStrings.VariationSourcesDict[CalcInfo.Lang][VariationSources.Lin], _vaResult.LinValues.PValue);
            }

            ReliabilityChecker.DoCheck(VariationList, CalcInfo.Lang);
        }

        #endregion

        protected override void CalcSM(int i, double secItem)
        {
            PEResults.PEValues[i].SM = secItem / 1.96 / Math.Log(10);
        }
    }
}
