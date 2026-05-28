using System;
using System.Collections.Generic;

namespace Doan.ViewModel
{
    // Một "đơn" khách đặt online = nhóm các dòng HoaDon cùng (MaKH, NgayLap)
    // đang ở trạng thái "Chờ xác nhận".
    public class DonChoXuLy_VM
    {
        public string MaKH { get; set; }
        public string TenKhachHang { get; set; }
        public string SDT { get; set; }
        public string DiaChi { get; set; }
        public DateTime? NgayLap { get; set; }
        public string MoTaMatHang { get; set; }   // liệt kê tên mặt hàng
        public int SoMatHang { get; set; }
        public decimal TongTien { get; set; }

        // Danh sách MaHD thuộc đơn này (để cập nhật trạng thái khi xử lý).
        public List<string> DanhSachMaHD { get; set; }
    }
}
