using Doan.Model;
using System.Collections.ObjectModel;
using Doan.ViewModel;

namespace Doan.Helper
{
    public static class PhienDangNhap
    {
        public static KhachHang KhachHangHienTai { get; set; }
        public static string TenDangNhap { get; set; }
        public static string Role { get; set; }

        private static readonly ObservableCollection<MatHangGio_VM> gioHangKhach = new ObservableCollection<MatHangGio_VM>();
        public static ObservableCollection<MatHangGio_VM> GioHangKhach
        {
            get { return gioHangKhach; }
        }
    }
}
