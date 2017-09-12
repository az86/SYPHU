using System;
using SYPHU.Assay.CalculationInfo;

namespace SYPHU.ViewModels.WizardControlsVM
{
    public class ReportInformationVM : VMBase
    {
        public CalcCases CalcCase { get; set; }

        public bool IsSingleBtnChecked
        {
            get { return CalcCase == CalcCases.Single; }
            set
            {
                if (value)
                {
                    CalcCase = CalcCases.Single;
                }
                NotifyPropertyChanged("IsSingleBtnChecked");
            }
        }

        private bool _isSingleBtnEnabled;
        public bool IsSingleBtnEnabled
        {
            get { return _isSingleBtnEnabled; }
            set
            {
                _isSingleBtnEnabled = value;
                NotifyPropertyChanged("IsSingleBtnEnabled");
            }
        }

        public bool IsMergeBtnChecked
        {
            get { return CalcCase == CalcCases.Merge; }
            set
            {
                if (value)
                {
                    CalcCase = CalcCases.Merge;
                }
                NotifyPropertyChanged("IsMergeBtnChecked");
            }
        }

        private bool _isMergeBtnEnabled;
        public bool IsMergeBtnEnabled
        {
            get { return _isMergeBtnEnabled; }
            set
            {
                _isMergeBtnEnabled = value;
                NotifyPropertyChanged("IsMergeBtnEnabled");
            }
        }

        private String _productName;

        public String ProductName
        {
            get { return _productName; }
            set { _productName = value; }
        }

        public String Checker()
        {
            if (String.IsNullOrEmpty(_productName))
            {
                return "请输入药品名称.";
            }
            return null;
        }

        public OutLang Lang { get; set; }

        public bool IsChineseBtnChecked
        {
            get { return Lang == OutLang.Chinese; }
            set
            {
                if (value)
                {
                    Lang = OutLang.Chinese;
                }
                NotifyPropertyChanged("IsChineseBtnChecked");
            }
        }

        public bool IsEnglishBtnChecked
        {
            get { return Lang == OutLang.English; }
            set
            {
                if (value)
                {
                    Lang = OutLang.English;
                }
                NotifyPropertyChanged("IsEnglishBtnChecked");
            }
        }
    }
}
