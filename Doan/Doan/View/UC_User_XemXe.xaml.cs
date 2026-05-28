using Doan.ViewModel;
using System.Windows.Controls;

namespace Doan.View
{
    public partial class UC_User_XemXe : UserControl
    {
        public UC_User_XemXe()
        {
            InitializeComponent();
            DataContext = new User_XemXe_VM();
        }

        // Khởi tạo và lọc sẵn theo hãng (khi khách bấm hãng ở Trang chủ).
        public UC_User_XemXe(string maHang)
        {
            InitializeComponent();
            DataContext = new User_XemXe_VM(maHang);
        }
    }
}
