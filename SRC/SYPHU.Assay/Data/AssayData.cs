using System.Collections.Generic;
using System.Collections.ObjectModel;
using SYPHU.Assay.CalculationInfo;

namespace SYPHU.Assay.Data
{
    /// <summary>
    /// 实验数据
    /// </summary>
    public class AssayData
    {
        /// <summary>
        /// 数据
        /// </summary>
        public List<List<ObservableCollection<double>>> Data;

        /// <summary>
        /// 输入数据最小精度
        /// </summary>
        public double Precision = double.MaxValue;

        /// <summary>
        /// 根据数据大小初始化
        /// </summary>
        /// <param name="size"></param>
        public void InitData(DataSize size)
        {
            Data = new List<List<ObservableCollection<double>>>();
            for (int i = 0; i < size.PreparationNum; i++)
            {
                Data.Add(new List<ObservableCollection<double>>());
                for (int j = 0; j < size.DoseNum; j++)
                {
                    Data[i].Add(new ObservableCollection<double>());
                    for (int k = 0; k < size.ReplicateNum; k++)
                    {
                        Data[i][j].Add(0.0);
                    }
                }
            }
        }
    }
}
