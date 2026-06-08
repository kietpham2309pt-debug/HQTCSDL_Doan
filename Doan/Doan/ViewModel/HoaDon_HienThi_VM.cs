using System;
using System.Collections.ObjectModel;

namespace Doan.ViewModel
{
    public class HoaDon_HienThi_VM
    {
        public string MaHD { get; set; }
        public DateTime? NgayLap { get; set; }
        public string TenNhanVien { get; set; }
        public string TenKhachHang { get; set; }
        public string SDT { get; set; }
        public string TenDV_SP { get; set; }
        public int? SoLuong { get; set; }
        public decimal? ThanhTien { get; set; }
        public string PhuongThucThanhToan { get; set; }
        public string TrangThai { get; set; }
        public string MaGiaoDich { get; set; }
    }

    // Một GIAO DỊCH = nhiều mặt hàng mua 1 lần (gộp theo MaGiaoDich).
    public class GiaoDich_HienThi_VM
    {
        public string MaGiaoDich { get; set; }
        public DateTime? NgayLap { get; set; }
        public string TenNhanVien { get; set; }
        public string TenKhachHang { get; set; }
        public string SDT { get; set; }
        public string PhuongThucThanhToan { get; set; }
        public string TrangThai { get; set; }
        public int SoMatHang { get; set; }
        public decimal TongTien { get; set; }
        public ObservableCollection<HoaDon_HienThi_VM> DanhSachMatHang { get; set; }
    }
}
