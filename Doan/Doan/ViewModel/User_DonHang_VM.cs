using Doan.Helper;
using Doan.Model;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Doan.ViewModel
{
    // Khách vãng lai tra cứu đơn của mình theo số điện thoại.
    public class User_DonHang_VM : BaseViewModel
    {
        private ObservableCollection<HoaDon_HienThi_VM> danhSachDonHang;
        public ObservableCollection<HoaDon_HienThi_VM> DanhSachDonHang
        {
            get { return danhSachDonHang; }
            set { danhSachDonHang = value; OnPropertyChanged(); }
        }

        private string sdtTraCuu;
        public string SDTTraCuu
        {
            get { return sdtTraCuu; }
            set { sdtTraCuu = value; OnPropertyChanged(); }
        }

        private string tenKhachHienThi = "(Nhập số điện thoại để tra cứu đơn)";
        public string TenKhachHienThi
        {
            get { return tenKhachHienThi; }
            set { tenKhachHienThi = value; OnPropertyChanged(); }
        }

        public int TongSoDon
        {
            get { return DanhSachDonHang?.Count ?? 0; }
        }

        public decimal TongTienDaMua
        {
            get { return DanhSachDonHang == null ? 0 : DanhSachDonHang.Sum(h => h.ThanhTien ?? 0); }
        }

        public ICommand LenhTraCuu { get; }
        public ICommand LenhTaiLai { get; }

        public User_DonHang_VM()
        {
            LenhTraCuu = new RelayCommand(_ => TraCuuTheoSDT());
            LenhTaiLai = new RelayCommand(_ => TraCuuTheoSDT());

            DanhSachDonHang = new ObservableCollection<HoaDon_HienThi_VM>();

            // Nếu khách vừa đặt hàng trong phiên này thì tự điền & tra cứu luôn.
            if (PhienDangNhap.KhachHangHienTai != null)
            {
                SDTTraCuu = PhienDangNhap.KhachHangHienTai.SDT;
                TaiTheoMaKH(PhienDangNhap.KhachHangHienTai.MaKH, PhienDangNhap.KhachHangHienTai.HoTen);
            }
        }

        private void TraCuuTheoSDT()
        {
            string sdt = (SDTTraCuu ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(sdt))
            {
                MessageBox.Show("Vui lòng nhập số điện thoại đã dùng khi đặt hàng.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var ctx = new QuanLyBanXeMayEntities())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                var kh = ctx.KhachHangs.FirstOrDefault(k => k.SDT == sdt);
                if (kh == null)
                {
                    DanhSachDonHang = new ObservableCollection<HoaDon_HienThi_VM>();
                    TenKhachHienThi = "(Không tìm thấy khách hàng với SĐT này)";
                    OnPropertyChanged(nameof(TongSoDon));
                    OnPropertyChanged(nameof(TongTienDaMua));
                    return;
                }
                TaiTheoMaKH(kh.MaKH, kh.HoTen);
            }
        }

        private void TaiTheoMaKH(string maKH, string tenKhach)
        {
            TenKhachHienThi = tenKhach;

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
