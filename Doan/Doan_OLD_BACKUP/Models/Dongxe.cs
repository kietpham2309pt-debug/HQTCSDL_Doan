using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doan.Models
{
    public class DongXe
    {
        public int Id { get; set; }
        public string TenDong { get; set; }
        public int IdHang { get; set; }
        public int NamSanXuat { get; set; }
        public decimal GiaBanDuKien { get; set; }
        public string MoTa { get; set; }
        public bool TrangThai { get; set; }
    }
}
