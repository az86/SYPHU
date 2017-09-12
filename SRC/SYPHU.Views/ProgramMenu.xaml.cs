using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SYPHU.ViewModels;
using SYPHU.Views.WizardControls;
using SYPHU.Assay.CalculationInfo;

namespace SYPHU.Views
{
    /// <summary>
    /// ProgramMenu.xaml 的交互逻辑
    /// </summary>
    public partial class ProgramMenu : UserControl
    {
        private SYPHU.ViewModels.ProgramMenuVM _vm;

        public ProgramMenu()
        {
            InitializeComponent();
            DataContextChanged += new DependencyPropertyChangedEventHandler(ProgramMenu_DataContextChanged);
        }

        private void ProgramMenu_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _vm = e.NewValue as SYPHU.ViewModels.ProgramMenuVM;
        }

        private void NewWizard(object sender, RoutedEventArgs e)
        {
            ShowWizard();
        }

        private void ModifyWizard(object sender, RoutedEventArgs e)
        {
            ShowWizard(false);
        }

        private void OpenDataFile(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.InitialDirectory = System.AppDomain.CurrentDomain.BaseDirectory + @"Examples\";
            dlg.DefaultExt = Configs.FileType.DefaultExt;
            dlg.Filter = SYPHU.Configs.FileType.Filter;
            if (dlg.ShowDialog() == true)
            {
                _vm.DeSerializeData(dlg.FileName);
            }
        }
        private void SaveAs(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.DefaultExt = Configs.FileType.DefaultExt;
            dlg.Filter = SYPHU.Configs.FileType.Filter;
            if (dlg.ShowDialog() == true)
            {
                _vm.SerializeData(dlg.FileName);
            }
        }

        private void SaveDataFile(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(MainWindowVM.Instance.CurrentFilePath))
            {
                SaveAs(sender, e);
            }
            else
            {
                _vm.SerializeData(MainWindowVM.Instance.CurrentFilePath);
            }
        }

        private void ShowWizard(bool beResetViewModel = true)
        {
            var guideWin = new Wizard();
            if (beResetViewModel)
            {
                _vm.WizardVM.Reset();
            }

            //修改向导时，计算类型不可选；新建向导时，计算类型可选
            _vm.WizardVM.ReportInformationVM.IsSingleBtnEnabled = beResetViewModel;
            _vm.WizardVM.ReportInformationVM.IsMergeBtnEnabled = beResetViewModel;

            _vm.WizardVM.TabSelectedIndex = 0;
            guideWin.DataContext = _vm.WizardVM;
            if (guideWin.ShowDialog() == true)
            {
                _vm.Initialize(beResetViewModel);
                MainWindowVM.Instance.CurrentFilePath = string.Empty;
            }            
        }

        private void DoDataPreprocess(object sender, RoutedEventArgs e)
        {
            _vm.CalcExceptionValues();
        }

        private void DoMainCalc(object sender, RoutedEventArgs e)
        {
            var method = new FinalAbnMethod {DataContext = _vm.FinalAbnMethod};
            if (method.ShowDialog() == true)
            {
                _vm.CalcResult();
            }
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        public event EventHandler PrintEvent;

        private void Print(object sender, RoutedEventArgs e)
        {
            PrintEvent.Invoke(sender, e);
        }

        private void UnDo(object sender, RoutedEventArgs e)
        {
            _vm.UnDo();
        }

        private void ReDo(object sender, RoutedEventArgs e)
        {
            _vm.ReDo();
        }

        private void RegisterClk(object sender, RoutedEventArgs e)
        {
            var regWin = new Register();
            regWin.ShowDialog();
        }

        private void AboutClick(object sender, RoutedEventArgs e)
        {
            var aboutDlg = new About {Owner = MainFrame.Instance.Parent as Window};
            aboutDlg.ShowDialog();
        }

        private void HelpClick(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("Help.pdf");
            }catch (Exception){}
        }

        private void CopyTableClick(object sender, RoutedEventArgs e)
        {
            _vm.CopyTable();
        }

        private void PasteTableClick(object sender, RoutedEventArgs e)
        {
            _vm.PasteTable();
        }
    }
}
