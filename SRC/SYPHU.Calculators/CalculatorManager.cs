using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using SYPHU.Assay.CalculationInfo;
using SYPHU.Assay.Plots;
using SYPHU.Assay.Tables;

namespace SYPHU.Calculators
{
    /// <summary>
    /// 计算器管理
    /// </summary>
    public class CalculatorManager
    {
        /// <summary>
        /// 输出的表格列表
        /// </summary>
        public readonly TableList TablesShownList = new TableList();

        /// <summary>
        /// 画图信息
        /// </summary>
        public List<PlotInfo> Plots;

        /// <summary>
        /// 合并计算工字图
        /// </summary>
        public List<MergeCalcPlotInfo> MergeCalcPlotInfos;

        /// <summary>
        /// 合并计算频数分布图
        /// </summary>
        public Histogram HistogramInfo;

        /// <summary>
        /// 异常数据检测结果
        /// </summary>
        public List<List<String>> ExtremeAbnormalCheckResults;

        private String _errorMsg;

        #region 私有成员

        /// <summary>
        /// 计算信息，to save
        /// </summary>
        private InitCalculationInfo _calculationInfo;

        public InitCalculationInfo CalculationInfo
        {
            get { return _calculationInfo; }
        }

        /// <summary>
        /// 程序信息等表，to save
        /// </summary>
        private CommonInfoTable _commonInfoTable;

        /// <summary>
        /// 计算信息表
        /// </summary>
        private CalcInfoTable _calcInfoTable;

        /// <summary>
        /// 原始数据表，to save
        /// </summary>
        private OrigDataTable _origDataTable;

        /// <summary>
        /// 转换后数据表
        /// </summary>
        private TranDataTable _tranDataTable;

        /// <summary>
        /// 拉丁方法数据转换表
        /// </summary>
        private LatinConvertTable _latinConvertTable;

        /// <summary>
        /// 拉丁方法标准数据表
        /// </summary>
        private LatinDataTable _latinDataTable;

        /// <summary>
        /// 校正后数据表
        /// </summary>
        private CorrDataTable _corrDataTable;

        /// <summary>
        /// 数据处理类
        /// </summary>
        private DataProcess _dataProcess;

        /// <summary>
        /// 标示是否进行了异常处理
        /// </summary>
        private bool _isAbnormalDataProcessDone;

        /// <summary>
        /// 计算器
        /// </summary>
        private AbstractCalculator _calculator;

        #endregion

        /// <summary>
        /// 向导结束后要对计算信息初始化
        /// </summary>
        /// <param name="lang">语言</param>
        /// <param name="size">数据大小</param>
        /// <param name="calcCase">计算类型</param>
        /// <param name="method">统计模型</param>
        /// <param name="design">设计类型</param>
        /// <param name="model">计算方法</param>
        /// <param name="type">反应类型</param>
        /// <param name="formula">内嵌转换公式</param>
        /// <param name="checkMethods">异常数据检测方法列表</param>
        /// <param name="edString">ED字符串</param>
        /// <param name="percent">ED法的百分比</param>
        /// <param name="userDefinedFormula">用户自定义的公式字符串</param>
        /// <param name="isNew">是否是新建</param>
        /// <param name="product">药品名称</param>
        public void InitCalcInfo(String product, OutLang lang, DataSize size, CalcCases calcCase, Methods method,
                                 Designs design, Models model, Types type,
                                 DataTransformationFormula formula,
                                 List<AbnormalDataCheckMethods> checkMethods, String edString, String percent,
                                 String userDefinedFormula = "", bool isNew = false)
        {
            Plots = new List<PlotInfo>();
            MergeCalcPlotInfos = new List<MergeCalcPlotInfo>();
            HistogramInfo = new Histogram();
            ExtremeAbnormalCheckResults = new List<List<string>>();
            _calculationInfo = new InitCalculationInfo();
            _calculationInfo.Init(product, lang, size, calcCase, method, design, model, type, formula, checkMethods, edString, percent, userDefinedFormula);
            TablesShownList.Clear(isNew);
        }

        /// <summary>
        /// 向导结束按钮
        /// 创建表格：基本信息表，计算信息表，原始数据表
        /// </summary>
        public void WizardFinish()
        {
            _commonInfoTable = new CommonInfoTable();
            _commonInfoTable.CreateTable(_calculationInfo.Lang);
            _calcInfoTable = new CalcInfoTable();
            _calcInfoTable.CreateTable(_calculationInfo);
            _origDataTable = new OrigDataTable();
            _origDataTable.CreateTable(_calculationInfo);
            if (_calculationInfo.Design == Designs.LatinSquare)
            {
                _latinConvertTable = new LatinConvertTable();
                _latinConvertTable.InitTable(_calculationInfo);
            }
            AddTableToList();
        }

        /// <summary>
        /// 正序列化
        /// </summary>
        /// <param name="fs"></param>
        public void Serialize(FileStream fs)
        {
            var bf = new BinaryFormatter();
            bf.Serialize(fs, _calculationInfo);
            bf.Serialize(fs, _commonInfoTable);
            bf.Serialize(fs, _calcInfoTable);
            if (_calculationInfo.Design == Designs.LatinSquare)
            {
                bf.Serialize(fs, _latinConvertTable);
            }
            bf.Serialize(fs, _origDataTable);
        }

        /// <summary>
        /// 逆序列化
        /// </summary>
        /// <param name="fs"></param>
        public void Deserialize(FileStream fs)
        {
            var bf = new BinaryFormatter();
            _calculationInfo = bf.Deserialize(fs) as InitCalculationInfo;
            _commonInfoTable = bf.Deserialize(fs) as CommonInfoTable;
            _calcInfoTable = bf.Deserialize(fs) as CalcInfoTable;
            if (_calculationInfo != null && _calculationInfo.Design == Designs.LatinSquare)
            {
                _latinConvertTable = bf.Deserialize(fs) as LatinConvertTable;
            }
            _origDataTable = bf.Deserialize(fs) as OrigDataTable;
            //清除所有数据
            TablesShownList.Clear(true);
            Plots = new List<PlotInfo>();
            MergeCalcPlotInfos = new List<MergeCalcPlotInfo>();
            HistogramInfo = new Histogram();
            ExtremeAbnormalCheckResults = new List<List<string>>();
            //Plots.Clear();
            //ExtremeAbnormalCheckResults.Clear();
            AddTableToList();
        }

        /// <summary>
        /// 异常值检测按钮：进行数据转换，异常值检测
        /// 创建表格：转换后数据表
        /// </summary>
        public String AbnormalDataProcess()
        {
            _isAbnormalDataProcessDone = true;
            TablesShownList.RemoveResultTables();
            if (_calculationInfo == null)
            {
                _errorMsg = "参数初始化失败，未进行参数设置.";
                return _errorMsg;
            }
            //原始数据表格解析
            _errorMsg = _origDataTable.Parse(_calculationInfo);
            if (_errorMsg != null)
            {
                return _errorMsg;
            }
            //数据转换
            _tranDataTable = new TranDataTable();
            _tranDataTable.TranData.InitData(_calculationInfo.DataSize);
            _dataProcess = new DataProcess();
            _errorMsg = _dataProcess.DataTransformation(_calculationInfo, _origDataTable.OrigData, _tranDataTable.TranData);
            if (_errorMsg != null)
            {
                return _errorMsg;
            }
            _tranDataTable.CreateTable(_calculationInfo, _origDataTable);
            //拉丁方数据转换
            _latinDataTable = new LatinDataTable();
            if (_calculationInfo.Design == Designs.LatinSquare)
            {
                _latinDataTable.LatinData.InitData(_calculationInfo.DataSize);
                _errorMsg = _dataProcess.LatinDataConvert(_tranDataTable.TranData, _latinDataTable.LatinData,
                                                          _latinConvertTable);
                if (_errorMsg != null)
                {
                    return _errorMsg;
                }
                _latinDataTable.CreateTable(_calculationInfo, _origDataTable);
            }
            //检测异常值
            ExtremeAbnormalCheckResults = _dataProcess.AbnormalDataCheck(_tranDataTable.TranData,
                                                                  _calculationInfo.AbnDataCheckMethods);

            TablesShownList.AddMultiTables(_tranDataTable.Tables);

            if (_calculationInfo.Design == Designs.LatinSquare)
            {
                TablesShownList.AddMultiTables(_latinDataTable.Tables);
            }
            return null;
        }

        /// <summary>
        /// 生物统计计算按钮，如果异常值检测按钮未点击，此处自动执行
        /// 创建表格：异常值处理后表格，方差分析表，效价估计表
        /// </summary>
        public String DoCalculate()
        {
            if (_isAbnormalDataProcessDone == false)
            {
                _errorMsg = AbnormalDataProcess();
            }
            _isAbnormalDataProcessDone = false;

            if (_errorMsg != null)
            {
                return _errorMsg;
            }

            //修改异常值检测方法显示内容
            _calcInfoTable.UpdateAbnDataCheckMethod(_calculationInfo);

            ExtremeAbnormalCheckResults = _dataProcess.AbnormalDataCheck(_tranDataTable.TranData,
                                                                  _calculationInfo.AbnDataCheckMethods);
            _corrDataTable = new CorrDataTable();
            _corrDataTable.CorrData.InitData(_calculationInfo.DataSize);
            _dataProcess.AbnormalDataProcess(_tranDataTable.TranData, _latinDataTable.LatinData, _corrDataTable.CorrData, _calculationInfo);
            _corrDataTable.CreateTable(_dataProcess.ExtremeAbnormalDataLocationList, _calculationInfo, _tranDataTable);
            TablesShownList.AddMultiTables(_corrDataTable.Tables);

            if (_calculationInfo.CalcCase == CalcCases.Single)
            {
                SelectCalculator();
                _calculator.LoadCalcData(_corrDataTable.CorrData);
                _calculator.DoCalculate(_calculationInfo, _dataProcess.TotalExtremeAbnormalDataNum, _dataProcess.ListAveB);
                _calculator.CreateTable();
                AddVAandPETable();
                _calculator.CreatePlotInfo(_calculationInfo.Design == Designs.CrossOver ? 2 : _calculationInfo.DataSize.PreparationNum);
                Plots = _calculator.PlotsInfo;

                #region 四种方法对比图

                if (_calculationInfo.Method == Methods.ED)
                {
                    var fullCalculator = new EDFullCalculator();
                    Plots[0].CurveDescs[0].Color =
                        ConstStrings.ColorList[fullCalculator.ModelList.IndexOf(_calculationInfo.Model)];
                    fullCalculator.LoadCalcData(_corrDataTable.CorrData);
                    fullCalculator.DoFullCalculate(_calculationInfo);
                    Plots.Add(fullCalculator.PlotsInfo);
                }

                #endregion

            }
            else if (_calculationInfo.CalcCase == CalcCases.Merge)
            {
                var calculator = new MergeCalculator();
                calculator.LoadCalcData(_corrDataTable.CorrData, _origDataTable.OrigData);
                calculator.DoCalculate(_calculationInfo, _dataProcess.ExtremeAbnormalDataLocationList.Count);
                TablesShownList.AddSingleTable(calculator.ConclusionTable.Table);
                TablesShownList.AddMultiTables(calculator.PETableList);
                HistogramInfo = calculator.HistogramInfos;
                MergeCalcPlotInfos = calculator.PlotInfos;
            }

            return null;
        }

        #region 私有方法

        private void AddTableToList()
        {
            TablesShownList.AddSingleTable(_commonInfoTable.Table);
            TablesShownList.AddSingleTable(_calcInfoTable.Table);
            if (_calculationInfo.Design == Designs.LatinSquare)
            {
                TablesShownList.AddLatinConvertTable(_latinConvertTable.Table);
            }
            else
            {
                TablesShownList.RemoveTable(TableCategory.LatinConvertTable);
            }
            if (_calculationInfo.CalcCase == CalcCases.Single)
            {
                TablesShownList.AddOrigDataTables(_origDataTable.Tables);
            }
            else if (_calculationInfo.CalcCase == CalcCases.Merge)
            {
                TablesShownList.AddMergeCalcOrigDataTables(_origDataTable.Tables);
            }
        }

        private void SelectCalculator()
        {
            switch (_calculationInfo.Method)
            {
                case Methods.Direct:
                    _calculator = new DirectMensurationCalculator();
                    break;
                case Methods.ParallelLine:
                    switch (_calculationInfo.Design)
                    {
                        case Designs.CompletelyRandomised:
                            _calculator = new PLCompletelyRandomisedCalculator();
                            break;
                        case Designs.RandomisedBlock:
                            _calculator = new PLRandomisedBlockCalculator();
                            break;
                        case Designs.LatinSquare:
                            _calculator = new PLLatinSquareCalculator();
                            break;
                        case Designs.CrossOver:
                            _calculator= new PLCrossOverCalculator();
                            break;
                    }
                    break;
                case Methods.SlopeRatio:
                    switch (_calculationInfo.Design)
                    {
                        case Designs.CompletelyRandomised:
                            _calculator = new SRCompletelyRandomisedCalculator();
                            break;
                        case Designs.RandomisedBlock:
                            _calculator = new SRRandomisedBlockCalculator();
                            break;
                        case Designs.LatinSquare:
                            _calculator = new SRLatinSquareCalculator();
                            break;
                    }
                    break;
                case Methods.SigmoidCurve:
                    switch (_calculationInfo.Type)
                    {
                        case Types.Graded:
                            _calculator = new SCGradedCalculator();
                            break;
                        case Types.Quantal:
                            _calculator = new SCQuantalCalculator();
                            break;
                    }
                    break;
                case Methods.ED:
                    _calculator = new EDCalculator();
                    break;
            }
        }

        private void AddVAandPETable()
        {
            switch (_calculationInfo.Method)
            {
                case Methods.Direct:
                    TablesShownList.AddSingleTable(((DirectMensurationCalculator)_calculator).VarAnaTable.Table);
                    TablesShownList.AddMultiTables(((DirectMensurationCalculator)_calculator).PETableList);
                    break;
                default:
                    TablesShownList.AddSingleTable(_calculator.VATable.Table);
                    TablesShownList.AddSingleTable(_calculator.ConclusionTable.Table);
                    TablesShownList.AddMultiTables(_calculator.PETableList);
                    break;
            }
        }

        #endregion

    }
}
