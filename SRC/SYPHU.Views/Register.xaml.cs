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
using System.Windows.Shapes;
using SYPHU.Exceptions;
using SYPHU.ViewModels;
using SYPHU.ViewModels.RegisterMgr;

namespace SYPHU.Views
{
    /// <summary>
    /// Register.xaml 的交互逻辑
    /// </summary>
    public partial class Register : Window
    {
        public Register()
        {
            InitializeComponent();
            DataContextChanged += new DependencyPropertyChangedEventHandler(Register_DataContextChanged);
            DataContext = RegisterVM.Instance;
        }

        void Register_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var vm = e.NewValue as RegisterVM;
            vm.ShowRegisterWnd += new EventHandler(Instance_ShowRegisterWnd);
        }

        void Instance_ShowRegisterWnd(object sender, EventArgs e)
        {
            ShowDialog();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (RegisterVM.Instance.OnClickOkBtn())
                {
                    MessageBox.Show("注册成功!");
                    DialogResult = true;
                }
                else
                {
                    MessageBox.Show("注册失败!");
                }
            }
            catch (ErrorRegCodeException)
            {
                MessageBox.Show("请输入正确的注册码");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        public bool Check()
        {
            var vm = DataContext as RegisterVM;
            return vm.Check();
        }
    }
}
