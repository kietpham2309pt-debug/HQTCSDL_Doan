using System;
using System.Collections.Generic;
using System.Linq;

namespace Doan.Helper
{
    /// <summary>
    /// Phân quyền theo vai trò (chức vụ) của nhân viên.
    /// Ánh xạ Role -> các tab (CommandParameter) được phép truy cập trong khu quản trị.
    /// Quản lý (Admin) thấy tất cả; CaiDat và DangXuat luôn được phép cho mọi vai trò.
    /// </summary>
    public static class QuyenHan
    {
        private static string Chuan(string giaTri)
        {
            return (giaTri ?? string.Empty).Trim();
        }

        public static bool LaQuanLy(string role)
        {
            string r = Chuan(role);
            return r.Equals("Quản lý", StringComparison.OrdinalIgnoreCase)
                || r.Equals("Quan ly", StringComparison.OrdinalIgnoreCase)
                || r.Equals("Admin", StringComparison.OrdinalIgnoreCase)
                || r.Equals("Quản trị", StringComparison.OrdinalIgnoreCase);
        }

        // Các tab luôn cho phép.
        private static readonly HashSet<string> LuonChoPhep =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "CaiDat", "DangXuat" };

        // Bảng phân quyền theo từng vai trò (không tính Quản lý — Quản lý thấy tất cả).
        private static readonly Dictionary<string, HashSet<string>> BangQuyen =
            new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase)
            {
                ["Bán hàng"] = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                    { "QuanLyXe", "TatCaXe", "KhachHang", "ThanhToan", "LichSu", "TraGop", "ThongBao", "ThongKe" },

                // Kỹ thuật cũng bán phụ tùng/dịch vụ -> cần xem & nhập Khách hàng + Lịch sử giao dịch + Thanh toán.
                ["Kỹ thuật"] = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                    { "QuanLyXe", "TatCaXe", "DichVu", "PhuTung", "KhachHang", "ThanhToan", "LichSu", "TraGop", "NhapKho", "LichSuNhap", "ThongBao", "ThongKe" },

                ["Kế toán"] = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                    { "ThanhToan", "LichSu", "TraGop", "NhapKho", "LichSuNhap", "ThongBao", "ThongKe" },

                ["Chăm sóc khách hàng"] = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                    { "KhachHang", "LichSu", "TraGop", "ThongBao", "ThongKe" },
            };

        // Các chức vụ không liên quan đến nghiệp vụ trên app -> KHÔNG được đăng nhập.
        public static bool CamDangNhap(string role)
        {
            string r = Chuan(role);
            return r.Equals("Bảo vệ", StringComparison.OrdinalIgnoreCase)
                || r.Equals("Bao ve", StringComparison.OrdinalIgnoreCase)
                || r.Equals("Tạp vụ", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>Vai trò <paramref name="role"/> có được phép vào tab <paramref name="tab"/> không.</summary>
        public static bool Duoc(string role, string tab)
        {
            if (string.IsNullOrWhiteSpace(tab)) return true;
            if (LuonChoPhep.Contains(tab)) return true;
            if (LaQuanLy(role)) return true;

            HashSet<string> tabs;
            if (BangQuyen.TryGetValue(Chuan(role), out tabs))
            {
                return tabs.Contains(tab);
            }
            return false;
        }

        /// <summary>Tab mặc định hợp lệ đầu tiên cho vai trò (màn hình hiển thị ngay sau đăng nhập).</summary>
        public static string TabMacDinh(string role)
        {
            string[] uuTien = { "QuanLyXe", "KhachHang", "ThanhToan", "LichSu", "DichVu", "NhapKho", "ThongKe", "ThongBao", "CaiDat" };
            return uuTien.FirstOrDefault(t => Duoc(role, t)) ?? "ThongKe";
        }
    }
}
