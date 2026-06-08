using Doan.Model;
using System;
using System.Collections.ObjectModel;
using Doan.ViewModel;

namespace Doan.Helper
{
    public static class PhienDangNhap
    {
        public static KhachHang KhachHangHienTai { get; set; }
        public static string TenDangNhap { get; set; }
        public static string Role { get; set; }

        // Nhân viên đang đăng nhập (null nếu là khách hoặc tài khoản admin mặc định).
        public static NhanVien NhanVienHienTai { get; set; }

        // Khách vãng lai (không đăng nhập) hoặc chưa có vai trò nhân viên.
        public static bool LaKhach
        {
            get { return string.IsNullOrEmpty(Role) || string.Equals(Role, "Khach", StringComparison.OrdinalIgnoreCase); }
        }

        // Đưa phiên về trạng thái khách vãng lai.
        public static void DatVeKhach()
        {
            TenDangNhap = null;
            Role = "Khach";
            NhanVienHienTai = null;
            KhachHangHienTai = null;
            GioHangKhach.Clear();
        }

        private static readonly ObservableCollection<MatHangGio_VM> gioHangKhach = new ObservableCollection<MatHangGio_VM>();
        public static ObservableCollection<MatHangGio_VM> GioHangKhach
        {
            get { return gioHangKhach; }
        }
    }
}
