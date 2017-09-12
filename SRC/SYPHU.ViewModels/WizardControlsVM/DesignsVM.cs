using SYPHU.Assay.CalculationInfo;

namespace SYPHU.ViewModels.WizardControlsVM
{
    /// <summary>
    /// 向导2：实验设计
    /// </summary>
    public class DesignsVM : VMBase
    {
        public void Update(ReportInformationVM informationVM, MethodsVM method)
        {
            if (informationVM.CalcCase == CalcCases.Merge)
            {
                IsCompletelyRandomisedBtnChecked = true;
                IsCompletelyRandomisedBtnEnabled = false;
                IsRandomisedBlockBtnEnabled = false;
                IsLatinSquareBtnEnabled = false;
                IsCrossOverBtnEnabled = false;
                return;
            }
            if (method.IsDirectBtnChecked)
            {
                IsCompletelyRandomisedBtnChecked = true;
                IsCompletelyRandomisedBtnEnabled = false;
                IsRandomisedBlockBtnEnabled = false;
                IsLatinSquareBtnEnabled = false;
                IsCrossOverBtnEnabled = false;
            }
            else if (method.IsParallelLineBtnChecked)
            {
                IsCompletelyRandomisedBtnEnabled = true;
                IsRandomisedBlockBtnEnabled = true;
                IsLatinSquareBtnEnabled = true;
                IsCrossOverBtnEnabled = true;
            }
            else if (method.IsSlopeRatioBtnChecked)
            {
                if (IsCrossOverBtnChecked)
                {
                    IsCompletelyRandomisedBtnChecked = true;
                }
                IsCompletelyRandomisedBtnEnabled = true;
                IsRandomisedBlockBtnEnabled = true;
                IsLatinSquareBtnEnabled = true;
                IsCrossOverBtnEnabled = false;
            }
            else if (method.IsSigmoidCurveBtnChecked)
            {
                IsCompletelyRandomisedBtnChecked = true;
                IsCompletelyRandomisedBtnEnabled = true;
                IsRandomisedBlockBtnEnabled = false;
                IsLatinSquareBtnEnabled = false;
                IsCrossOverBtnEnabled = false;
            }
            else if (method.IsEDBtnChecked)
            {
                IsCompletelyRandomisedBtnChecked = true;
                IsCompletelyRandomisedBtnEnabled = true;
                IsRandomisedBlockBtnEnabled = false;
                IsLatinSquareBtnEnabled = false;
                IsCrossOverBtnEnabled = false;
            }
        }

        public Designs Design { get; set; }

        public bool IsCompletelyRandomisedBtnChecked
        {
            get { return Design == Designs.CompletelyRandomised; }
            set
            {
                if (value)
                {
                    Design = Designs.CompletelyRandomised;
                }
                NotifyPropertyChanged("IsCompletelyRandomisedBtnChecked");
            }
        }

        private bool _isCompletelyRandomisedBtnEnabled;
        public bool IsCompletelyRandomisedBtnEnabled
        {
            get { return _isCompletelyRandomisedBtnEnabled; }
            set
            {
                _isCompletelyRandomisedBtnEnabled = value;
                NotifyPropertyChanged("IsCompletelyRandomisedBtnEnabled");
            }
        }

        public bool IsRandomisedBlockBtnChecked
        {
            get { return Design == Designs.RandomisedBlock; }
            set
            {
                if (value)
                {
                    Design = Designs.RandomisedBlock;
                }
                NotifyPropertyChanged("IsRandomisedBlockBtnChecked");
            }
        }

        private bool _isRandomisedBlockBtnEnabled;
        public bool IsRandomisedBlockBtnEnabled
        {
            get { return _isRandomisedBlockBtnEnabled; }
            set
            {
                _isRandomisedBlockBtnEnabled = value;
                NotifyPropertyChanged("IsRandomisedBlockBtnEnabled");
            }
        }

        public bool IsLatinSquareBtnChecked
        {
            get { return Design == Designs.LatinSquare; }
            set
            {
                if (value)
                {
                    Design = Designs.LatinSquare;
                }
                NotifyPropertyChanged("IsLatinSquareBtnChecked");
            }
        }

        private bool _isLatinSquareBtnEnabled;
        public bool IsLatinSquareBtnEnabled
        {
            get { return _isLatinSquareBtnEnabled; }
            set
            {
                _isLatinSquareBtnEnabled = value;
                NotifyPropertyChanged("IsLatinSquareBtnEnabled");
            }
        }

        public bool IsCrossOverBtnChecked
        {
            get { return Design == Designs.CrossOver; }
            set
            {
                if (value)
                {
                    Design = Designs.CrossOver;
                }
                NotifyPropertyChanged("IsCrossOverBtnChecked");
            }
        }

        private bool _isCrossOverBtnEnabled;
        public bool IsCrossOverBtnEnabled
        {
            get { return _isCrossOverBtnEnabled; }
            set
            {
                _isCrossOverBtnEnabled = value;
                NotifyPropertyChanged("IsCrossOverBtnEnabled");
            }
        }
    }
}
