using System.Collections.Generic;
using SYPHU.Assay.CalculationInfo;

namespace SYPHU.ViewModels.WizardControlsVM
{
    /// <summary>
    /// 选择异常数据检测方法，可多选，可单选
    /// </summary>
    public class AbnormalDataCheckMethodVM : VMBase
    {
        private List<AbnormalDataCheckMethods> _abnDataCheckMethods = new List<AbnormalDataCheckMethods>();

        public List<AbnormalDataCheckMethods> AbnDataCheckMethods
        {
            get { return _abnDataCheckMethods; }
            set{ _abnDataCheckMethods = value; }
        }

        private bool _isNullBtnChecked;
        public bool IsNullBtnChecked
        {
            get { return AbnDataCheckMethods.Contains(AbnormalDataCheckMethods.Null); }
            set
            {
                _isNullBtnChecked = value;
                if (value)
                {
                    AbnDataCheckMethods.Add(AbnormalDataCheckMethods.Null);
                }
                else
                {
                    AbnDataCheckMethods.Remove(AbnormalDataCheckMethods.Null);
                }
                NotifyPropertyChanged("IsNullBtnChecked");
            }
        }

        private bool _isDixonBtnChecked;
        public bool IsDixonBtnChecked
        {
            get { return AbnDataCheckMethods.Contains(AbnormalDataCheckMethods.Dixon); }
            set
            {
                _isDixonBtnChecked = value;
                if (value)
                {
                    AbnDataCheckMethods.Add(AbnormalDataCheckMethods.Dixon);
                }
                else
                {
                    AbnDataCheckMethods.Remove(AbnormalDataCheckMethods.Dixon);
                }
                NotifyPropertyChanged("IsDixonBtnChecked");
            }
        }

        private bool _isGrubbBtnChecked;
        public bool IsGrubbBtnChecked
        {
            get { return AbnDataCheckMethods.Contains(AbnormalDataCheckMethods.Grubb); }
            set
            {
                _isGrubbBtnChecked = value;
                if (value)
                {
                    AbnDataCheckMethods.Add(AbnormalDataCheckMethods.Grubb);
                } 
                else
                {
                    AbnDataCheckMethods.Remove(AbnormalDataCheckMethods.Grubb);
                }
                NotifyPropertyChanged("IsGrubbBtnChecked");
            }
        }

        private bool _isRomanovskyBtnChecked;
        public bool IsRomanovskyBtnChecked
        {
            get { return AbnDataCheckMethods.Contains(AbnormalDataCheckMethods.Romanovsky); }
            set
            {
                _isRomanovskyBtnChecked = value;
                if (value)
                {
                    AbnDataCheckMethods.Add(AbnormalDataCheckMethods.Romanovsky);
                }
                else
                {
                    AbnDataCheckMethods.Remove(AbnormalDataCheckMethods.Romanovsky);
                }
                NotifyPropertyChanged("IsRomanovskyBtnChecked");
            }
        }

        private bool _isHampelBtnChecked;
        public bool IsHampelBtnChecked
        {
            get { return AbnDataCheckMethods.Contains(AbnormalDataCheckMethods.Hampel); }
            set
            {
                _isHampelBtnChecked = value;
                if (value)
                {
                    AbnDataCheckMethods.Add(AbnormalDataCheckMethods.Hampel);
                }
                else
                {
                    AbnDataCheckMethods.Remove(AbnormalDataCheckMethods.Hampel);
                }
                NotifyPropertyChanged("IsHampelBtnChecked");
            }
        }

        private bool _isQuartileBtnChecked;
        public bool IsQuartileBtnChecked
        {
            get { return AbnDataCheckMethods.Contains(AbnormalDataCheckMethods.Quartile); }
            set
            {
                _isQuartileBtnChecked = value;
                if (value)
                {
                    AbnDataCheckMethods.Add(AbnormalDataCheckMethods.Quartile);
                }
                else
                {
                    AbnDataCheckMethods.Remove(AbnormalDataCheckMethods.Quartile);
                }
                NotifyPropertyChanged("IsQuartileBtnChecked");
            }
        }
    }
}
