using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doan.Models
{
    public class Xe
    {
        public int Id { get; set; }
        public int IdDongXe { get; set; }
        public string TenDong { get; set; }
        public string MauSac { get; set; }
        public int NamSanXuat { get; set; }
        public decimal GiaBan { get; set; }
        public int SoLuong { get; set; }
        public string HinhAnh { get; set; }
        public string MoTa { get; set; }
        public bool TrangThai { get; set; }
    }
}
