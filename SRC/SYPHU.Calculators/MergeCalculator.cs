using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using SYPHU.Assay.CalculationInfo;
using SYPHU.Assay.Data;
using SYPHU.Assay.Plots;
using SYPHU.Assay.Results.PotencyEstimateResults;
using SYPHU.Assay.Tables;
using SYPHU.Utilities;

namespace SYPHU.Calculators
{
    public class MergeCalculator
    {
        private const int CalcCase = 3;

        /// <summary>
        /// 试验数据（校正后），用于计算合并效价
        /// </summary>
        private AssayData _corData = new AssayData();

        /// <summary>
        /// 原始数据，用于画直方图
        /// </summary>
        private AssayData _orgData = new AssayData();

        /// <summary>
        /// 计算信息
        /// </summary>
        private InitCalculationInfo _calculationInfo;

        /// <summary>
        /// 实际行数（去掉异常值行）
        /// </summary>
        private int _actualRowNum;

        /// <summary>
        /// 结论表
        /// </summary>
        public readonly ReliabilityConclusionTable ConclusionTable = new ReliabilityConclusionTable();

        /// <summary>
        /// 可靠性检测结果
        /// </summary>
        private String _checkResult;

        /// <summary>
        /// 可靠性检测结论
        /// </summary>
        private String _checkConclusion;

        /// <summary>
        /// 效价表计算结果
        /// </summary>
        public PEResult PEResults;

        /// <summary>
        /// 估计效价表
        /// </summary>
        public List<PotencyEstimateTable> PETableList;

        private Dictionary<String, double> _labelDict;

        /// <summary>
        /// 频率直方图画图信息
        /// </summary>
        public Histogram HistogramInfos;

        /// <summary>
        /// 工字图画图信息
        /// </summary>
        public List<MergeCalcPlotInfo> PlotInfos;

        #region 私有成员

        private List<double> _MList;

        private List<double> _orgMList;

        private List<double> _dfList;

        private double _tTotal;

        private List<double> _WList;

        private double _MAve;

        private double _SmAve;

        private double _X2;

        private double _df;

        private List<AssayData> _splitData;

        private Dictionary<int, int> _splitDict;

        #endregion

        /// <summary>
        /// 读取计算数据
        /// </summary>
        /// <param name="corData"></param>
        /// <param name="orgData"></param>
        public void LoadCalcData(AssayData corData, AssayData orgData = null)
        {
            _corData = corData;
            if (orgData != null)
            {
                _orgData = orgData;
            }
        }

        public void DoCalculate(InitCalculationInfo calculationInfo, int extremeAbnormalDataNum)
        {
            _calculationInfo = calculationInfo;
            _actualRowNum = calculationInfo.DataSize.ReplicateNum - extremeAbnormalDataNum;
            bool isSep = SplitData();
            DoFullCalculation();
            CreateTables();
            CalcHistogramInfo();
            if (isSep)
            {
                DoSeparatedCalculation();
            }
        }

        private void CreateTables()
        {
            CreatePETable();
            CreateConclusionTable();
        }

        private void DoFullCalculation()
        {
            PEResults = new PEResult();
            if (_actualRowNum == 1)
            {
                CalcPEValuesOneRows();
            }
            else
            {
                GetMAndDfValues();
                DoCalcWeighted();
                DoCalcSemiWeighted();
                DoCalcUnweighted();
            }
            AddPlotInfo();
        }

        private void GetMAndDfValues()
        {
            _MList = new List<double>();
            _orgMList = new List<double>();
            _dfList = new List<double>();
            for (int i = 0; i < _actualRowNum; i++)
            {
                if (_calculationInfo.DataSize.DoseNum == 5)
                {
                    _MList.Add(_corData.Data[0][2][i]);
                }
                else
                {
                    _MList.Add(_corData.Data[0][1][i]);
                }

                _dfList.Add(_corData.Data[0][_calculationInfo.DataSize.DoseNum - 1][i]);
            }

            for (int i = 0; i < _calculationInfo.DataSize.ReplicateNum; i++)
            {
                if (_calculationInfo.DataSize.DoseNum == 5)
                {
                    _orgMList.Add(_orgData.Data[0][2][i]);
                }
                else
                {
                    _orgMList.Add(_orgData.Data[0][1][i]);
                }
            }
        }

        private void DoSeparatedCalculation()
        {
            for (int i = 0; i < _splitDict.Count; i++)
            {
                _actualRowNum = _splitDict.ElementAt(i).Value;
                LoadCalcData(_splitData[i]);
                DoFullCalculation();
            }
        }

        private void CalcHistogramInfo()
        {
            CreateLabelDict();
            Grouping(CalcGroup());
            SetHistogramPlotInfo();
        }

        private void CreateLabelDict()
        {
            _labelDict = new Dictionary<string, double>();
            var groupsDict = new Dictionary<int, int>();
            for (int i = 0; i < _calculationInfo.DataSize.ReplicateNum; i++)
            {
                int curKey = (int) _orgData.Data[0][0][i];
                if (groupsDict.ContainsKey(curKey))
                {
                    groupsDict[curKey]++;
                }
                else
                {
                    groupsDict.Add(curKey, 1);
                }
                _labelDict.Add(curKey.ToString(CultureInfo.InvariantCulture) + (groupsDict[curKey]-1).IntToLabelString(), _orgMList[i]);
            }
        }

        private List<double> CalcGroup()
        {
            var dataWithLabel = new List<double>();
            dataWithLabel.AddRange(_orgMList);

            HistogramInfos = new Histogram();
            HistogramInfos.GroupNum = (int)(Math.Ceiling(1 + Math.Log(_orgMList.Count, 2.0)) + ConstantsExt.Eps(-6));
            double lowerLimit = _orgMList.Min() - _orgData.Precision*0.5;
            double upperLimit = _orgMList.Max() + _orgData.Precision*0.5;
            HistogramInfos.XRange = upperLimit - lowerLimit;
            double interval = HistogramInfos.XRange / HistogramInfos.GroupNum;

            HistogramInfos.XLabelList = new List<double>();
            HistogramInfos.XLabelList.Add(lowerLimit);
            for (int i = 0; i < HistogramInfos.GroupNum; i++)
            {
                HistogramInfos.XLabelList.Add(HistogramInfos.XLabelList[i] + interval);
            }
            dataWithLabel.AddRange(HistogramInfos.XLabelList);
            return dataWithLabel;
        }

        private void Grouping(List<double> dataWithLabel)
        {
            dataWithLabel.Sort();//sorted data: label0, d0, d1, ...,label1,..,labeln
            var labelLocation = HistogramInfos.XLabelList.Select(label => dataWithLabel.FindIndex(t => Math.Abs(t - label) < ConstantsExt.Eps())).ToList();

            HistogramInfos.FrequencyListList = new List<List<String>>();
            HistogramInfos.PlotCoordinateListList = new List<List<Point>>();
            int dataCurIndex = 0;//标记数据索引位置
            for (int i = 0; i < labelLocation.Count - 1; i++)
            {
                //每次先偏移一个label位置
                dataCurIndex++;
                int freq = labelLocation[i + 1] - labelLocation[i] - 1;
                //根据频数计算y轴坐标范围
                if (freq > HistogramInfos.YRange)
                {
                    HistogramInfos.YRange = freq;
                }
                //直方图标记
                HistogramInfos.FrequencyListList.Add(new List<string>());
                HistogramInfos.PlotCoordinateListList.Add(new List<Point>());
                for (int j = 0; j < freq; j++)
                {
                    HistogramInfos.FrequencyListList[i].Add(GetLabel(dataWithLabel[dataCurIndex]));
                    double pointX = (HistogramInfos.XLabelList[i + 1] + HistogramInfos.XLabelList[i])*0.5;
                    double pointY = j+0.5;
                    HistogramInfos.PlotCoordinateListList[i].Add(new Point(pointX, pointY));
                    dataCurIndex++;
                }
            }
        }

        private String GetLabel(double val)
        {
            String label = "";
            if (_labelDict.ContainsValue(val))
            {
                int loc = _labelDict.Values.ToList().FindIndex(v => Math.Abs(v - val) < ConstantsExt.Eps());
                label = _labelDict.Keys.ElementAt(loc);
                _labelDict.Remove(label);
            }
            return label;
        }

        private void SetHistogramPlotInfo()
        {
            HistogramInfos.SetValues(ConstStrings.HistogramHeader[_calculationInfo.Lang], ConstStrings.HistogramXLabel[_calculationInfo.Lang], ConstStrings.HistogramYLabel[_calculationInfo.Lang]);
        }

        #region 三类算法

        private void DoCalcWeighted(int groupId = 0)
        {
            _tTotal = Distributions.Dist_t(_dfList.Sum());
            CalcW();
            CalcMAveWeighted(_WList);
            CalcSmAveWeighted(_WList);
            ReliablityCalc();
            ReliablityCheck();
            CalcPEValues(groupId, _MAve - _SmAve * _tTotal, _MAve, _MAve + _SmAve * _tTotal);
        }

        private void DoCalcSemiWeighted(int groupId = 1)
        {
            _tTotal = 2.0;
            List<double> WCorList = CalcMAveCor();
            CalcMAveWeighted(WCorList);
            CalcSmAveWeighted(WCorList);
            CalcPEValues(groupId, _MAve - _SmAve * _tTotal, _MAve, _MAve + _SmAve * _tTotal);
        }

        private void DoCalcUnweighted(int groupId = 2)
        {
            _tTotal = Distributions.Dist_t(_actualRowNum - 1);
            CalcMAveUnweighted();
            CalcSmAveUnweighted();
            CalcPEValues(groupId, _MAve - _SmAve * _tTotal, _MAve, _MAve + _SmAve * _tTotal);
        }

        #endregion

        #region 创建表格

        private void CreatePETable()
        {
            PETableList = new List<PotencyEstimateTable>();
            for (int i = 0; i < CalcCase; i++)
            {
                PETableList.Add(new PotencyEstimateTable());
                PETableList[i].InitTable(CalcCase, i, _calculationInfo);
            }
            FillPETable(CalcCase);
        }

        private void CreateConclusionTable()
        {
            ConclusionTable.CreateTable(new List<string> { _checkResult }, _checkConclusion, _calculationInfo.Lang);
        }

        #endregion

        #region 计算置信区间

        private void CalcW()
        {
            _WList = new List<double>();
            for (int i = 0; i < _actualRowNum; i++)
            {
                if (_calculationInfo.DataSize.DoseNum == 5)
                {
                    double t = Distributions.Dist_t(_corData.Data[0][4][i]);
                    double l = Math.Abs(_corData.Data[0][3][i] - _corData.Data[0][1][i]);
                    _WList.Add(4.0*t*t/(l*l));
                }
                else
                {
                    _WList.Add(1.0/(_corData.Data[0][2][i]*_corData.Data[0][2][i]));
                }
            }
        }

        private void CalcMAveWeighted(List<double> wList)
        {
            double numerator = 0;
            double denominator = wList.Sum();
            for (int i = 0; i < _actualRowNum; i++)
            {
                numerator += wList[i]*_MList[i];
            }
            _MAve = numerator/denominator;
        }

        private void CalcMAveUnweighted()
        {
            _MAve = _MList.Average();
        }

        private List<double> CalcMAveCor()
        {
            CalcMAveUnweighted();
            double Sm2 = 0;
            for (int i = 0; i < _actualRowNum; i++)
            {
                Sm2 += (_MList[i] - _MAve)*(_MList[i] - _MAve);
            }
            Sm2 /= _actualRowNum*(_actualRowNum - 1);
            var WCor = new List<double>();
            for (int i = 0; i < _actualRowNum; i++)
            {
                WCor.Add(1.0/(1.0/_WList[i] + Sm2));
            }
            return WCor;
        }

        private double CalcSm2()
        {
            double Sm2 = 0;
            for (int i = 0; i < _actualRowNum; i++)
            {
                Sm2 += (_MList[i] - _MAve)*(_MList[i] - _MAve);
            }
            Sm2 /= _actualRowNum*(_actualRowNum - 1);
            return Sm2;
        }

        private void CalcSmAveWeighted(List<double> wList)
        {
            _SmAve = Math.Sqrt(1.0/wList.Sum());
        }

        private void CalcSmAveUnweighted()
        {
            _SmAve = Math.Sqrt(CalcSm2());
        }

        #endregion

        #region 可靠性检验

        private void ReliablityCalc()
        {
            _X2 = 0;
            for (int i = 0; i < _actualRowNum; i++)
            {
                _X2 += _WList[i]*(_MList[i] - _MAve)*(_MList[i] - _MAve);
            }
            _df = _actualRowNum - 1;
        }

        private void ReliablityCheck()
        {
            //先判断自由度是否都不小于6
            for (int i = 0; i < _actualRowNum; i++)
            {
                if (_dfList[i] < 6)
                {
                    _checkResult = ConstStrings.MergeCalcDfCheckFalseConclution[_calculationInfo.Lang][false][0];
                    _checkConclusion = ConstStrings.MergeCalcDfCheckFalseConclution[_calculationInfo.Lang][false][1];
                    return;
                }
            }
            //判断X2值
            _checkResult = ConstStrings.MergeCalcReliabilityCheckConclution[_calculationInfo.Lang][Distributions.Dist_X2(_df, _X2) > 0.05][0];
            _checkConclusion = ConstStrings.MergeCalcReliabilityCheckConclution[_calculationInfo.Lang][Distributions.Dist_X2(_df, _X2) > 0.05][1];
        }

        #endregion

        #region 效价计算及填表

        /// <summary>
        /// 计算3类效价值
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="lower"></param>
        /// <param name="est"></param>
        /// <param name="upper"></param>
        private void CalcPEValues(int groupId, double lower, double est, double upper)
        {
            //加权合并
            PEResults.PEValues.Add(new TreatPEValues());
            switch (_calculationInfo.DataTransFormula)
            {
                case DataTransformationFormula.Null:
                    PEResults.PEValues[groupId].Potency.Lower = lower;
                    PEResults.PEValues[groupId].Potency.Est = est;
                    PEResults.PEValues[groupId].Potency.Upper = upper;
                    break;
                case DataTransformationFormula.LogE:
                    PEResults.PEValues[groupId].Potency.Lower = VectorCalcMethodExt.Antiln(lower);
                    PEResults.PEValues[groupId].Potency.Est = VectorCalcMethodExt.Antiln(est);
                    PEResults.PEValues[groupId].Potency.Upper = VectorCalcMethodExt.Antiln(upper);
                    break;
                case DataTransformationFormula.Log10:
                    PEResults.PEValues[groupId].Potency.Lower = VectorCalcMethodExt.Antilg(lower);
                    PEResults.PEValues[groupId].Potency.Est = VectorCalcMethodExt.Antilg(est);
                    PEResults.PEValues[groupId].Potency.Upper = VectorCalcMethodExt.Antilg(upper);
                    break;
                default:
                    goto case DataTransformationFormula.Null;
            }
            //计算Rel. to Ass
            PEResults.PEValues[groupId].RelToAss.Lower = PEResults.PEValues[groupId].Potency.Lower/
                                                         _calculationInfo.UnitA[0].Val;
            PEResults.PEValues[groupId].RelToAss.Est = PEResults.PEValues[groupId].Potency.Est/
                                                       _calculationInfo.UnitA[0].Val;
            PEResults.PEValues[groupId].RelToAss.Upper = PEResults.PEValues[groupId].Potency.Upper/
                                                         _calculationInfo.UnitA[0].Val;
            //计算Rel. to Est
            PEResults.PEValues[groupId].RelToEst.Lower = PEResults.PEValues[groupId].RelToAss.Lower/
                                                         PEResults.PEValues[groupId].RelToAss.Est;
            PEResults.PEValues[groupId].RelToEst.Est = PEResults.PEValues[groupId].RelToAss.Est/
                                                       PEResults.PEValues[groupId].RelToAss.Est;
            PEResults.PEValues[groupId].RelToEst.Upper = PEResults.PEValues[groupId].RelToAss.Upper/
                                                         PEResults.PEValues[groupId].RelToAss.Est;

            PEResults.PEValues[groupId].SM = _SmAve;
        }

        private void CalcPEValuesOneRow(int groupId)
        {
            double lower = 0;
            double est = 0;
            double upper = 0;
            if (_calculationInfo.DataSize.DoseNum == 5)
            {
                lower = _corData.Data[0][1][0];
                est = _corData.Data[0][2][0];
                upper = _corData.Data[0][3][0];
            }
            else if (_calculationInfo.DataSize.DoseNum == 4)
            {
                double t = Distributions.Dist_t(_corData.Data[0][3][0]);
                double dSmAve = /*Math.Sqrt*/(_corData.Data[0][2][0]);
                est = _corData.Data[0][1][0];
                lower = est - dSmAve*t;
                upper = est + dSmAve*t;
            }
            CalcPEValues(groupId, lower, est, upper);
        }

        private void CalcPEValuesOneRows()
        {
            for (int i = 0; i < CalcCase; i++)
            {
                CalcPEValuesOneRow(i);
            }
        }

        /// <summary>
        /// 填充效价估计值
        /// </summary>
        /// <param name="groupId">组号</param>
        /// <param name="row">行号</param>
        /// <param name="val">值</param>
        /// <param name="precision">精度</param>
        /// <param name="isFilledWithNull">是否用空值填充</param>
        private void FillPEValues(int groupId, int row, BasicPEValues val, bool isFilledWithNull = false,
                                  String precision = "P2")
        {
            if (isFilledWithNull)
            {
                PETableList[groupId].Table.Cells[row][PETableList[groupId].Table.DataStartCol].Content = "";
                PETableList[groupId].Table.Cells[row][PETableList[groupId].Table.DataStartCol + 1].Content = "";
                PETableList[groupId].Table.Cells[row][PETableList[groupId].Table.DataStartCol + 2].Content = "";
            }
            else
            {
                PETableList[groupId].Table.Cells[row][PETableList[groupId].Table.DataStartCol].Content =
                    val.Lower.ToString(precision);
                PETableList[groupId].Table.Cells[row][PETableList[groupId].Table.DataStartCol + 1].Content =
                    val.Est.ToString(precision);
                PETableList[groupId].Table.Cells[row][PETableList[groupId].Table.DataStartCol + 2].Content =
                    val.Upper.ToString(precision);
            }

        }

        /// <summary>
        /// 填充表格
        /// </summary>
        /// <param name="h"></param>
        private void FillPETable(int h)
        {
            for (int i = 0; i < h; i++)
            {
                if (_calculationInfo.UnitA[0].Unit != "")
                {
                    PETableList[i].Table.Cells[3][0].Content += "(" + _calculationInfo.UnitA[0].Unit + ")";
                }
                FillPEValues(i, PETableList[i].Table.DataStartRow, PEResults.PEValues[i].Potency, false, "F4");
                FillPEValues(i, PETableList[i].Table.DataStartRow + 1, PEResults.PEValues[i].RelToAss,
                             _calculationInfo.UnitA[0].isEmpty);
                FillPEValues(i, PETableList[i].Table.DataStartRow + 2, PEResults.PEValues[i].RelToEst);
                PETableList[i].Table.Cells[PETableList[i].Table.DataStartRow + 3][1].Content =
                    PEResults.PEValues[i].SM.ToString("F10");
                PETableList[i].Table.Cells[PETableList[i].Table.DataStartRow + 4][1].Content =
                    PEResults.PEValues[i].CLPercent.ToString("P2");
            }
        }

        #endregion

        #region 分组计算内容

        private bool SplitData()
        {
            //分组数小于2个，不需要拆分计算
            if (GetSplitNumList() < 2)
            {
                return false;
            }
            DoDataSplit();
            return true;
        }

        private int GetSplitNumList()
        {
            _splitDict = new Dictionary<int, int>();
            for (int i = 0; i < _actualRowNum; i++)
            {
                var curId =(int)_corData.Data[0][0][i];
                //有该组信息，增加元素个数
                if (_splitDict.ContainsKey(curId))
                {
                    _splitDict[curId]++;
                }
                    //无该组信息，新建并将元素个数置成1
                else
                {
                    _splitDict.Add(curId, 1);
                }
            }
            return _splitDict.Count;
        }

        private void DoDataSplit()
        {
            _splitData = new List<AssayData>();
            int offSet = 0;
            for (int i = 0; i < _splitDict.Count; i++)
            {
                _splitData.Add(new AssayData());
                var size = new DataSize
                    {
                        PreparationNum = _calculationInfo.DataSize.PreparationNum,
                        DoseNum = _calculationInfo.DataSize.DoseNum,
                        ReplicateNum = _splitDict.ElementAt(i).Value
                    };
                _splitData[i].InitData(size);
                for (int j = 0; j < size.PreparationNum; j++)
                {
                    for (int k = 0; k < size.DoseNum; k++)
                    {
                        for (int l = 0; l < size.ReplicateNum; l++)
                        {
                            _splitData[i].Data[j][k][l] = _corData.Data[j][k][offSet + l];
                        }
                    }
                }
                offSet += size.ReplicateNum;
            }
        }

        #endregion

        private void AddPlotInfo()
        {
            if (PlotInfos == null)
            {
                PlotInfos = new List<MergeCalcPlotInfo>();
                for (int i = 0; i < CalcCase; i++)
                {
                    PlotInfos.Add(new MergeCalcPlotInfo());
                    PlotInfos[i].ConfidenceRangeInfos = new List<ConfidenceRangeInfo>();
                }
            }

            for (int i = 0; i < CalcCase; i++)
            {
                //工字图
                PlotInfos[i].ConfidenceRangeInfos.Add(new ConfidenceRangeInfo());
                int num = PlotInfos[i].ConfidenceRangeInfos.Count;
                PlotInfos[i].ConfidenceRangeInfos[num - 1].PEValues = PEResults.PEValues[i].Potency;
                //上下界虚线
                if (num == 1)
                {
                    PlotInfos[i].LimitsLines = new List<CurveDesc>();
                    PlotInfos[i].LimitsLines.Add(new CurveDesc());
                    PlotInfos[i].LimitsLines[0].PlotType = PlotTypes.DashedLine;
                    PlotInfos[i].LimitsLines[0].Color = Colors.Red;
                    PlotInfos[i].LimitsLines[0].FitPoints = new List<Point>();
                    PlotInfos[i].LimitsLines[0].FitPoints.Add(new Point(-1, PlotInfos[i].ConfidenceRangeInfos[0].PEValues.Upper));
                    PlotInfos[i].LimitsLines[0].FitPoints.Add(new Point(_splitDict.Count + 1, PlotInfos[i].ConfidenceRangeInfos[0].PEValues.Upper));
                    PlotInfos[i].LimitsLines.Add(new CurveDesc());
                    PlotInfos[i].LimitsLines[1].PlotType = PlotTypes.DashedLine;
                    PlotInfos[i].LimitsLines[1].Color = Colors.Red;
                    PlotInfos[i].LimitsLines[1].FitPoints = new List<Point>();
                    PlotInfos[i].LimitsLines[1].FitPoints.Add(new Point(-1, PlotInfos[i].ConfidenceRangeInfos[0].PEValues.Lower));
                    PlotInfos[i].LimitsLines[1].FitPoints.Add(new Point(_splitDict.Count + 1, PlotInfos[i].ConfidenceRangeInfos[0].PEValues.Lower));

                    PlotInfos[i].LimitsLines.Add(new CurveDesc());
                    PlotInfos[i].LimitsLines[2].PlotType = PlotTypes.SolidLine;
                    PlotInfos[i].LimitsLines[2].Color = Colors.Red;
                    PlotInfos[i].LimitsLines[2].FitPoints = new List<Point>();
                    PlotInfos[i].LimitsLines[2].FitPoints.Add(new Point(-1, PlotInfos[i].ConfidenceRangeInfos[0].PEValues.Est));
                    PlotInfos[i].LimitsLines[2].FitPoints.Add(new Point(_splitDict.Count + 1, PlotInfos[i].ConfidenceRangeInfos[0].PEValues.Est));

                    PlotInfos[i].ConfidenceRangeInfos[0].Color = Colors.Red;
                }
                //所有工字型都填完，计算坐标范围
                if (_splitDict.Count == 1 || num == _splitDict.Count + 1)
                {
                    double yMin = double.MaxValue;
                    double yMax = 0;
                    for (int j = 0; j < num; j++)
                    {
                        if (PlotInfos[i].ConfidenceRangeInfos[j].PEValues.Upper > yMax)
                        {
                            yMax = PlotInfos[i].ConfidenceRangeInfos[j].PEValues.Upper;
                        }
                        if (PlotInfos[i].ConfidenceRangeInfos[j].PEValues.Lower < yMin)
                        {
                            yMin = PlotInfos[i].ConfidenceRangeInfos[j].PEValues.Lower;
                        }
                    }
                    double yRange = (yMax - yMin)*1.2;
                    PlotInfos[i].SetValues(ConstStrings.MergeCalcTypes[_calculationInfo.Lang][i],
                                           ConstStrings.TestDepartmentNumber[_calculationInfo.Lang],
                                           ConstStrings.Potency[_calculationInfo.Lang], _splitDict.Count + 2, yRange);
                }
            }
        }
    }
}
