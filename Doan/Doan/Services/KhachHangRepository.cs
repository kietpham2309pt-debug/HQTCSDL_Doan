using Doan.Model;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;

namespace Doan.Services
{
    public class KhachHangRepository
    {
        private readonly string _connectionString = @"Data Source=.;Initial Catalog=DL_OTO;Integrated Security=True";

        public ObservableCollection<KhachHang> LayTatCa()
        {
            var danhSach = new ObservableCollection<KhachHang>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(
                "SELECT MaKH, HoTen, GioiTinh, SoDienThoai, NgaySinh, Email FROM KHACHHANG ORDER BY HoTen", conn))
            {
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        danhSach.Add(MapKhachHang(reader));
                    }
                }
            }

            return danhSach;
        }

        public ObservableCollection<KhachHang> TimTheoSoDienThoai(string soDienThoai)
        {
            var danhSach = new ObservableCollection<KhachHang>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_TimKhachHangSDT", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@SoDienThoai", SqlDbType.VarChar, 15).Value = soDienThoai;

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        danhSach.Add(MapKhachHang(reader));
                    }
                }
            }

            return danhSach;
        }

        public void Them(KhachHang khachHang)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_ThemKhachHang", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@HoTen", SqlDbType.NVarChar, 100).Value = khachHang.HoTen.Trim();
                cmd.Parameters.Add("@GioiTinh", SqlDbType.NVarChar, 5).Value = khachHang.GioiTinh.Trim();
                cmd.Parameters.Add("@SoDienThoai", SqlDbType.VarChar, 15).Value = khachHang.SoDienThoai.Trim();
                cmd.Parameters.Add("@NgaySinh", SqlDbType.Date).Value = (object)khachHang.NgaySinh ?? DBNull.Value;
                cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 50).Value =
                    string.IsNullOrWhiteSpace(khachHang.Email) ? (object)DBNull.Value : khachHang.Email.Trim();

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void SuaTheoSoDienThoai(string soDienThoaiCu, KhachHang khachHang)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_SuaKhachHang", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@SoDienThoai", SqlDbType.VarChar, 15).Value = soDienThoaiCu;
                cmd.Parameters.Add("@HoTen", SqlDbType.NVarChar, 100).Value = khachHang.HoTen.Trim();
                cmd.Parameters.Add("@GioiTinh", SqlDbType.NVarChar, 5).Value = khachHang.GioiTinh.Trim();
                cmd.Parameters.Add("@NgaySinh", SqlDbType.Date).Value = (object)khachHang.NgaySinh ?? DBNull.Value;
                cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 50).Value =
                    string.IsNullOrWhiteSpace(khachHang.Email) ? (object)DBNull.Value : khachHang.Email.Trim();

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void XoaTheoMaKhachHang(Guid maKhachHang)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_XoaKhachHangMKH", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@MaKhachHang", SqlDbType.UniqueIdentifier).Value = maKhachHang;

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void XoaTheoSoDienThoai(string soDienThoai)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_XoaKhachHangSDT", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@SoDienThoai", SqlDbType.VarChar, 15).Value = soDienThoai;

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private static KhachHang MapKhachHang(SqlDataReader reader)
        {
            return new KhachHang
            {
                MaKH = reader.GetGuid(reader.GetOrdinal("MaKH")),
                HoTen = reader["HoTen"].ToString(),
                GioiTinh = reader["GioiTinh"].ToString(),
                SoDienThoai = reader["SoDienThoai"].ToString(),
                NgaySinh = reader["NgaySinh"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["NgaySinh"]),
                Email = reader["Email"] == DBNull.Value ? string.Empty : reader["Email"].ToString()
            };
        }
    }
}
