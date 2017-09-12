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

namespace SYPHU
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string BaseTitle = "ReguStats v1.0";

        public MainWindow()
        {
            InitializeComponent();
            this.DataContextChanged += new DependencyPropertyChangedEventHandler(MainWindow_DataContextChanged);
            DataContext = SYPHU.ViewModels.MainWindowVM.Instance;
        }

        void MainWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var vm = e.NewValue as SYPHU.ViewModels.MainWindowVM;
            if (vm != null)
            {
                vm.FileChangedEvent += new ViewModels.MainWindowVM.FileChangedEventHandler<object, string>(vm_FileChangedEvent);
            }
        }

        void vm_FileChangedEvent(object sender, string path)
        {
            Title = BaseTitle + " - " + path;
        }
    }
}
