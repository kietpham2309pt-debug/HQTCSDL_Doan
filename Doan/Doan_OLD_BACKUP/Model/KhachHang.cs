using System;

namespace Doan.Model
{
    public class KhachHang
    {
        public Guid MaKH { get; set; }
        public string HoTen { get; set; }
        public string GioiTinh { get; set; }
        public string SoDienThoai { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string Email { get; set; }
    }
}
