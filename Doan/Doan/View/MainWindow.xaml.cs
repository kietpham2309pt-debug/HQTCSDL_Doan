using System.Linq;
using System.Windows;
using System.Windows.Controls;
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

            // Gán lại MainWindow hiện hành để các ViewModel điều hướng tìm đúng DataContext.
            Application.Current.MainWindow = this;

            // Hiển thị đúng tên + vai trò người đang đăng nhập.
            if (!string.IsNullOrWhiteSpace(PhienDangNhap.TenDangNhap))
            {
                UserNameTextBlock.Text = PhienDangNhap.TenDangNhap;
            }
            if (!string.IsNullOrWhiteSpace(PhienDangNhap.Role))
            {
                RoleTextBlock.Text = PhienDangNhap.Role;
            }

            ApDungPhanQuyen();
        }

        // Ẩn các nút menu mà vai trò hiện tại không được phép truy cập.
        private void ApDungPhanQuyen()
        {
            string role = PhienDangNhap.Role;
            foreach (var nut in MenuPanel.Children.OfType<Button>())
            {
                string tab = nut.CommandParameter as string;
                if (string.IsNullOrEmpty(tab)) continue; // nút Đăng xuất (không có tham số) luôn hiện
                nut.Visibility = QuyenHan.Duoc(role, tab) ? Visibility.Visible : Visibility.Collapsed;
            }
        }
    }
}
