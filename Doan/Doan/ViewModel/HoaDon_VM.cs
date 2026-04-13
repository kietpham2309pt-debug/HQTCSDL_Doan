using Doan.Helper;
using Doan.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace Doan.ViewModel
{
    public class HoaDon_VM : BaseViewModel
    {
        string connectionString = @"Data Source=LAPTOP-80MIEMQ9\SQLEXPRESS;Initial Catalog=DL_OTO;Integrated Security=True";
        public ObservableCollection<HoaDonModel> DanhSachHoaDonHienThi { get; set; }
        public KhachHang KhachHangDuocChon { get; set; }
        public ObservableCollection<Car> GioHangHienTai { get; set; }
        public string HinhThucThanhToanDangChon { get; set; }


        private string _tenNhanVienLap;
        public string TenNhanVienLap
        {
            get => _tenNhanVienLap;
            set { _tenNhanVienLap = value; OnPropertyChanged(); }
        }

        private DateTime _ngayLapNhap = DateTime.Now;
        public DateTime NgayLapNhap
        {
            get => _ngayLapNhap;
            set { _ngayLapNhap = value; OnPropertyChanged(); }
        }

        private string _sdtKhachNhap;
        public string SDTKhachNhap
        {
            get => _sdtKhachNhap;
            set { _sdtKhachNhap = value; OnPropertyChanged(); }
        }

        private decimal _tongTienHang;
        public decimal TongTienHang
        {
            get => _tongTienHang;
            set { _tongTienHang = value; OnPropertyChanged(); }
        }

        private decimal _thanhTienThanhToan;
        public decimal ThanhTienThanhToan
        {
            get => _thanhTienThanhToan;
            set { _thanhTienThanhToan = value; OnPropertyChanged(); }
        }

        // Command
        public ICommand LenhKiemTraSDTKhach { get; set; }
        public ICommand LenhHuyHoaDon { get; set; }
        public ICommand LenhXacNhanThanhToan { get; set; }

        public HoaDon_VM()
        {
            DanhSachHoaDonHienThi = new ObservableCollection<HoaDonModel>();

            // Thêm dữ liệu mẫu để test
            DanhSachHoaDonHienThi.Add(new HoaDonModel
            {
                MaHD = Guid.NewGuid(),
                NgayLap = DateTime.Now,
                TenNhanVien = "Nguyễn Văn A",
                TenKhachHang = "Trần Thị B",
                SDT = "0901234567",
                TenDV_SP = "Xe Toyota Vios",
                SoLuong = 1,
                ThanhTien = 500000000,
                PhuongThucThanhToan = "Tiền mặt"
            });

            // Khởi tạo command
            LenhKiemTraSDTKhach = new RelayCommand(o => KiemTraSDT());
            LenhHuyHoaDon = new RelayCommand(o => HuyHoaDon());
            LenhXacNhanThanhToan = new RelayCommand(o => XacNhanThanhToan());
        }

        private void KiemTraSDT()
        {
            // Logic kiểm tra khách hàng theo SDT
        }

        private void HuyHoaDon()
        {
            // Logic hủy hóa đơn
        }
        public Guid ThemDonHang(string tenNV, Guid maKH, DateTime ngayLap, decimal tongTien)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"INSERT INTO DonHang (TenDH, MaKhachHang, MaNV, NgayLap, TongTien)
                         OUTPUT INSERTED.MaDH
                         VALUES (@TenDH, @MaKH, @MaNV, @NgayLap, @TongTien)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@TenDH", "Hóa đơn mới");
                cmd.Parameters.AddWithValue("@MaKH", maKH);
                cmd.Parameters.AddWithValue("@MaNV", 1); // giả sử nhân viên ID=1
                cmd.Parameters.AddWithValue("@NgayLap", ngayLap);
                cmd.Parameters.AddWithValue("@TongTien", tongTien);

                conn.Open();
                return (Guid)cmd.ExecuteScalar();
            }
        }

        public void ThemChiTietDonHang(Guid maDH, int maXe, DateTime ngayLap, int soLuong)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"INSERT INTO ChiTietDH (MaDH, MaXe, NgayLap, SoLuong)
                         VALUES (@MaDH, @MaXe, @NgayLap, @SoLuong)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaDH", maDH);
                cmd.Parameters.AddWithValue("@MaXe", maXe);
                cmd.Parameters.AddWithValue("@NgayLap", ngayLap);
                cmd.Parameters.AddWithValue("@SoLuong", soLuong);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private void XacNhanThanhToan()
        { 
        }

    }
}

