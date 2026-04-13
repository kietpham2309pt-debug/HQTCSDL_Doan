using System.Data.SqlClient;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Configuration;

namespace Doan.Model
{
    public static class DuLieuHeThong
    {
        private static readonly string connectionString = GetConnectionString();

        private static string GetConnectionString()
        {
            return @"Data Source=.;Initial Catalog=DL_OTO;Integrated Security=True";
        }
        public static ObservableCollection<HangXe> LayDanhSachHangXe()
        {
            var ds = new ObservableCollection<HangXe>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM HangXe";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ds.Add(new HangXe
                    {
                        TenHang = reader["TenHang"].ToString(),
                        QuocGia = reader["QuocGia"].ToString(),
                        LogoFullPath = reader["LogoFullPath"].ToString()
                    });
                }
            }
            return ds;
        }

        public static ObservableCollection<Car> LayDanhSachXe()
        {
            var ds = new ObservableCollection<Car>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Car";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ds.Add(new Car
                    {
                        TenHang = reader["TenHang"].ToString(),
                        TenDongXe = reader["TenDongXe"].ToString(),
                        LoaiXe = reader["LoaiXe"].ToString(),
                        MauSac = reader["MauSac"].ToString(),
                        NamSX = Convert.ToInt32(reader["NamSX"]),
                        GiaXe = Convert.ToInt32(reader["GiaXe"]),
                        HinhAnhFullPath = reader["HinhAnhFullPath"].ToString(),
                        MoTa = reader["MoTa"].ToString(),
                        SoLuongTon = Convert.ToInt32(reader["SoLuongTon"])
                    });
                }
            }
            return ds;
        }

        // Cập nhật hàm này để khớp với lời gọi từ ViewModel
        public static void LuuXeSQL(string tenHang, string tenDongXe, string loaiXe, string mauSac, int namSX, int giaXe, string hinhAnh, string moTa, int soLuong, bool laThemMoi, string tenCu = null)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query;
                if (laThemMoi)
                {
                    query = "INSERT INTO Car (TenHang, TenDongXe, LoaiXe, MauSac, NamSX, GiaXe, HinhAnhFullPath, MoTa, SoLuongTon) " +
                            "VALUES (@TenHang, @TenDongXe, @LoaiXe, @MauSac, @NamSX, @GiaXe, @HinhAnh, @MoTa, @SL)";
                }
                else
                {
                    // Update dựa trên tên cũ để tránh lỗi khi người dùng đổi luôn cả tên dòng xe
                    query = "UPDATE Car SET TenDongXe=@TenDongXe, LoaiXe=@LoaiXe, MauSac=@MauSac, NamSX=@NamSX, GiaXe=@GiaXe, " +
                            "HinhAnhFullPath=@HinhAnh, MoTa=@MoTa, SoLuongTon=@SL " +
                            "WHERE TenHang=@TenHang AND TenDongXe=@TenCu";
                }

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@TenHang", tenHang);
                cmd.Parameters.AddWithValue("@TenDongXe", tenDongXe);
                cmd.Parameters.AddWithValue("@LoaiXe", loaiXe);
                cmd.Parameters.AddWithValue("@MauSac", mauSac);
                cmd.Parameters.AddWithValue("@NamSX", namSX);
                cmd.Parameters.AddWithValue("@GiaXe", giaXe);
                cmd.Parameters.AddWithValue("@HinhAnh", hinhAnh);
                cmd.Parameters.AddWithValue("@MoTa", moTa);
                cmd.Parameters.AddWithValue("@SL", soLuong);
                if (!laThemMoi) cmd.Parameters.AddWithValue("@TenCu", tenCu ?? tenDongXe);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public static void XoaXeSQL(string tenHang, string tenDongXe)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM Car WHERE TenHang = @h AND TenDongXe = @d";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@h", tenHang);
                cmd.Parameters.AddWithValue("@d", tenDongXe);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public static void LuuHangXeSQL(string tenHang, string quocGia, string logo, bool laThemMoi)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = laThemMoi
                    ? "INSERT INTO HangXe (TenHang, QuocGia, LogoFullPath) VALUES (@ten, @qg, @logo)"
                    : "UPDATE HangXe SET QuocGia = @qg, LogoFullPath = @logo WHERE TenHang = @ten";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ten", tenHang);
                cmd.Parameters.AddWithValue("@qg", quocGia);
                cmd.Parameters.AddWithValue("@logo", logo);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public static void XoaHangXeSQL(string tenHang)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM HangXe WHERE TenHang = @ten";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ten", tenHang);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public static bool KiemTraDangNhap(string user, string pass)
        {
                try
            {
                using (var conn = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("SELECT COUNT(*) FROM dbo.TaiKhoan WHERE Username = @u AND Password = @p", conn))
                {
                    cmd.Parameters.AddWithValue("@u", user);
                    cmd.Parameters.AddWithValue("@p", pass);
                    conn.Open();
                    var result = cmd.ExecuteScalar();
                    return result != null && Convert.ToInt32(result) > 0;
                }
            }
            catch (SqlException ex)
            {
                // log ex.Number and ex.Message to a file/telemetry
                return false;
            }
        }

        public static void TaoTaiKhoan(string username, string password)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO TaiKhoan (Username, Password) VALUES (@u, @p)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@u", username);
                cmd.Parameters.AddWithValue("@p", password);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public static void TestSQLConnection()
        {
            using (var conn = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=DL_OTO;Integrated Security=True"))
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT DB_NAME();", conn))
                    Console.WriteLine("Connected DB: " + (string)cmd.ExecuteScalar());
                using (var cmd = new SqlCommand("SELECT COUNT(*) FROM sys.objects WHERE type='U' AND name=@t;", conn))
                {
                    cmd.Parameters.AddWithValue("@t", "TaiKhoan");
                    Console.WriteLine("TaiKhoan exists: " + ((int)cmd.ExecuteScalar() > 0));
                }
            }
        }
    }
}