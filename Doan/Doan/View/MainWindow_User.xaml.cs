using System.Windows;
using Doan.Helper;

namespace Doan.View
{
    public partial class MainWindow_User : Window
    {
        public MainWindow_User()
        {
            // Cổng khách: khách vào thẳng không cần đăng nhập. Khởi tạo phiên khách vãng lai.
            PhienDangNhap.DatVeKhach();

            InitializeComponent();

            // Tương tự MainWindow: gán lại để cửa sổ này là MainWindow hiện hành.
            Application.Current.MainWindow = this;
        }
    }
}
