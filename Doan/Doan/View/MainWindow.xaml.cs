using System;
using System.Windows;
using System.Windows.Shapes;
using Doan.Helper;

namespace Doan.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Cửa sổ đăng nhập mới là MainWindow lúc khởi động (StartupUri), nên sau khi
            // mở cửa sổ chính cần gán lại để các ViewModel điều hướng tìm đúng DataContext.
            Application.Current.MainWindow = this;

            // Hiển thị đúng tên người đang đăng nhập thay vì để cứng "Quản Trị Viên".
            if (!string.IsNullOrWhiteSpace(PhienDangNhap.TenDangNhap))
            {
                UserNameTextBlock.Text = PhienDangNhap.TenDangNhap;
            }
        }
    }
}
