using System;
using System.Linq;
using SYPHU.Assay.CalculationInfo;
using SYPHU.Extentions;
using SYPHU.Calculators;
using System.IO;

namespace SYPHU.ViewModels
{
    public class MainFrameVM
    {
        private readonly ProgramMenuVM _programMenuVM = new ProgramMenuVM();
        public ProgramMenuVM ProgramMenuVM { get { return _programMenuVM; } }

        private readonly ClientVM _clientVM = new ClientVM();
        public ClientVM ClientVM { get { return _clientVM; } }

        private static readonly OutputWinVM _outputWinVM = new OutputWinVM();
        public static OutputWinVM OutputWinVM { get { return _outputWinVM; } }

        private readonly CalculatorManager _calculatorManager = new CalculatorManager();

        public MainFrameVM()
        {
            ProgramMenuVM.InitializeEvent += ProgramMenuVM_InitializeEvent;
            ProgramMenuVM.SerializeDataEvent += ProgramMenuVM_SerializeDataEvent;
            ProgramMenuVM.DeSerializeDataEvent += ProgramMenuVM_DeSerializeDataEvent;
            ProgramMenuVM.CalcExceptionValuesEvent += ProgramMenuVM_CalcExceptionValuesEvent;
            ProgramMenuVM.CalcResultEvent += ProgramMenuVM_CalcResultEvent;
            ProgramMenuVM.UnDoEvent += (sender, e) => ClientVM.UnDo();
            ProgramMenuVM.ReDoEvent += (sender, e) => ClientVM.ReDo();
            ProgramMenuVM.CopyTableEvent += (sender, e) => ClientVM.CopyTable();
            ProgramMenuVM.PasteTableEvent += (sender, e) => ClientVM.PasteTable();
        }

        /// <summary>
        /// 初始化，向导结束之后执行此函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="beReset">新建向导为true，修改向导为false</param>
        void ProgramMenuVM_InitializeEvent(object sender, bool beReset)
        {
            _calculatorManager.InitCalcInfo(ProgramMenuVM.WizardVM.ReportInformationVM.ProductName,
                                            ProgramMenuVM.WizardVM.ReportInformationVM.Lang,
                                            ProgramMenuVM.WizardVM.DataInfoVM.DataSize,
                                            ProgramMenuVM.WizardVM.ReportInformationVM.CalcCase,
                                            ProgramMenuVM.WizardVM.MethodsVM.Method,
                                            ProgramMenuVM.WizardVM.DesignsVM.Design,
                                            ProgramMenuVM.WizardVM.CalculationModelVM.Model,
                                            ProgramMenuVM.WizardVM.TypesVM.Type,
                                            ProgramMenuVM.WizardVM.DataTransformationFormulaVM.DataTransFormula,
                                            ProgramMenuVM.WizardVM.AbnormalDataCheckMethodVM.AbnDataCheckMethods,
                                            ProgramMenuVM.WizardVM.MethodsVM.SelectedED,
                                            ProgramMenuVM.WizardVM.MethodsVM.EDPercent,
                                            ProgramMenuVM.WizardVM.DataTransformationFormulaVM.UserDefinedFormula,
                                            beReset);
            ProgramMenuVM.FinalAbnMethod.AbnDataCheckMethods.Clear();
            _calculatorManager.WizardFinish();
            ClientVM.UpdateElements(_calculatorManager.TablesShownList.List);
            OutputWinVM.Clear();
        }

        /// <summary>
        /// 执行计算，取得结果
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void ProgramMenuVM_CalcResultEvent(object sender, EventArgs args)
        {
            OutputWinVM.Clear();
            var errorMsg =  _calculatorManager.DoCalculate();
            if (errorMsg != null)
            {
                OutputWinVM.ShowInformation(errorMsg);
                return;
            }
            OutputWinVM.ShowInformation(_calculatorManager.ExtremeAbnormalCheckResults);
            ClientVM.UpdateElements(_calculatorManager.TablesShownList.List);
            if (_calculatorManager.CalculationInfo.CalcCase == CalcCases.Single)
            {
                ClientVM.UpdateElements(_calculatorManager.Plots);
            }
            else if (_calculatorManager.CalculationInfo.CalcCase == CalcCases.Merge)
            {
                ClientVM.UpdateElements(_calculatorManager.HistogramInfo);
                ClientVM.UpdateElements(_calculatorManager.MergeCalcPlotInfos);
            }
        }

        /// <summary>
        /// 异常值计算
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void ProgramMenuVM_CalcExceptionValuesEvent(object sender, EventArgs args)
        {
            OutputWinVM.Clear();
            String errorMsg = _calculatorManager.AbnormalDataProcess();
            if (errorMsg != null)
            {
                OutputWinVM.ShowInformation(errorMsg);
                return;
            }

            ClientVM.UpdateElements(_calculatorManager.TablesShownList.List);
            OutputWinVM.ShowInformation(_calculatorManager.ExtremeAbnormalCheckResults);
        }

        /// <summary>
        /// 读取数据文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="filePath"></param>
        void ProgramMenuVM_DeSerializeDataEvent(object sender, string filePath)
        {
            using (var fs = new FileStream(filePath, FileMode.Open))
            {
                try
                {
                    _calculatorManager.Deserialize(fs);
                }
                catch (Exception)
                {
                    OutputWinVM.ShowInformation("文件 " + filePath + " 打开失败.");
                    throw new Exception("文件打开失败.");
                }
                
                ProgramMenuVM.WizardVM.SetInitInfo(_calculatorManager.CalculationInfo);
                ClientVM.UpdateElements(_calculatorManager.TablesShownList.List);
                OutputWinVM.Clear();
                MainWindowVM.Instance.CurrentFilePath = filePath;
            }
        }

        /// <summary>
        /// 保存数据文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="filePath"></param>
        void ProgramMenuVM_SerializeDataEvent(object sender, string filePath)
        {
            using (var fs = new FileStream(filePath, FileMode.Create))
            {
                try
                {
                    _calculatorManager.Serialize(fs);
                }
                catch (Exception ex)
                {
                    OutputWinVM.ShowInformation("文件 " + filePath + " 保存失败.");
                    throw new Exception("文件保存失败." + ex.Message);
                }
                
                MainWindowVM.Instance.CurrentFilePath = filePath;
            }
        }

        public void OnWindowLoaded()
        {
            var regInfo = new FileTypeRegInfo { Description = "SYPHU Files", ExePath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName, ExtendName = ".syphu" };
            FileTypeRegister.RegisterFileType(regInfo);
            FileTypeRegister.UpdateFileTypeRegInfo(regInfo);
            var args = Environment.GetCommandLineArgs();
            if (args.Count() == 2)
            {
                Console.WriteLine(args[1]);
                ProgramMenuVM.DeSerializeData(args[1]);
            }
        }
    }
}
