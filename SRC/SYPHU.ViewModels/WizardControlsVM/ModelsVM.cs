using SYPHU.Assay.CalculationInfo;

namespace SYPHU.ViewModels.WizardControlsVM
{
    /// <summary>
    /// 向导3：计算方法
    /// </summary>
    public class CalculationModelVM : VMBase
    {
        public void Update(ReportInformationVM informationVM, MethodsVM method)
        {
            //直接法无计算方法
            if (informationVM.CalcCase == CalcCases.Merge)
            {
                _models = Models.Null;
                IsProbitBtnChecked = false;
                IsLogitBtnChecked = false;
                IsGompitBtnChecked = false;
                IsAngleBtnChecked = false;
                IsFourParasBtnChecked = false;
                IsProbitBtnEnabled = false;
                IsLogitBtnEnabled = false;
                IsGompitBtnEnabled = false;
                IsAngleBtnEnabled = false;
                IsFourParasBtnEnabled = false;
                return;
            }
            if (method.IsDirectBtnChecked)
            {
                _models = Models.Null;
                IsProbitBtnChecked = false;
                IsLogitBtnChecked = false;
                IsGompitBtnChecked = false;
                IsAngleBtnChecked = false;
                IsFourParasBtnChecked = false;
                IsProbitBtnEnabled = false;
                IsLogitBtnEnabled = false;
                IsGompitBtnEnabled = false;
                IsAngleBtnEnabled = false;
                IsFourParasBtnEnabled = false;
            }
                //平行线法无计算方法
            else if (method.IsParallelLineBtnChecked)
            {
                _models = Models.Null;
                IsProbitBtnChecked = false;
                IsLogitBtnChecked = false;
                IsGompitBtnChecked = false;
                IsAngleBtnChecked = false;
                IsFourParasBtnChecked = false;
                IsProbitBtnEnabled = false;
                IsLogitBtnEnabled = false;
                IsGompitBtnEnabled = false;
                IsAngleBtnEnabled = false;
                IsFourParasBtnEnabled = false;
            }
                //斜率比法无计算方法
            else if (method.IsSlopeRatioBtnChecked)
            {
                _models = Models.Null;
                IsProbitBtnChecked = false;
                IsLogitBtnChecked = false;
                IsGompitBtnChecked = false;
                IsAngleBtnChecked = false;
                IsFourParasBtnChecked = false;
                IsProbitBtnEnabled = false;
                IsLogitBtnEnabled = false;
                IsGompitBtnEnabled = false;
                IsAngleBtnEnabled = false;
                IsFourParasBtnEnabled = false;
            }
                //S型曲线法默认为Probit
            else if (method.IsSigmoidCurveBtnChecked)
            {
                if (_models == Models.Null)
                {
                    IsProbitBtnChecked = true;
                }
                
                IsProbitBtnEnabled = true;
                IsLogitBtnEnabled = true;
                IsGompitBtnEnabled = true;
                IsAngleBtnEnabled = true;
                //暂时改为false
                IsFourParasBtnEnabled = false;
            }
                //ED法默认为Probit
            else if (method.IsEDBtnChecked)
            {
                if (_models == Models.Null || _models == Models.FourPara)
                {
                    IsProbitBtnChecked = true;
                }
                IsProbitBtnEnabled = true;
                IsLogitBtnEnabled = true;
                IsGompitBtnEnabled = true;
                IsAngleBtnEnabled = true;
                IsFourParasBtnEnabled = false;
            }
        }

        private Models _models = Models.Null;

        public Models Model { get { return _models; }
            set { _models = value; }}

        public bool IsProbitBtnChecked
        {
            get { return Model == Models.Probit; }
            set
            {
                if (value)
                {
                    Model = Models.Probit;
                }
                NotifyPropertyChanged("IsProbitBtnChecked");
            }
        }

        private bool _isProbitBtnEnabled;
        public bool IsProbitBtnEnabled
        {
            get { return _isProbitBtnEnabled; }
            set
            {
                _isProbitBtnEnabled = value;
                NotifyPropertyChanged("IsProbitBtnEnabled");
            }
        }

        public bool IsLogitBtnChecked
        {
            get { return Model == Models.Logit; }
            set
            {
                if (value)
                {
                    Model = Models.Logit;
                }
                NotifyPropertyChanged("IsLogitBtnChecked");
            }
        }

        private bool _isLogitBtnEnabled;
        public bool IsLogitBtnEnabled
        {
            get { return _isLogitBtnEnabled; }
            set
            {
                _isLogitBtnEnabled = value;
                NotifyPropertyChanged("IsLogitBtnEnabled");
            }
        }

        public bool IsGompitBtnChecked
        {
            get { return Model == Models.Gompit; }
            set
            {
                if (value)
                {
                    Model = Models.Gompit;
                }
                NotifyPropertyChanged("IsGompitBtnChecked");
            }
        }

        private bool _isGompitBtnEnabled;
        public bool IsGompitBtnEnabled
        {
            get { return _isGompitBtnEnabled; }
            set
            {
                _isGompitBtnEnabled = value;
                NotifyPropertyChanged("IsGompitBtnEnabled");
            }
        }

        public bool IsAngleBtnChecked
        {
            get { return Model == Models.Angle; }
            set
            {
                if (value)
                {
                    Model = Models.Angle;
                }
                NotifyPropertyChanged("IsAngleBtnChecked");
            }
        }

        private bool _isAngleBtnEnabled;
        public bool IsAngleBtnEnabled
        {
            get { return _isAngleBtnEnabled; }
            set
            {
                _isAngleBtnEnabled = value;
                NotifyPropertyChanged("IsAngleBtnEnabled");
            }
        }

        public bool IsFourParasBtnChecked
        {
            get { return Model == Models.FourPara; }
            set
            {
                if (value)
                {
                    Model = Models.FourPara;
                }
                NotifyPropertyChanged("IsFourParasBtnChecked");
            }
        }

        private bool _isFourParasBtnEnabled;
        public bool IsFourParasBtnEnabled
        {
            get { return _isFourParasBtnEnabled; }
            set
            {
                _isFourParasBtnEnabled = value;
                NotifyPropertyChanged("IsFourParasBtnEnabled");
            }
        }
    }
}
