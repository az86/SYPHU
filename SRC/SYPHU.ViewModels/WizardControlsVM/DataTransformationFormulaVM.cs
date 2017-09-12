using System;
using SYPHU.Assay.CalculationInfo;
using SYPHU.Utilities;

namespace SYPHU.ViewModels.WizardControlsVM
{
    /// <summary>
    /// 数据转换公式：内嵌公式、用户自定义公式----加入字符串公式
    /// </summary>
    public class DataTransformationFormulaVM : VMBase
    {
        public void Update(ReportInformationVM reportInfo)
        {
            if (reportInfo.IsSingleBtnChecked)
            {
                IsSquareBtnEnabled = true;
                IsSquareRootBtnEnabled = true;
                IsReciprocalBtnEnabled = true;
                IsUserDefinedBtnEnabled = true;
            }
            if (reportInfo.IsMergeBtnChecked)
            {
                if (IsSquareBtnChecked || IsSquareRootBtnChecked || IsReciprocalBtnChecked || IsUserDefinedBtnChecked)
                {
                    IsNullBtnChecked = true;
                }
                IsSquareBtnEnabled = false;
                IsSquareRootBtnEnabled = false;
                IsReciprocalBtnEnabled = false;
                IsUserDefinedBtnEnabled = false;
            }
        }

        public DataTransformationFormula DataTransFormula { get; set; }

        private String _userDefinedFormula;

        public string UserDefinedFormula
        {
            set 
            {
                _userDefinedFormula = IsUserDefinedBtnChecked ? value : "";
            }
            get
            {
                return _userDefinedFormula;
            }
        }

        public bool IsNullBtnChecked
        {
            get { return DataTransFormula == DataTransformationFormula.Null; }
            set
            {
                if (value)
                {
                    DataTransFormula = DataTransformationFormula.Null;
                }
                NotifyPropertyChanged("IsNullBtnChecked");
            }
        }

        public bool IsLogEBtnChecked
        {
            get { return DataTransFormula == DataTransformationFormula.LogE; }
            set
            {
                if (value)
                {
                    DataTransFormula = DataTransformationFormula.LogE;
                }
                NotifyPropertyChanged("IsLogEBtnChecked");
            }
        }

        public bool IsLog10BtnChecked
        {
            get { return DataTransFormula == DataTransformationFormula.Log10; }
            set
            {
                if (value)
                {
                    DataTransFormula = DataTransformationFormula.Log10;
                }
                NotifyPropertyChanged("IsLog10BtnChecked");
            }
        }

        public bool IsSquareBtnChecked
        {
            get { return DataTransFormula == DataTransformationFormula.Square; }
            set
            {
                if (value)
                {
                    DataTransFormula = DataTransformationFormula.Square;
                }
                NotifyPropertyChanged("IsSquareBtnChecked");
            }
        }

        private bool _isSquareBtnEnabled;
        public bool IsSquareBtnEnabled
        {
            get { return _isSquareBtnEnabled; }
            set
            {
                _isSquareBtnEnabled = value;
                NotifyPropertyChanged("IsSquareBtnEnabled");
            }
        }

        public bool IsSquareRootBtnChecked
        {
            get { return DataTransFormula == DataTransformationFormula.SquareRoot; }
            set
            {
                if (value)
                {
                    DataTransFormula = DataTransformationFormula.SquareRoot;
                }
                NotifyPropertyChanged("IsSquareRootBtnChecked");
            }
        }

        private bool _isSquareRootBtnEnabled;
        public bool IsSquareRootBtnEnabled
        {
            get { return _isSquareRootBtnEnabled; }
            set
            {
                _isSquareRootBtnEnabled = value;
                NotifyPropertyChanged("IsSquareRootBtnEnabled");
            }
        }

        public bool IsReciprocalBtnChecked
        {
            get { return DataTransFormula == DataTransformationFormula.Reciprocal; }
            set
            {
                if (value)
                {
                    DataTransFormula = DataTransformationFormula.Reciprocal;
                }
                NotifyPropertyChanged("IsReciprocalBtnChecked");
            }
        }

        private bool _isReciprocalBtnEnabled;
        public bool IsReciprocalBtnEnabled
        {
            get { return _isReciprocalBtnEnabled; }
            set
            {
                _isReciprocalBtnEnabled = value;
                NotifyPropertyChanged("IsReciprocalBtnEnabled");
            }
        }

        public bool IsUserDefinedBtnChecked
        {
            get { return DataTransFormula == DataTransformationFormula.UserDefined; }
            set
            {
                if (value)
                {
                    DataTransFormula = DataTransformationFormula.UserDefined;
                }
                NotifyPropertyChanged("IsUserDefinedBtnChecked");
            }
        }

        private bool _isUserDefinedBtnEnabled;
        public bool IsUserDefinedBtnEnabled
        {
            get { return _isUserDefinedBtnEnabled; }
            set
            {
                _isUserDefinedBtnEnabled = value;
                NotifyPropertyChanged("IsUserDefinedBtnEnabled");
            }
        }

        public String UserDefinedFormulaChecker()
        {
            if (IsUserDefinedBtnChecked)
            {
                var rpn = new ReversePolishNotation();
                if (!rpn.IsValid(UserDefinedFormula) || !rpn.Parse())
                {
                    return "自定义的公式无法解析.";
                }
            }
            else
            {
                UserDefinedFormula = "";
            }
            return null;
        }
    }
}
