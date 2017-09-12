using System;
using SYPHU.Assay.CalculationInfo;
namespace SYPHU.ViewModels.WizardControlsVM
{
    public class WizardVM : VMBase
    {
        private ReportInformationVM _reportInformationVM = new ReportInformationVM();

        public ReportInformationVM ReportInformationVM
        {
            get { return _reportInformationVM; }
            set { _reportInformationVM = value; }
        }

        private MethodsVM _methodsVM = new MethodsVM();

        public MethodsVM MethodsVM
        {
            get { return _methodsVM; }
            set
            {
                _methodsVM = value;
                NotifyPropertyChanged("MethodsVM");
            }
        }

        private TypesVM _typesVM = new TypesVM();

        public TypesVM TypesVM
        {
            get { return _typesVM; }
            set
            {
                _typesVM = value;
                NotifyPropertyChanged("TypesVM");
            }
        }

        private DesignsVM _designsVM = new DesignsVM();

        public DesignsVM DesignsVM
        {
            get { return _designsVM; }
            set
            {
                _designsVM = value;
                NotifyPropertyChanged("DesignsVM");
            }
        }

        private CalculationModelVM _calculationModelVM = new CalculationModelVM();

        public CalculationModelVM CalculationModelVM
        {
            get { return _calculationModelVM; }
            set
            {
                _calculationModelVM = value;
                NotifyPropertyChanged("CalculationModelVM");
            }
        }

        private DataSizeInfoVM _dataInfoVM = new DataSizeInfoVM();

        public DataSizeInfoVM DataInfoVM
        {
            get { return _dataInfoVM; }
            set
            {
                _dataInfoVM = value;
                NotifyPropertyChanged("DataInfoVM");
            }
        }

        private DataTransformationFormulaVM _dataTransformationFormulaVM = new DataTransformationFormulaVM();

        public DataTransformationFormulaVM DataTransformationFormulaVM
        {
            get { return _dataTransformationFormulaVM; }
            set
            {
                _dataTransformationFormulaVM = value;
                NotifyPropertyChanged("DataTransformationFormulaVM");
            }
        }

        private AbnormalDataCheckMethodVM _abnormalDataCheckMethodVM = new AbnormalDataCheckMethodVM();

        public AbnormalDataCheckMethodVM AbnormalDataCheckMethodVM
        {
            get { return _abnormalDataCheckMethodVM; }
            set
            {
                _abnormalDataCheckMethodVM = value;
                NotifyPropertyChanged("AbnormalDataCheckMethodVM");
            }
        }

        private int _tabSelectedIndex;

        public int TabSelectedIndex
        {
            get { return _tabSelectedIndex; }
            set
            {
                _tabSelectedIndex = value;
                NotifyPropertyChanged("TabSelectedIndex");
                DesignsVM.Update(ReportInformationVM, MethodsVM);
                CalculationModelVM.Update(ReportInformationVM, MethodsVM);
                TypesVM.Update(ReportInformationVM, MethodsVM);
                DataTransformationFormulaVM.Update(ReportInformationVM);
            }
        }

        private bool GuidChecker(int guidId)
        {
            String errorMsg;
            switch (guidId)
            {
                case 0:
                    errorMsg = _reportInformationVM.Checker();
                    break;
                    //method
                case 1:
                    errorMsg = _methodsVM.Checker();
                    break;
                    //数据大小
                case 5:
                    errorMsg = _dataInfoVM.DataSize.Checker(ReportInformationVM.CalcCase, MethodsVM.Method, DesignsVM.Design, TypesVM.Type);
                    break;
                    //公式
                case 6:
                    errorMsg = _dataTransformationFormulaVM.UserDefinedFormulaChecker();
                    break;
                default:
                    errorMsg = null;
                    break;
            }

            if (errorMsg != null)
            {
                MainFrameVM.OutputWinVM.ShowInformation(errorMsg);
                return false;
            }
            return true;
        }

        public void NextBtnClicked()
        {
            System.Diagnostics.Debug.WriteLine("NextBtnClicked");
            if (TabSelectedIndex != 7 && GuidChecker(TabSelectedIndex))
            {
                TabSelectedIndex++;
            }
        }

        public void BackBtnClicked()
        {
            System.Diagnostics.Debug.WriteLine("BackBtnClicked");
            if (TabSelectedIndex != 0)
            {
                TabSelectedIndex--;
            }
        }
        
        public void SetInitInfo(InitCalculationInfo initInfo)
        {
            DataInfoVM.DataSize = initInfo.DataSize;
            MethodsVM.Method = initInfo.Method;
            MethodsVM.EDPercent = initInfo.EDPercent;
            TypesVM.Type = initInfo.Type;
            DesignsVM.Design = initInfo.Design;
            CalculationModelVM.Model = initInfo.Model;
            AbnormalDataCheckMethodVM.AbnDataCheckMethods = initInfo.AbnDataCheckMethods;
            DataTransformationFormulaVM.DataTransFormula = initInfo.DataTransFormula;
            DataTransformationFormulaVM.UserDefinedFormula = initInfo.UserDefinedFormula;
            ReportInformationVM.CalcCase = initInfo.CalcCase;
            ReportInformationVM.Lang = initInfo.Lang;
            ReportInformationVM.ProductName = initInfo.ProductName;
        }

        public void Reset()
        {
            SetInitInfo(new InitCalculationInfo());
            TabSelectedIndex = 0;
        }

        public bool OnOkBtnClicked()
        {
            //从0开始确认有效性
            TabSelectedIndex = 0;
            while (TabSelectedIndex != 7)
            {
                if (GuidChecker(TabSelectedIndex))
                {
                    TabSelectedIndex++;
                }
                else
                {
                    return false;                    
                }
            }
            return true;
        }
    }
}
