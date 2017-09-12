using System;
using System.Windows;
using System.Windows.Input;

namespace SYPHU.Views.WizardControls
{
    /// <summary>
    /// Guide.xaml 的交互逻辑
    /// </summary>
    public partial class Wizard : Window
    {
        public Wizard()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as SYPHU.ViewModels.WizardControlsVM.WizardVM;
            
            if (vm.OnOkBtnClicked())
            {
                DialogResult = true;
            }
            
        }
    }
}
