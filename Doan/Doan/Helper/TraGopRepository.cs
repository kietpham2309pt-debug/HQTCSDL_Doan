using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Doan.Model;

namespace Doan.Helper
{
    public class TheoDoiTraGop_DTO
    {
        public string MaTraGop { get; set; }
        public string MaGiaoDich { get; set; }
        public string MaKH { get; set; }
        public string TenKhach { get; set; }
        public string SDT { get; set; }
        public decimal? TongTien { get; set; }
        public decimal? TraTruoc { get; set; }
        public int? SoKy { get; set; }
        public decimal? DaTra { get; set; }
        public decimal? ConLai { get; set; }
        public decimal? SoTienMoiKy { get; set; }
        public DateTime? NgayLap { get; set; }
        public string TrangThai { get; set; }
    }

    public class ChiTietTraGop_DTO
    {
        public int ID { get; set; }
        public DateTime? NgayTra { get; set; }
        public decimal? SoTien { get; set; }
        public string GhiChu { get; set; }
    }

    // Truy cập bảng trả góp (PhieuTraGop/ChiTietTraGop) qua stored procedure + view.
    public static class TraGopRepository
    {
        // Tạo phiếu trả góp cho 1 giao dịch. Trả về MaTraGop mới.
        public static string TaoPhieuTraGop(string maGiaoDich, string maKH, decimal tongTien, decimal traTruoc, int soKy)
        {
            var pMaPN = new SqlParameter("@MaTraGop", SqlDbType.VarChar, 20) { Direction = ParameterDirection.Output };
            using (var ctx = new QuanLyBanXeMayEntities())
            {
                ctx.Database.ExecuteSqlCommand(
                    "EXEC sp_TaoPhieuTraGop @MaGiaoDich, @MaKH, @TongTien, @TraTruoc, @SoKy, @MaTraGop OUTPUT",
                    new SqlParameter("@MaGiaoDich", (object)maGiaoDich ?? DBNull.Value),
                    new SqlParameter("@MaKH", (object)maKH ?? DBNull.Value),
                    new SqlParameter("@TongTien", tongTien),
                    new SqlParameter("@TraTruoc", traTruoc),
                    new SqlParameter("@SoKy", soKy),
                    pMaPN);
            }
            return pMaPN.Value as string;
        }

        public static List<TheoDoiTraGop_DTO> LayTheoDoi()
        {
            using (var ctx = new QuanLyBanXeMayEntities())
            {
                return ctx.Database.SqlQuery<TheoDoiTraGop_DTO>(
                    "SELECT MaTraGop, MaGiaoDich, MaKH, TenKhach, SDT, TongTien, TraTruoc, SoKy, DaTra, ConLai, SoTienMoiKy, NgayLap, TrangThai " +
                    "FROM vw_TheoDoiTraGop ORDER BY NgayLap DESC").ToList();
            }
        }

        public static void GhiNhan(string maTraGop, decimal soTien, string ghiChu)
        {
            using (var ctx = new QuanLyBanXeMayEntities())
            {
                ctx.Database.ExecuteSqlCommand(
                    "EXEC sp_GhiNhanTraGop @MaTraGop, @SoTien, @GhiChu",
                    new SqlParameter("@MaTraGop", (object)maTraGop ?? DBNull.Value),
                    new SqlParameter("@SoTien", soTien),
                    new SqlParameter("@GhiChu", (object)ghiChu ?? DBNull.Value));
            }
        }

        public static List<ChiTietTraGop_DTO> LayChiTiet(string maTraGop)
        {
            using (var ctx = new QuanLyBanXeMayEntities())
            {
                return ctx.Database.SqlQuery<ChiTietTraGop_DTO>(
                    "SELECT ID, NgayTra, SoTien, GhiChu FROM ChiTietTraGop WHERE MaTraGop = @p0 ORDER BY NgayTra",
                    maTraGop).ToList();
            }
        }
    }
}
