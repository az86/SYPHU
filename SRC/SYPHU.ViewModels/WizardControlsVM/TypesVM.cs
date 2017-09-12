using SYPHU.Assay.CalculationInfo;

namespace SYPHU.ViewModels.WizardControlsVM
{
    public class TypesVM : VMBase
    {
        public void Update(ReportInformationVM informationVM, MethodsVM method)
        {
            if (informationVM.CalcCase == CalcCases.Merge)
            {
                IsGradedBtnChecked = true;
                IsGradedBtnEnabled = false;
                IsQuantalBtnEnabled = false;
                return;
            }
            if (method.IsSigmoidCurveBtnChecked)
            {
                IsQuantalBtnChecked = true;
                IsGradedBtnEnabled = false;
                IsQuantalBtnEnabled = false;
            }
            else if (method.IsEDBtnChecked)
            {
                IsQuantalBtnChecked = true;
                IsGradedBtnEnabled = false;
                IsQuantalBtnEnabled = true;
            }
            else
            {
                IsGradedBtnChecked = true;
                IsGradedBtnEnabled = false;
                IsQuantalBtnEnabled = false;
            }
        }

        private bool _isGradedBtnEnabled;

        public bool IsGradedBtnEnabled
        {
            get { return _isGradedBtnEnabled; }
            set
            {
                _isGradedBtnEnabled = value;
                NotifyPropertyChanged("IsGradedBtnEnabled");
            }
        }

        private bool _isQuantalBtnEnabled;

        public bool IsQuantalBtnEnabled
        {
            get { return _isQuantalBtnEnabled; }
            set
            {
                _isQuantalBtnEnabled = value;
                NotifyPropertyChanged("IsQuantalBtnEnabled");
            }
        }

        public Types Type;

         public bool IsGradedBtnChecked
        {
            get { return Type == Types.Graded; }
            set
            {
                if (value)
                {
                    Type = Types.Graded;
                }
                NotifyPropertyChanged("IsGradedBtnChecked");
            }
        }

        public bool IsQuantalBtnChecked
        {
            get { return Type == Types.Quantal; }
            set
            {
                if (value)
                {
                    Type = Types.Quantal;
                }
                NotifyPropertyChanged("IsQuantalBtnChecked");
            }
        }
    }
}