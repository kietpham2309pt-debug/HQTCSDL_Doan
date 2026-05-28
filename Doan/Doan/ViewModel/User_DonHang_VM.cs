using Doan.Helper;
using Doan.Model;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Windows.Input;

namespace Doan.ViewModel
{
    public class User_DonHang_VM : BaseViewModel
    {
        private ObservableCollection<HoaDon_HienThi_VM> danhSachDonHang;
        public ObservableCollection<HoaDon_HienThi_VM> DanhSachDonHang
        {
            get { return danhSachDonHang; }
            set
            {
                danhSachDonHang = value;
                OnPropertyChanged();
            }
        }

        public string TenKhachHienThi
        {
            get
            {
                if (PhienDangNhap.KhachHangHienTai != null)
                {
                    return PhienDangNhap.KhachHangHienTai.HoTen ?? string.Empty;
                }
                return "(Chưa đăng nhập với hồ sơ khách)";
            }
        }

        public int TongSoDon
        {
            get { return DanhSachDonHang?.Count ?? 0; }
        }

        public decimal TongTienDaMua
        {
            get
            {
                if (DanhSachDonHang == null) return 0;
                return DanhSachDonHang.Sum(h => h.ThanhTien ?? 0);
            }
        }

        public ICommand LenhTaiLai { get; }

        public User_DonHang_VM()
        {
            LenhTaiLai = new RelayCommand(_ => TaiDanhSach());
            TaiDanhSach();
        }

        private void TaiDanhSach()
        {
            if (PhienDangNhap.KhachHangHienTai == null)
            {
                DanhSachDonHang = new ObservableCollection<HoaDon_HienThi_VM>();
                OnPropertyChanged(nameof(TongSoDon));
                OnPropertyChanged(nameof(TongTienDaMua));
                return;
            }

            string maKH = PhienDangNhap.KhachHangHienTai.MaKH;

            using (var ctx = new QuanLyBanXeMayEntities())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                var ds = ctx.HoaDons
                    .Include("NhanVien")
                    .Include("KhachHang")
                    .Where(item => item.MaKH == maKH)
                    .OrderByDescending(item => item.NgayLap)
                    .ThenByDescending(item => item.MaHD)
                    .ToList();

                var ket = ds.Select(item => new HoaDon_HienThi_VM
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

                DanhSachDonHang = new ObservableCollection<HoaDon_HienThi_VM>(ket);
            }

            OnPropertyChanged(nameof(TongSoDon));
            OnPropertyChanged(nameof(TongTienDaMua));
        }
    }
}
