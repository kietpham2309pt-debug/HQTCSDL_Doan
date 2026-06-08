using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Doan.Model;

namespace Doan.Helper
{
    // Một dòng trong phiếu nhập (giỏ nhập kho).
    public class DongNhapKho
    {
        public string LoaiMatHang { get; set; } // "Xe" | "PhuTung"
        public string MaXe { get; set; }
        public string MaPT { get; set; }
        public string MaMatHang { get { return LoaiMatHang == "Xe" ? MaXe : MaPT; } }
        public string TenMatHang { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGiaNhap { get; set; }
        public decimal ThanhTien { get { return SoLuong * DonGiaNhap; } }
    }

    // Một dòng lịch sử nhập (đọc từ view vw_LichSuNhap). Tên thuộc tính khớp tên cột view.
    public class DongLichSuNhap
    {
        public string MaPN { get; set; }
        public DateTime? NgayNhap { get; set; }
        public string NhaCungCap { get; set; }
        public string GhiChu { get; set; }
        public decimal? TongTien { get; set; }
        public string MaNV { get; set; }
        public string TenNhanVien { get; set; }
        public int? ChiTietID { get; set; }
        public string LoaiMatHang { get; set; }
        public string MaMatHang { get; set; }
        public string TenMatHang { get; set; }
        public int? SoLuong { get; set; }
        public decimal? DonGiaNhap { get; set; }
        public decimal? ThanhTien { get; set; }
    }

    // Truy cập bảng nhập kho (PhieuNhap/ChiTietPhieuNhap) qua stored procedure + view,
    // KHÔNG đưa vào EDMX để tránh phẫu thuật model.
    public static class NhapKhoRepository
    {
        // Tạo phiếu nhập (gọi sp_NhapKho với TVP). Trả về mã phiếu nhập mới (PNxxx).
        public static string ThucHienNhapKho(string maNV, string nhaCungCap, string ghiChu, IEnumerable<DongNhapKho> cacDong)
        {
            DataTable bang = TaoBangChiTiet(cacDong);
            if (bang.Rows.Count == 0)
            {
                throw new InvalidOperationException("Phiếu nhập phải có ít nhất 1 dòng.");
            }

            var pMaNV = new SqlParameter("@MaNV", SqlDbType.VarChar, 20) { Value = (object)maNV ?? DBNull.Value };
            var pNcc = new SqlParameter("@NhaCungCap", SqlDbType.NVarChar, 150) { Value = (object)nhaCungCap ?? DBNull.Value };
            var pGhiChu = new SqlParameter("@GhiChu", SqlDbType.NVarChar, 255) { Value = (object)ghiChu ?? DBNull.Value };
            var pChiTiet = new SqlParameter("@ChiTiet", SqlDbType.Structured)
            {
                TypeName = "dbo.ChiTietNhapType",
                Value = bang
            };
            var pMaPN = new SqlParameter("@MaPN", SqlDbType.VarChar, 20) { Direction = ParameterDirection.Output };

            using (var ctx = new QuanLyBanXeMayEntities())
            {
                ctx.Database.ExecuteSqlCommand(
                    "EXEC sp_NhapKho @MaNV, @NhaCungCap, @GhiChu, @ChiTiet, @MaPN OUTPUT",
                    pMaNV, pNcc, pGhiChu, pChiTiet, pMaPN);
            }

            return pMaPN.Value as string;
        }

        private static DataTable TaoBangChiTiet(IEnumerable<DongNhapKho> cacDong)
        {
            var bang = new DataTable();
            bang.Columns.Add("LoaiMatHang", typeof(string));
            bang.Columns.Add("MaXe", typeof(string));
            bang.Columns.Add("MaPT", typeof(string));
            bang.Columns.Add("SoLuong", typeof(int));
            bang.Columns.Add("DonGiaNhap", typeof(decimal));

            if (cacDong != null)
            {
                foreach (var d in cacDong)
                {
                    if (d == null || d.SoLuong <= 0) continue;
                    bang.Rows.Add(
                        d.LoaiMatHang,
                        d.LoaiMatHang == "Xe" ? (object)d.MaXe : DBNull.Value,
                        d.LoaiMatHang == "PhuTung" ? (object)d.MaPT : DBNull.Value,
                        d.SoLuong,
                        d.DonGiaNhap);
                }
            }
            return bang;
        }

        // Đọc lịch sử nhập (mỗi dòng = 1 chi tiết) từ view vw_LichSuNhap.
        public static List<DongLichSuNhap> LayLichSuNhap()
        {
            using (var ctx = new QuanLyBanXeMayEntities())
            {
                return ctx.Database.SqlQuery<DongLichSuNhap>(
                    "SELECT MaPN, NgayNhap, NhaCungCap, GhiChu, TongTien, MaNV, TenNhanVien, " +
                    "ChiTietID, LoaiMatHang, MaMatHang, TenMatHang, SoLuong, DonGiaNhap, ThanhTien " +
                    "FROM vw_LichSuNhap ORDER BY NgayNhap DESC, MaPN DESC, ChiTietID").ToList();
            }
        }
    }
}
