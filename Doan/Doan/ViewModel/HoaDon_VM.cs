using Doan.Helper;
using Doan.Model;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Windows.Input;

namespace Doan.ViewModel
{
    // Màn hình LỊCH SỬ GIAO DỊCH (đã tách khỏi màn thanh toán).
    public class HoaDon_VM : BaseViewModel
    {
        private ObservableCollection<HoaDon_HienThi_VM> danhSachHoaDonHienThi;
        public ObservableCollection<HoaDon_HienThi_VM> DanhSachHoaDonHienThi
        {
            get { return danhSachHoaDonHienThi; }
            set
            {
                danhSachHoaDonHienThi = value;
                OnPropertyChanged();
            }
        }

        private string tuKhoaTimKiem;
        public string TuKhoaTimKiem
        {
            get { return tuKhoaTimKiem; }
            set
            {
                tuKhoaTimKiem = value;
                OnPropertyChanged();
            }
        }

        private int tongSoHoaDon;
        public int TongSoHoaDon
        {
            get { return tongSoHoaDon; }
            set { tongSoHoaDon = value; OnPropertyChanged(); }
        }

        private decimal tongDoanhThu;
        public decimal TongDoanhThu
        {
            get { return tongDoanhThu; }
            set { tongDoanhThu = value; OnPropertyChanged(); }
        }

        public ICommand LenhTimKiem { get; }
        public ICommand LenhLamMoi { get; }

        public HoaDon_VM()
        {
            LenhTimKiem = new RelayCommand(_ => TaiDanhSachHoaDonHienThi());
            LenhLamMoi = new RelayCommand(_ => { TuKhoaTimKiem = string.Empty; TaiDanhSachHoaDonHienThi(); });

            DanhSachHoaDonHienThi = new ObservableCollection<HoaDon_HienThi_VM>();
            TaiDanhSachHoaDonHienThi();
        }

        private void TaiDanhSachHoaDonHienThi()
        {
            using (var ctx = new QuanLyBanXeMayEntities())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                var ds = ctx.HoaDons
                    .Include("NhanVien")
                    .Include("KhachHang")
                    .OrderByDescending(item => item.NgayLap)
                    .ThenByDescending(item => item.MaHD)
                    .ToList();

                var danhSachHienThi = ds.Select(item => new HoaDon_HienThi_VM
                {
                    MaHD = item.MaHD,
                    NgayLap = item.NgayLap,
                    TenNhanVien = item.NhanVien != null ? item.NhanVien.HoTen : "(Khách tự đặt)",
                    TenKhachHang = item.KhachHang != null ? item.KhachHang.HoTen : string.Empty,
                    SDT = item.KhachHang != null ? item.KhachHang.SDT : string.Empty,
                    TenDV_SP = item.TenDV_SP,
                    SoLuong = item.SoLuong,
                    ThanhTien = item.ThanhTien,
                    PhuongThucThanhToan = item.PhuongThucThanhToan,
                    TrangThai = item.TrangThai
                }).ToList();

                if (!string.IsNullOrWhiteSpace(TuKhoaTimKiem))
                {
                    string tuKhoa = TuKhoaTimKiem.Trim().ToLower();
                    danhSachHienThi = danhSachHienThi.Where(item =>
                        (item.MaHD ?? string.Empty).ToLower().Contains(tuKhoa) ||
                        (item.TenKhachHang ?? string.Empty).ToLower().Contains(tuKhoa) ||
                        (item.SDT ?? string.Empty).ToLower().Contains(tuKhoa) ||
                        (item.TenDV_SP ?? string.Empty).ToLower().Contains(tuKhoa)).ToList();
                }

                DanhSachHoaDonHienThi = new ObservableCollection<HoaDon_HienThi_VM>(danhSachHienThi);
                TongSoHoaDon = danhSachHienThi.Count;
                TongDoanhThu = danhSachHienThi.Sum(item => item.ThanhTien ?? 0);
            }
        }
    }
}
