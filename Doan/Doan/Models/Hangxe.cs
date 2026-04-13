using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doan.Models
{
    public class HangXe
    {
        public int Id { get; set; }
        public string TenHang { get; set; }
        public string QuocGia { get; set; }
        public string Logo { get; set; }
        public string MoTa { get; set; }
        public bool TrangThai { get; set; }
    }
}
