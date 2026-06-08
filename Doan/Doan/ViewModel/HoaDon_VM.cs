using Doan.Helper;
using Doan.Model;
using Doan.View;
using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Windows.Input;

namespace Doan.ViewModel
{
    // Màn hình LỊCH SỬ GIAO DỊCH — gộp nhiều mặt hàng 1 lần mua thành 1 dòng (theo MaGiaoDich).
    public class HoaDon_VM : BaseViewModel
    {
        private ObservableCollection<GiaoDich_HienThi_VM> danhSachGiaoDich;
        public ObservableCollection<GiaoDich_HienThi_VM> DanhSachGiaoDich
        {
            get { return danhSachGiaoDich; }
            set { danhSachGiaoDich = value; OnPropertyChanged(); }
        }

        private string tuKhoaTimKiem;
        public string TuKhoaTimKiem
        {
            get { return tuKhoaTimKiem; }
            set { tuKhoaTimKiem = value; OnPropertyChanged(); TaiDanhSach(); }
        }

        private int tongSoHoaDon;
        public int TongSoHoaDon { get { return tongSoHoaDon; } set { tongSoHoaDon = value; OnPropertyChanged(); } }

        private decimal tongDoanhThu;
        public decimal TongDoanhThu { get { return tongDoanhThu; } set { tongDoanhThu = value; OnPropertyChanged(); } }

        private int soKhachHang;
        public int SoKhachHang { get { return soKhachHang; } set { soKhachHang = value; OnPropertyChanged(); } }

        private decimal giaTriTrungBinh;
        public decimal GiaTriTrungBinh { get { return giaTriTrungBinh; } set { giaTriTrungBinh = value; OnPropertyChanged(); } }

        private decimal doanhThuTienMat;
        public decimal DoanhThuTienMat { get { return doanhThuTienMat; } set { doanhThuTienMat = value; OnPropertyChanged(); } }

        private decimal doanhThuChuyenKhoan;
        public decimal DoanhThuChuyenKhoan { get { return doanhThuChuyenKhoan; } set { doanhThuChuyenKhoan = value; OnPropertyChanged(); } }

        private decimal doanhThuTraGop;
        public decimal DoanhThuTraGop { get { return doanhThuTraGop; } set { doanhThuTraGop = value; OnPropertyChanged(); } }

        public ICommand LenhTimKiem { get; }
        public ICommand LenhLamMoi { get; }
        public ICommand LenhXemChiTiet { get; }
        public ICommand LenhInGiaoDich { get; }

        public HoaDon_VM()
        {
            LenhTimKiem = new RelayCommand(_ => TaiDanhSach());
            LenhLamMoi = new RelayCommand(_ => { TuKhoaTimKiem = string.Empty; TaiDanhSach(); });
            LenhXemChiTiet = new RelayCommand(p => XemChiTiet(p as GiaoDich_HienThi_VM), p => p is GiaoDich_HienThi_VM);
            LenhInGiaoDich = new RelayCommand(p => HoaDonPrinter.InGiaoDich(p as GiaoDich_HienThi_VM), p => p is GiaoDich_HienThi_VM);

            DanhSachGiaoDich = new ObservableCollection<GiaoDich_HienThi_VM>();
            TaiDanhSach();
        }

        private void TaiDanhSach()
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

                var items = ds.Select(item => new HoaDon_HienThi_VM
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
                    TrangThai = item.TrangThai,
                    MaGiaoDich = string.IsNullOrWhiteSpace(item.MaGiaoDich) ? item.MaHD : item.MaGiaoDich
                }).ToList();

                var groups = items
                    .GroupBy(i => i.MaGiaoDich)
                    .Select(grp => new GiaoDich_HienThi_VM
                    {
                        MaGiaoDich = grp.Key,
                        NgayLap = grp.First().NgayLap,
                        TenNhanVien = grp.First().TenNhanVien,
                        TenKhachHang = grp.First().TenKhachHang,
                        SDT = grp.First().SDT,
                        PhuongThucThanhToan = grp.First().PhuongThucThanhToan,
                        TrangThai = grp.First().TrangThai,
                        SoMatHang = grp.Count(),
                        TongTien = grp.Sum(x => x.ThanhTien ?? 0),
                        DanhSachMatHang = new ObservableCollection<HoaDon_HienThi_VM>(grp.ToList())
                    })
                    .OrderByDescending(g => g.NgayLap)
                    .ThenByDescending(g => g.MaGiaoDich)
                    .ToList();

                if (!string.IsNullOrWhiteSpace(TuKhoaTimKiem))
                {
                    string k = TuKhoaTimKiem.Trim().ToLower();
                    groups = groups.Where(g =>
                        (g.MaGiaoDich ?? string.Empty).ToLower().Contains(k) ||
                        (g.TenKhachHang ?? string.Empty).ToLower().Contains(k) ||
                        (g.SDT ?? string.Empty).ToLower().Contains(k) ||
                        g.DanhSachMatHang.Any(i => (i.TenDV_SP ?? string.Empty).ToLower().Contains(k))).ToList();
                }

                DanhSachGiaoDich = new ObservableCollection<GiaoDich_HienThi_VM>(groups);

                TongSoHoaDon = groups.Count;
                TongDoanhThu = groups.Sum(g => g.TongTien);
                GiaTriTrungBinh = groups.Count > 0 ? TongDoanhThu / groups.Count : 0;
                SoKhachHang = groups.Select(g => g.SDT ?? string.Empty)
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Distinct(StringComparer.OrdinalIgnoreCase).Count();
                DoanhThuTienMat = groups.Where(g => (g.PhuongThucThanhToan ?? "") == "Tiền mặt").Sum(g => g.TongTien);
                DoanhThuChuyenKhoan = groups.Where(g => (g.PhuongThucThanhToan ?? "") == "Chuyển khoản").Sum(g => g.TongTien);
                DoanhThuTraGop = groups.Where(g => (g.PhuongThucThanhToan ?? "") == "Trả góp").Sum(g => g.TongTien);
            }
        }

        private void XemChiTiet(GiaoDich_HienThi_VM gd)
        {
            if (gd == null) return;
            var cuaSo = new W_ChiTietGiaoDich { DataContext = gd };
            cuaSo.ShowDialog();
        }
    }
}
