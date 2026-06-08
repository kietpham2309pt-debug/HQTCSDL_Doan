using Doan.Helper;
using Doan.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Doan.ViewModel
{
    public class ThongKe_VM : BaseViewModel
    {
        private decimal tongDoanhThu;
        public decimal TongDoanhThu
        {
            get { return tongDoanhThu; }
            set
            {
                tongDoanhThu = value;
                OnPropertyChanged();
            }
        }

        private int soHoaDonMoi;
        public int SoHoaDonMoi
        {
            get { return soHoaDonMoi; }
            set
            {
                soHoaDonMoi = value;
                OnPropertyChanged();
            }
        }

        private int soKhachHangPhucVu;
        public int SoKhachHangPhucVu
        {
            get { return soKhachHangPhucVu; }
            set
            {
                soKhachHangPhucVu = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<CotDoanhThuThang_VM> duLieuDoanhThu6Thang;
        public ObservableCollection<CotDoanhThuThang_VM> DuLieuDoanhThu6Thang
        {
            get { return duLieuDoanhThu6Thang; }
            set
            {
                duLieuDoanhThu6Thang = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<MucThongKeTop_VM> danhSachDichVuBanChay;
        public ObservableCollection<MucThongKeTop_VM> DanhSachDichVuBanChay
        {
            get { return danhSachDichVuBanChay; }
            set
            {
                danhSachDichVuBanChay = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<MucThongKeTop_VM> danhSachXeBanChay;
        public ObservableCollection<MucThongKeTop_VM> DanhSachXeBanChay
        {
            get { return danhSachXeBanChay; }
            set
            {
                danhSachXeBanChay = value;
                OnPropertyChanged();
            }
        }

        // Thống kê chi tiết theo mục (đọc từ các VIEW vw_ThongKeXe/DichVu/PhuTung).
        private ObservableCollection<ThongKeXe_DTO> thongKeTheoHang;
        public ObservableCollection<ThongKeXe_DTO> ThongKeTheoHang
        {
            get { return thongKeTheoHang; }
            set { thongKeTheoHang = value; OnPropertyChanged(); }
        }

        private ObservableCollection<ThongKeDichVu_DTO> thongKeDichVu;
        public ObservableCollection<ThongKeDichVu_DTO> ThongKeDichVu
        {
            get { return thongKeDichVu; }
            set { thongKeDichVu = value; OnPropertyChanged(); }
        }

        private ObservableCollection<ThongKePhuTung_DTO> thongKePhuTung;
        public ObservableCollection<ThongKePhuTung_DTO> ThongKePhuTung
        {
            get { return thongKePhuTung; }
            set { thongKePhuTung = value; OnPropertyChanged(); }
        }

        private long tongGiaTriTonXe;
        public long TongGiaTriTonXe
        {
            get { return tongGiaTriTonXe; }
            set { tongGiaTriTonXe = value; OnPropertyChanged(); }
        }

        // ===== Bộ lọc năm + hãng xe =====
        public ObservableCollection<string> DanhSachNamLoc { get; } = new ObservableCollection<string>();
        private string namLoc = "Tất cả";
        public string NamLoc
        {
            get { return namLoc; }
            set { namLoc = value; OnPropertyChanged(); TaiThongKe(); }
        }

        public ObservableCollection<string> DanhSachHangLoc { get; } = new ObservableCollection<string>();
        private string hangLoc = "Tất cả";
        public string HangLoc
        {
            get { return hangLoc; }
            set { hangLoc = value; OnPropertyChanged(); TaiThongKe(); }
        }

        public ICommand LenhTaiLaiThongKe { get; }

        public ThongKe_VM()
        {
            LenhTaiLaiThongKe = new RelayCommand(_ => TaiThongKe());
            TaiThongKe();
        }

        private void TaiThongKe()
        {
            List<HoaDon> tatCaHoaDon;
            try
            {
                using (var ctx = new QuanLyBanXeMayEntities())
                {
                    tatCaHoaDon = ctx.HoaDons.ToList();
                }
            }
            catch (Exception)
            {
                tatCaHoaDon = new List<HoaDon>();
            }

            DungBoLoc(tatCaHoaDon);
            List<HoaDon> danhSachHoaDon = LocHoaDon(tatCaHoaDon);

            TongDoanhThu = danhSachHoaDon.Sum(item => item.ThanhTien ?? 0m);
            SoHoaDonMoi = danhSachHoaDon
                .Select(item => item.MaHD ?? string.Empty)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Count();

            SoKhachHangPhucVu = danhSachHoaDon
                .Select(item => item.MaKH ?? string.Empty)
                .Where(item => !string.IsNullOrWhiteSpace(item))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Count();

            TaiDuLieuDoanhThu6Thang(danhSachHoaDon);
            TaiDanhSachTop(danhSachHoaDon);
            TaiThongKeChiTietTheoMuc();
        }

        // Thống kê chi tiết từng mục: xe theo hãng, dịch vụ, phụ tùng.
        private void TaiThongKeChiTietTheoMuc()
        {
            try
            {
                using (var ctx = new QuanLyBanXeMayEntities())
                {
                    var xe = ctx.Database.SqlQuery<ThongKeXe_DTO>(
                        "SELECT MaHang, TenHang, SoMauXe, TongTon, GiaTriTon, SoXeDangBan, SoXeAn FROM vw_ThongKeXe ORDER BY GiaTriTon DESC").ToList();
                    if (HangLoc != "Tất cả")
                    {
                        xe = xe.Where(x => string.Equals(x.TenHang, HangLoc, StringComparison.OrdinalIgnoreCase)).ToList();
                    }
                    ThongKeTheoHang = new ObservableCollection<ThongKeXe_DTO>(xe);
                    TongGiaTriTonXe = (long)xe.Sum(x => x.GiaTriTon ?? 0);

                    var dv = ctx.Database.SqlQuery<ThongKeDichVu_DTO>(
                        "SELECT MaPT, Ten, Gia, SoLuongBan, DoanhThu FROM vw_ThongKeDichVu ORDER BY DoanhThu DESC").ToList();
                    ThongKeDichVu = new ObservableCollection<ThongKeDichVu_DTO>(dv);

                    var pt = ctx.Database.SqlQuery<ThongKePhuTung_DTO>(
                        "SELECT MaPT, Ten, Gia, TonKho, SoLuongBan, DoanhThu FROM vw_ThongKePhuTung ORDER BY DoanhThu DESC").ToList();
                    ThongKePhuTung = new ObservableCollection<ThongKePhuTung_DTO>(pt);
                }
            }
            catch (Exception)
            {
                ThongKeTheoHang = new ObservableCollection<ThongKeXe_DTO>();
                ThongKeDichVu = new ObservableCollection<ThongKeDichVu_DTO>();
                ThongKePhuTung = new ObservableCollection<ThongKePhuTung_DTO>();
            }
        }

        // Dựng danh sách năm + hãng cho bộ lọc (chỉ 1 lần).
        private void DungBoLoc(List<HoaDon> tatCaHoaDon)
        {
            if (DanhSachNamLoc.Count == 0)
            {
                DanhSachNamLoc.Add("Tất cả");
                var nams = tatCaHoaDon.Where(h => h.NgayLap.HasValue)
                    .Select(h => h.NgayLap.Value.Year).Distinct().OrderByDescending(y => y).ToList();
                foreach (var n in nams) DanhSachNamLoc.Add(n.ToString());
                OnPropertyChanged(nameof(NamLoc));
            }

            if (DanhSachHangLoc.Count == 0)
            {
                DanhSachHangLoc.Add("Tất cả");
                try
                {
                    using (var ctx = new QuanLyBanXeMayEntities())
                    {
                        ctx.Configuration.LazyLoadingEnabled = false;
                        foreach (var ten in ctx.HangXes.Select(h => h.TenHang).ToList())
                        {
                            if (!string.IsNullOrWhiteSpace(ten)) DanhSachHangLoc.Add(ten);
                        }
                    }
                }
                catch (Exception) { }
                OnPropertyChanged(nameof(HangLoc));
            }
        }

        // Lọc hóa đơn theo năm + hãng đang chọn.
        private List<HoaDon> LocHoaDon(List<HoaDon> nguon)
        {
            IEnumerable<HoaDon> ds = nguon;

            int nam;
            if (NamLoc != "Tất cả" && int.TryParse(NamLoc, out nam))
            {
                ds = ds.Where(h => h.NgayLap.HasValue && h.NgayLap.Value.Year == nam);
            }

            if (HangLoc != "Tất cả")
            {
                var tenXe = LayTenXeTheoHang(HangLoc);
                ds = ds.Where(h => tenXe.Contains(h.TenDV_SP ?? string.Empty));
            }

            return ds.ToList();
        }

        private HashSet<string> LayTenXeTheoHang(string tenHang)
        {
            try
            {
                using (var ctx = new QuanLyBanXeMayEntities())
                {
                    ctx.Configuration.LazyLoadingEnabled = false;
                    var ten = ctx.Xes.Where(x => x.HangXe != null && x.HangXe.TenHang == tenHang)
                        .Select(x => x.TenXe).ToList();
                    return new HashSet<string>(ten, StringComparer.OrdinalIgnoreCase);
                }
            }
            catch (Exception)
            {
                return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            }
        }

        private void TaiDuLieuDoanhThu6Thang(List<HoaDon> danhSachHoaDon)
        {
            DateTime thangBatDau = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-5);
            List<CotDoanhThuThang_VM> duLieu = new List<CotDoanhThuThang_VM>();
            decimal doanhThuLonNhat = 0m;

            for (int i = 0; i < 6; i++)
            {
                DateTime thang = thangBatDau.AddMonths(i);
                decimal doanhThu = danhSachHoaDon
                    .Where(item => item.NgayLap.HasValue && item.NgayLap.Value.Year == thang.Year && item.NgayLap.Value.Month == thang.Month)
                    .Sum(item => item.ThanhTien ?? 0m);

                if (doanhThu > doanhThuLonNhat)
                {
                    doanhThuLonNhat = doanhThu;
                }

                duLieu.Add(new CotDoanhThuThang_VM
                {
                    ThangHienThi = "Thg " + thang.Month,
                    DoanhThuThang = doanhThu,
                    GiaTriHienThi = doanhThu.ToString("N0") + " VNĐ"
                });
            }

            foreach (CotDoanhThuThang_VM cot in duLieu)
            {
                if (doanhThuLonNhat <= 0m)
                {
                    cot.ChieuCaoCot = 20;
                }
                else
                {
                    cot.ChieuCaoCot = Math.Max(20, (double)(cot.DoanhThuThang / doanhThuLonNhat) * 160);
                }
            }

            DuLieuDoanhThu6Thang = new ObservableCollection<CotDoanhThuThang_VM>(duLieu);
        }

        private void TaiDanhSachTop(List<HoaDon> danhSachHoaDon)
        {
            HashSet<string> tapTenXe;
            List<MucThongKeTop_VM> topXe;
            try
            {
                using (var ctx = new QuanLyBanXeMayEntities())
                {
                    tapTenXe = new HashSet<string>(ctx.Xes.Select(x => x.TenXe ?? string.Empty), StringComparer.OrdinalIgnoreCase);

                    // Xe bán chạy lấy trực tiếp từ VIEW vw_XeBanChay của CSDL.
                    topXe = ctx.Database.SqlQuery<XeBanChay_DTO>(
                            "SELECT TenXe, SoLuongBan, DoanhThu FROM vw_XeBanChay")
                        .ToList()
                        .Select(item => new MucThongKeTop_VM
                        {
                            TenMuc = item.TenXe ?? "Không xác định",
                            SoLuongBan = item.SoLuongBan ?? 0,
                            DoanhThu = item.DoanhThu ?? 0m
                        })
                        .OrderByDescending(item => item.DoanhThu)
                        .ThenByDescending(item => item.SoLuongBan)
                        .Take(5)
                        .ToList();
                }
            }
            catch (Exception)
            {
                tapTenXe = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                topXe = new List<MucThongKeTop_VM>();
            }

            List<MucThongKeTop_VM> topDichVu = danhSachHoaDon
                .Where(item => !tapTenXe.Contains(item.TenDV_SP ?? string.Empty))
                .GroupBy(item => item.TenDV_SP ?? "Không xác định")
                .Select(nhom => new MucThongKeTop_VM
                {
                    TenMuc = nhom.Key,
                    SoLuongBan = nhom.Sum(item => item.SoLuong ?? 0),
                    DoanhThu = nhom.Sum(item => item.ThanhTien ?? 0m)
                })
                .OrderByDescending(item => item.DoanhThu)
                .ThenByDescending(item => item.SoLuongBan)
                .Take(5)
                .ToList();

            if (topDichVu.Count == 0)
            {
                topDichVu.Add(new MucThongKeTop_VM { TenMuc = "Chưa có dữ liệu", SoLuongBan = 0, DoanhThu = 0 });
            }

            if (topXe.Count == 0)
            {
                topXe.Add(new MucThongKeTop_VM { TenMuc = "Chưa có dữ liệu", SoLuongBan = 0, DoanhThu = 0 });
            }

            DanhSachDichVuBanChay = new ObservableCollection<MucThongKeTop_VM>(topDichVu);
            DanhSachXeBanChay = new ObservableCollection<MucThongKeTop_VM>(topXe);
        }
    }

    public class CotDoanhThuThang_VM : BaseViewModel
    {
        private string thangHienThi;
        public string ThangHienThi
        {
            get { return thangHienThi; }
            set
            {
                thangHienThi = value;
                OnPropertyChanged();
            }
        }

        private decimal doanhThuThang;
        public decimal DoanhThuThang
        {
            get { return doanhThuThang; }
            set
            {
                doanhThuThang = value;
                OnPropertyChanged();
            }
        }

        private double chieuCaoCot;
        public double ChieuCaoCot
        {
            get { return chieuCaoCot; }
            set
            {
                chieuCaoCot = value;
                OnPropertyChanged();
            }
        }

        private string giaTriHienThi;
        public string GiaTriHienThi
        {
            get { return giaTriHienThi; }
            set
            {
                giaTriHienThi = value;
                OnPropertyChanged();
            }
        }
    }

    public class MucThongKeTop_VM
    {
        public string TenMuc { get; set; }
        public int SoLuongBan { get; set; }
        public decimal DoanhThu { get; set; }
    }

    // Ánh xạ kết quả đọc từ VIEW vw_XeBanChay.
    internal class XeBanChay_DTO
    {
        public string TenXe { get; set; }
        public int? SoLuongBan { get; set; }
        public decimal? DoanhThu { get; set; }
    }

    // Đọc từ vw_ThongKeXe (thống kê xe theo hãng).
    public class ThongKeXe_DTO
    {
        public string MaHang { get; set; }
        public string TenHang { get; set; }
        public int? SoMauXe { get; set; }
        public int? TongTon { get; set; }
        public decimal? GiaTriTon { get; set; }
        public int? SoXeDangBan { get; set; }
        public int? SoXeAn { get; set; }
    }

    // Đọc từ vw_ThongKeDichVu.
    public class ThongKeDichVu_DTO
    {
        public string MaPT { get; set; }
        public string Ten { get; set; }
        public decimal? Gia { get; set; }
        public int? SoLuongBan { get; set; }
        public decimal? DoanhThu { get; set; }
    }

    // Đọc từ vw_ThongKePhuTung.
    public class ThongKePhuTung_DTO
    {
        public string MaPT { get; set; }
        public string Ten { get; set; }
        public decimal? Gia { get; set; }
        public int? TonKho { get; set; }
        public int? SoLuongBan { get; set; }
        public decimal? DoanhThu { get; set; }
    }
}
