using Doan.Services;
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

namespace Doan
{
    /// <summary>
    /// Interaction logic for DangNhap.xaml
    /// </summary>
    public partial class DangNhap : Window
    {
        private DatabaseService _dbService;

        public DangNhap()
        {
            InitializeComponent();
            _dbService = new DatabaseService();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text.Trim();
            string password = PasswordBox.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập tên đăng nhập và mật khẩu!");
                return;
            }

            try
            {
                var user = _dbService.AuthenticateUser(username, password);

                if (user != null)
                {
                    MessageBox.Show($"Đăng nhập thành công! Chào {user.TenDangNhap}");

                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();

                    this.Close();
                }
                else
                {
                    MessageBox.Show("Sai tài khoản hoặc mật khẩu!");
                    PasswordBox.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}");
            }
        }
    }
}
