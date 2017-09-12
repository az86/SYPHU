using System;
using System.Collections.ObjectModel;
using SYPHU.Assay.CalculationInfo;
using SYPHU.Utilities;

namespace SYPHU.ViewModels.WizardControlsVM
{
    /// <summary>
    /// 统计模型
    /// </summary>
    public class MethodsVM : VMBase
    {
        public Methods Method;

        public bool IsDirectBtnChecked
        {
            get { return Method == Methods.Direct; }
            set
            {
                if (value)
                {
                    Method = Methods.Direct;
                }
                NotifyPropertyChanged("IsDirectBtnChecked");
            }
        }

        public bool IsParallelLineBtnChecked
        {
            get { return Method == Methods.ParallelLine; }
            set
            {
                if (value)
                {
                    Method = Methods.ParallelLine;
                }
                NotifyPropertyChanged("IsParallelLineBtnChecked");
            }
        }

        public bool IsSlopeRatioBtnChecked
        {
            get { return Method == Methods.SlopeRatio; }
            set
            {
                if (value)
                {
                    Method = Methods.SlopeRatio;
                }
                NotifyPropertyChanged("IsSlopeRatioBtnChecked");
            }
        }

        /// <summary>
        /// 原来有2个：s型曲线 质反应/量反应  现在就叫S型曲线了 
        /// 改成 IsSigmoidCurveBtnChecked 与下一个合并
        /// </summary>
        public bool IsSigmoidCurveBtnChecked
        {
            get { return Method == Methods.SigmoidCurve; }
            set
            {
                if (value)
                {
                    Method = Methods.SigmoidCurve;
                }
                NotifyPropertyChanged("IsSigmoidCurveGradedBtnChecked");
            }
        }

        public bool IsEDBtnChecked
        {
            get { return Method == Methods.ED; }
            set
            {
                if (value)
                {
                    Method = Methods.ED;
                }
                NotifyPropertyChanged("IsEDBtnChecked");
            }
        }

        private String _EDPercent  = "50";

        public String EDPercent
        {
            set
            {
                if (IsEDBtnChecked)
                {
                    _EDPercent = value;
                }
            }
            get { return _EDPercent; }
        }

        public String Checker()
        {
            try
            {
               ParseString.ParseEDString(EDPercent);
            }
            catch (Exception e)
            {
                return e.Message;
            }
            return null;
        }

        public ObservableCollection<String> EDs { get; private set; }

        public String SelectedED { get; set; }

        public MethodsVM()
        {
            SelectedED = Assay.Tables.ConstStrings.EDString[EDEnum.ED];
            EDs = new ObservableCollection<String>();
            foreach (var ed in Assay.Tables.ConstStrings.EDString.Keys)
            {
                EDs.Add(Assay.Tables.ConstStrings.EDString[ed]);
            }
        }
    }

}
