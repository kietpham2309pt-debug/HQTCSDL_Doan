using Doan.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doan.Services
{
    public class DatabaseService
    {
        private string _connectionString =@"Server=DESKTOP-LLD1MQC;Database=CarManagementDB;Trusted_Connection=True;";

        // Constructor để cho phép truyền connection string
        public DatabaseService(string connectionString = null)
        {
            if (!string.IsNullOrEmpty(connectionString))
            {
                _connectionString = connectionString;
            }
        }

        /// <summary>
        /// Xác thực người dùng từ database
        /// </summary>
        public ObservableCollection<HangXe> GetAllHangXe()
        {
            ObservableCollection<HangXe> hangXeList = new ObservableCollection<HangXe>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string query = "SELECT Id, TenHang, QuocGia, Logo, MoTa, TrangThai FROM HangXe WHERE TrangThai = 1 ORDER BY TenHang";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                HangXe hangXe = new HangXe
                                {
                                    Id = (int)reader["Id"],
                                    TenHang = reader["TenHang"].ToString(),
                                    QuocGia = reader["QuocGia"].ToString(),
                                    Logo = reader["Logo"].ToString(),
                                    MoTa = reader["MoTa"] != DBNull.Value ? reader["MoTa"].ToString() : "",
                                    TrangThai = (bool)reader["TrangThai"]
                                };

                                hangXeList.Add(hangXe);
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception($"Lỗi kết nối database: {ex.Message}");
            }

            return hangXeList;
        }

        /// <summary>
        /// Lấy danh sách xe theo hãng sử dụng stored procedure
        /// </summary>
        public ObservableCollection<Xe> GetXeByHang(int idHang)
        {
            ObservableCollection<Xe> xeList = new ObservableCollection<Xe>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("sp_GetXeByHang", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@IdHang", idHang);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Xe xe = new Xe
                                {
                                    Id = (int)reader["Id"],
                                    IdDongXe = (int)reader["IdDongXe"],
                                    TenDong = reader["TenDong"].ToString(),
                                    MauSac = reader["MauSac"].ToString(),
                                    NamSanXuat = (int)reader["NamSanXuat"],
                                    GiaBan = (decimal)reader["GiaBan"],
                                    SoLuong = (int)reader["SoLuong"],
                                    HinhAnh = reader["HinhAnh"] != DBNull.Value ? reader["HinhAnh"].ToString() : "",
                                    MoTa = reader["MoTa"] != DBNull.Value ? reader["MoTa"].ToString() : "",
                                    TrangThai = true
                                };

                                xeList.Add(xe);
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception($"Lỗi kết nối database: {ex.Message}");
            }

            return xeList;
        }

        /// <summary>
        /// Lấy xe theo ID
        /// </summary>
        public Xe GetXeById(int idXe)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string query = @"
                        SELECT 
                            x.Id, x.IdDongXe, dx.TenDong, x.MauSac, x.NamSanXuat, 
                            x.GiaBan, x.SoLuong, x.HinhAnh, x.MoTa
                        FROM Xe x
                        INNER JOIN DongXe dx ON x.IdDongXe = dx.Id
                        WHERE x.Id = @IdXe AND x.TrangThai = 1";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@IdXe", idXe);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Xe
                                {
                                    Id = (int)reader["Id"],
                                    IdDongXe = (int)reader["IdDongXe"],
                                    TenDong = reader["TenDong"].ToString(),
                                    MauSac = reader["MauSac"].ToString(),
                                    NamSanXuat = (int)reader["NamSanXuat"],
                                    GiaBan = (decimal)reader["GiaBan"],
                                    SoLuong = (int)reader["SoLuong"],
                                    HinhAnh = reader["HinhAnh"] != DBNull.Value ? reader["HinhAnh"].ToString() : "",
                                    MoTa = reader["MoTa"] != DBNull.Value ? reader["MoTa"].ToString() : "",
                                    TrangThai = true
                                };
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception($"Lỗi kết nối database: {ex.Message}");
            }

            return null;
        }

        /// <summary>
        /// Lấy tất cả màu sắc của một dòng xe
        /// </summary>
        public ObservableCollection<string> GetMauSacByDongXe(int idDongXe)
        {
            ObservableCollection<string> mauList = new ObservableCollection<string>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string query = "SELECT DISTINCT MauSac FROM Xe WHERE IdDongXe = @IdDongXe AND TrangThai = 1 ORDER BY MauSac";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@IdDongXe", idDongXe);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                mauList.Add(reader["MauSac"].ToString());
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception($"Lỗi kết nối database: {ex.Message}");
            }

            return mauList;
        }

        /// <summary>
        /// Test kết nối database
        /// </summary>
        public bool TestConnection()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        public User AuthenticateUser(string username, string password)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"SELECT * FROM TaiKhoan
                         WHERE TenDangNhap = @username
                         AND MatKhau = @password
                         AND TrangThai = 1";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@password", password);

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return new User
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        TenDangNhap = reader["TenDangNhap"].ToString(),
                        MatKhau = reader["MatKhau"].ToString(),
                        HoTen = reader["HoTen"] == DBNull.Value ? "" : reader["HoTen"].ToString(),
                        Email = reader["Email"] == DBNull.Value ? "" : reader["Email"].ToString(),
                        SoDienThoai = reader["SoDienThoai"] == DBNull.Value ? "" : reader["SoDienThoai"].ToString(),
                        NgayTao = reader["NgayTao"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["NgayTao"]),
                        NgayCapNhat = reader["NgayCapNhat"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["NgayCapNhat"]),
                        TrangThai = Convert.ToBoolean(reader["TrangThai"])
                    };
                }
            }

            return null;
        }
    }
}
