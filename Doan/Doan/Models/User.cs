using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doan
{
    public class User
    {
        public int Id { get; set; }

        public string TenDangNhap { get; set; }

        public string MatKhau { get; set; }

        public string HoTen { get; set; }

        public string VaiTro { get; set; }

        public string Email { get; set; }

        public string SoDienThoai { get; set; }

        public DateTime NgayTao { get; set; }

        public DateTime NgayCapNhat { get; set; }

        public bool TrangThai { get; set; }
    }
}
