using System;
using System.Linq;
using SYPHU.ViewModels;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace SYPHU.Views
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class MainFrame : UserControl
    {
        public static MainFrame Instance { get; private set; }

        private MainFrameVM _vm;

        public MainFrame()
        {
            InitializeComponent();
            Instance = this;
            menu.PrintEvent += new System.EventHandler(menu_PrintEvent);
        }

        void menu_PrintEvent(object sender, System.EventArgs e)
        {
            client.ElementMgr.Print();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            var regChecker = new Register { DataContext = RegisterVM.Instance };
            if (!regChecker.Check())
            {
                Application.Current.Shutdown(-1);
            }
            if (_vm != null)
            {
                _vm.OnWindowLoaded();
            }
            
        }

        private void Grid_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _vm = e.NewValue as MainFrameVM;
        }
    }
}
