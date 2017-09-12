using SYPHU.Assay.CalculationInfo;

namespace SYPHU.ViewModels.WizardControlsVM
{
    /// <summary>
    /// 数据大小，向导4：组数、行列数
    /// </summary>
    public class DataSizeInfoVM : VMBase
    {
        private DataSize _dataSize = new DataSize();

        public DataSize DataSize
        {
            get { return _dataSize; }
            set
            {
                _dataSize = value;
                NotifyPropertyChanged("DataSize");
            }
        }

        /// <summary>
        /// 实验组数：分表个数，向导4--新建按钮
        /// </summary>
        public int PreparationNum
        {
            get { return _dataSize.PreparationNum; }
            set { _dataSize.PreparationNum = value; }
        }

        /// <summary>
        /// 每组实验计量数：分表列数，向导4--新建按钮
        /// </summary>
        public int DoseNum
        {
            get { return _dataSize.DoseNum; }
            set { _dataSize.DoseNum = value; }
        }

        /// <summary>
        /// 每组实验样本数：分表行数，向导4--新建按钮
        /// </summary>
        public int ReplicateNum
        {
            get { return _dataSize.ReplicateNum; }
            set { _dataSize.ReplicateNum = value; }
        }
    }
}
