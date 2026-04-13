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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private User _currentUser;

        public MainWindow(User user = null)
        {
            InitializeComponent();
            _currentUser = user;

            // Khởi tạo
            InitializeWindow();

            // Load view mặc định
            LoadUserControl("TongQuan");
        }

        private void InitializeWindow()
        {
            if (_currentUser != null)
            {
                UserNameTextBlock.Text = _currentUser.HoTen ?? _currentUser.TenDangNhap;
            }
        }

        /// <summary>
        /// Xử lý click menu
        /// </summary>
        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                string tag = button.Tag?.ToString();
                LoadUserControl(tag);
            }
        }

        /// <summary>
        /// Load UserControl vào ContentControl
        /// </summary>
        private void LoadUserControl(string viewName)
        {
            try
            {
                UserControl userControl = null;

                switch (viewName)
                {
                    case "TongQuan":
                        //userControl = new TongQuanView();
                        break;
                    case "QuanLyXe":
                        userControl = new Views.BrandUserControl();
                        break;
                    case "HangXe":
                        userControl = new Views.BrandUserControl();
                        break;
                    case "KhachHang":
                        //userControl = new KhachHangView();
                        break;
                    case "NhanVien":
                        //userControl = new NhanVienView();
                        break;
                    case "DonHang":
                        //userControl = new DonHangView();
                        break;
                    case "ThanhToan":
                        //userControl = new ThanhToanView();
                        break;
                    case "ThongKe":
                        //userControl = new ThongKeView();
                        break;
                    case "CaiDat":
                        //userControl = new CaiDatView();
                        break;
                    default:
                        //userControl = new TongQuanView();
                        break;
                }

                MainContentControl.Content = userControl;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải view: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Đăng xuất
        /// </summary>
        private void DangXuat_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Bạn có muốn đăng xuất không?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                DangNhap loginWindow = new DangNhap();
                loginWindow.Show();
                this.Close();
            }
        }
    }
}
