using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using SYPHU.Assay.CalculationInfo;
using SYPHU.Calculators;
using SYPHU.ViewModels.WizardControlsVM;

namespace SYPHU.ViewModels
{
    public class ProgramMenuVM : VMBase
    {   
        public delegate void CalcEventHandler<T, A>(T sender, A args);

        /// <summary>
        /// 序列化数据事件类型
        /// </summary>
        /// <param name="filePath">文件路径</param>
        public delegate void SerializeEventHandler<T, A>(T sender, A filePath);

        /// <summary>
        /// 序列化数据事件类型
        /// </summary>
        /// <param name="beReset">是否重置</param>
        public delegate void InitializeEventHandler<T, A>(T sender, A beReset);

        /// <summary>
        /// 更新视图的事件
        /// </summary>
        public event InitializeEventHandler<object,bool> InitializeEvent;

        /// <summary>
        /// 撤销操作
        /// </summary>
        public event EventHandler UnDoEvent;

        /// <summary>
        /// 重做操作
        /// </summary>
        public event EventHandler ReDoEvent;

        /// <summary>
        /// 异常值计算
        /// </summary>
        public event CalcEventHandler<object, EventArgs> CalcExceptionValuesEvent;

        /// <summary>
        /// 执行数据计算
        /// </summary>
        public event CalcEventHandler<object, EventArgs> CalcResultEvent;

        /// <summary>
        /// 序列化程序数据
        /// </summary>
        public event SerializeEventHandler<object, string> SerializeDataEvent;

        /// <summary>
        /// 反序列换程序数据
        /// </summary>
        public event SerializeEventHandler<object, string> DeSerializeDataEvent;

        /// <summary>
        /// 复制表
        /// </summary>
        public event EventHandler CopyTableEvent;

        /// <summary>
        /// 粘贴表
        /// </summary>
        public event EventHandler PasteTableEvent;

        private WizardVM _wizardVM = new WizardVM();

        public WizardVM WizardVM
        {
            get { return _wizardVM; }
            private set
            {
                WizardVM = value;
                NotifyPropertyChanged("WizardVM");
            }
        }

        private bool _isModifyEnable;

        public bool IsModifyEnable
        {
            get { return _isModifyEnable; }
            set
            { 
                _isModifyEnable = value;
                NotifyPropertyChanged("IsModifyEnable");
            }
        }

        private FinalAbnMethod _finalAbnMethod = new FinalAbnMethod();

        private bool _isExceptionCalcOk;

        public bool IsExceptionCalcOk
        {
            get { return _isExceptionCalcOk; }
            private set
            { 
                _isExceptionCalcOk = value;
                NotifyPropertyChanged("IsExceptionCalcOk");
            }
        }

        public FinalAbnMethod FinalAbnMethod
        {
            get { return _finalAbnMethod; }
            set { _finalAbnMethod = value; }
        }

        public void SerializeData(string filePath)
        {
            try
            {
                SerializeDataEvent.Invoke(this, filePath);
            }
            catch (Exception)
            {
                var fileInfo = new FileInfo(filePath);
                if (fileInfo.Length == 0)
                {
                    File.Delete(filePath);
                }
            }
        }

        public void DeSerializeData(string filePath)
        {
            try
            {
                DeSerializeDataEvent.Invoke(this, filePath);
            }
            catch (Exception)
            {
                return;
            }
            
            IsModifyEnable = true;
        }

        public void Initialize(bool beReset)
        {
            try
            {
                InitializeEvent.Invoke(this, beReset);
            }
            catch (Exception)
            {
                return;
            }
            
            IsModifyEnable = true;
        }

        public void CalcExceptionValues()
        {
            CalcExceptionValuesEvent.Invoke(this, null);
            IsExceptionCalcOk = true;
        }

        public void CalcResult()
        {
            WizardVM.AbnormalDataCheckMethodVM.AbnDataCheckMethods.Clear();
            WizardVM.AbnormalDataCheckMethodVM.AbnDataCheckMethods.AddRange(FinalAbnMethod.AbnDataCheckMethods);
            CalcResultEvent.Invoke(this, null);
        }

        public void UnDo()
        {
            UnDoEvent.Invoke(this, null);
        }

        public void ReDo()
        {
            ReDoEvent.Invoke(this, null);
        }

        public void CopyTable()
        {
            CopyTableEvent.Invoke(this, null);
        }

        public void PasteTable()
        {
            PasteTableEvent.Invoke(this, null);
        }
    }
}