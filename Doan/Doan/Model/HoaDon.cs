using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doan.Model
{
    public class HoaDonModel
    {
        public Guid MaHD { get; set; }
        public DateTime NgayLap { get; set; }
        public string TenNhanVien { get; set; }
        public string TenKhachHang { get; set; }
        public string SDT { get; set; }
        public string TenDV_SP { get; set; }
        public int SoLuong { get; set; }
        public decimal ThanhTien { get; set; }
        public string PhuongThucThanhToan { get; set; }
    }

}
