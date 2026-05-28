using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Doan.View
{
    /// <summary>
    /// Interaction logic for W_DangNhap.xaml
    /// </summary>
    public partial class W_DangNhap : Window
    {
        private bool dangHienMatKhau;

        public W_DangNhap()
        {
            InitializeComponent();
            this.DataContext = new Doan.ViewModel.DangNhap_VM();
        }

        private void TogglePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            if (!dangHienMatKhau)
            {
                PasswordTextBox.Text = PasswordBox.Password;
                PasswordTextBox.Visibility = Visibility.Visible;
                PasswordBox.Visibility = Visibility.Collapsed;
                TogglePasswordButton.Content = "🙈";
                dangHienMatKhau = true;
                return;
            }

            PasswordBox.Password = PasswordTextBox.Text;
            PasswordBox.Visibility = Visibility.Visible;
            PasswordTextBox.Visibility = Visibility.Collapsed;
            TogglePasswordButton.Content = "👁";
            dangHienMatKhau = false;
        }
    }
}
