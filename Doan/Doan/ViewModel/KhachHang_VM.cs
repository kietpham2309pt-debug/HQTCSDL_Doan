using Doan.Helper;
using Doan.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Net.Mail;
using Microsoft.Win32;

namespace Doan.ViewModel
{
    public class KhachHang_VM : BaseViewModel
    {
        private ObservableCollection<KhachHang> danhSachKhachHang;
        public ObservableCollection<KhachHang> DanhSachKhachHang
        {
            get { return danhSachKhachHang; }
            set
            {
                danhSachKhachHang = value;
                OnPropertyChanged();
            }
        }

        private KhachHang khachHangDangChon;
        public KhachHang KhachHangDangChon
        {
            get { return khachHangDangChon; }
            set
            {
                khachHangDangChon = value;
                OnPropertyChanged();
                if (khachHangDangChon != null)
                {
                    MaKHNhap = khachHangDangChon.MaKH;
                    HoTenNhap = khachHangDangChon.HoTen;
                    SDTNhap = khachHangDangChon.SDT;
                    CCCDNhap = khachHangDangChon.CCCD;
                    EmailNhap = khachHangDangChon.Email;
                    DiaChiNhap = khachHangDangChon.DiaChi;
                    NgaySinhNhap = khachHangDangChon.NgaySinh;
                    GioiTinhNhap = khachHangDangChon.GioiTinh;
                    AnhCaNhanNhap = khachHangDangChon.AnhCaNhan;
                    dangThemMoi = false;
                }
                lenhXoaKhachHang?.RaiseCanExecuteChanged();
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

        private string maKHNhap;
        public string MaKHNhap
        {
            get { return maKHNhap; }
            set
            {
                maKHNhap = value;
                OnPropertyChanged();
            }
        }

        private string hoTenNhap;
        public string HoTenNhap
        {
            get { return hoTenNhap; }
            set
            {
                hoTenNhap = value;
                OnPropertyChanged();
            }
        }

        private string sdtNhap;
        public string SDTNhap
        {
            get { return sdtNhap; }
            set
            {
                sdtNhap = value;
                OnPropertyChanged();
            }
        }

        private string cccdNhap;
        public string CCCDNhap
        {
            get { return cccdNhap; }
            set
            {
                cccdNhap = value;
                OnPropertyChanged();
            }
        }

        private string emailNhap;
        public string EmailNhap
        {
            get { return emailNhap; }
            set
            {
                emailNhap = value;
                OnPropertyChanged();
            }
        }

        private string diaChiNhap;
        public string DiaChiNhap
        {
            get { return diaChiNhap; }
            set
            {
                diaChiNhap = value;
                OnPropertyChanged();
            }
        }

        private Nullable<DateTime> ngaySinhNhap;
        public Nullable<DateTime> NgaySinhNhap
        {
            get { return ngaySinhNhap; }
            set
            {
                ngaySinhNhap = value;
                OnPropertyChanged();
            }
        }

        private string gioiTinhNhap;
        public string GioiTinhNhap
        {
            get { return gioiTinhNhap; }
            set
            {
                gioiTinhNhap = value;
                OnPropertyChanged();
            }
        }

        private string anhCaNhanNhap;
        public string AnhCaNhanNhap
        {
            get { return anhCaNhanNhap; }
            set
            {
                anhCaNhanNhap = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> DanhSachGioiTinh { get; } =
            new ObservableCollection<string> { "Nam", "Nữ", "Khác" };

        private bool dangThemMoi;
        private readonly RelayCommand lenhXoaKhachHang;

        public ICommand LenhThemKhachHang { get; }
        public ICommand LenhXoaKhachHang
        {
            get { return lenhXoaKhachHang; }
        }
        public ICommand LenhLuuKhachHang { get; }
        public ICommand LenhTimKiemKhachHang { get; }
        public ICommand LenhLamMoiKhachHang { get; }
        public ICommand LenhChonAnh { get; }

        public KhachHang_VM()
        {
            LenhThemKhachHang = new RelayCommand(_ => ThemKhachHang());
            lenhXoaKhachHang = new RelayCommand(_ => XoaKhachHang(), _ => KhachHangDangChon != null);
            LenhLuuKhachHang = new RelayCommand(_ => LuuKhachHang());
            LenhTimKiemKhachHang = new RelayCommand(_ => TaiDanhSachKhachHang());
            LenhLamMoiKhachHang = new RelayCommand(_ => LamMoiNhapKhachHang());
            LenhChonAnh = new RelayCommand(_ => ChonAnh());

            DanhSachKhachHang = new ObservableCollection<KhachHang>();
            TaiDanhSachKhachHang();
            LamMoiNhapKhachHang();
        }

        private void TaiDanhSachKhachHang()
        {
            List<KhachHang> danhSachLoc;

            using (var ctx = new QuanLyBanXeMayEntities())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                danhSachLoc = ctx.KhachHangs.ToList();

                // Xếp hạng khách hàng tính bằng FUNCTION fn_XepHangKhachHang của CSDL.
                var bangXepHang = ctx.Database.SqlQuery<XepHangKH_DTO>(
                        "SELECT MaKH, dbo.fn_XepHangKhachHang(MaKH) AS XepHang FROM KhachHang")
                    .ToList()
                    .ToDictionary(item => item.MaKH ?? string.Empty, item => item.XepHang,
                        StringComparer.OrdinalIgnoreCase);

                foreach (var kh in danhSachLoc)
                {
                    string xh;
                    kh.XepHang = bangXepHang.TryGetValue(kh.MaKH ?? string.Empty, out xh)
                        ? xh : string.Empty;
                }
            }

            if (!string.IsNullOrWhiteSpace(TuKhoaTimKiem))
            {
                string tuKhoa = TuKhoaTimKiem.Trim().ToLower();
                danhSachLoc = danhSachLoc.Where(item =>
                    (item.MaKH ?? string.Empty).ToLower().Contains(tuKhoa) ||
                    (item.HoTen ?? string.Empty).ToLower().Contains(tuKhoa) ||
                    (item.SDT ?? string.Empty).ToLower().Contains(tuKhoa)).ToList();
            }

            DanhSachKhachHang = new ObservableCollection<KhachHang>(danhSachLoc);

            if (KhachHangDangChon != null)
            {
                string maDangChon = KhachHangDangChon.MaKH;
                KhachHangDangChon = DanhSachKhachHang.FirstOrDefault(item =>
                    string.Equals(item.MaKH, maDangChon, StringComparison.OrdinalIgnoreCase));
            }

            lenhXoaKhachHang.RaiseCanExecuteChanged();
        }

        private void ThemKhachHang()
        {
            dangThemMoi = true;
            MaKHNhap = TaoMaKhachHangMoi();
            HoTenNhap = string.Empty;
            SDTNhap = string.Empty;
            CCCDNhap = string.Empty;
            EmailNhap = string.Empty;
            DiaChiNhap = string.Empty;
            NgaySinhNhap = null;
            GioiTinhNhap = null;
            AnhCaNhanNhap = string.Empty;
            KhachHangDangChon = null;
        }

        private void XoaKhachHang()
        {
            if (KhachHangDangChon == null)
            {
                return;
            }

            MessageBoxResult ketQua = MessageBox.Show("Bạn có chắc muốn xóa khách hàng đang chọn?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (ketQua != MessageBoxResult.Yes)
            {
                return;
            }

            using (var ctx = new QuanLyBanXeMayEntities())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                var ef = ctx.KhachHangs.FirstOrDefault(k => k.MaKH == KhachHangDangChon.MaKH);
                if (ef != null)
                {
                    ctx.KhachHangs.Remove(ef);
                    ctx.SaveChanges();
                }
            }

            KhachHangDangChon = null;
            LamMoiNhapKhachHang();
            TaiDanhSachKhachHang();
        }

        private void LuuKhachHang()
        {
            if (!KiemTraDuLieuKhachHang())
            {
                return;
            }

            if (dangThemMoi)
            {
                KhachHang khachHangTrungMa;
                using (var ctx = new QuanLyBanXeMayEntities())
                {
                    ctx.Configuration.LazyLoadingEnabled = false;
                    string maKHNhapUpper = MaKHNhap.ToUpper();
                    khachHangTrungMa = ctx.KhachHangs.FirstOrDefault(item =>
                        item.MaKH == maKHNhapUpper);
                }

                if (khachHangTrungMa != null)
                {
                    MessageBox.Show("Mã khách hàng đã tồn tại.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (KiemTraTrungThongTinKhachHang(null))
                {
                    return;
                }

                using (var ctx = new QuanLyBanXeMayEntities())
                {
                    ctx.Configuration.LazyLoadingEnabled = false;
                    var ef = new KhachHang
                    {
                        MaKH = MaKHNhap.Trim().ToUpper(),
                        HoTen = HoTenNhap.Trim(),
                        SDT = SDTNhap.Trim(),
                        CCCD = CCCDNhap.Trim(),
                        Email = EmailNhap.Trim(),
                        DiaChi = DiaChiNhap.Trim(),
                        NgaySinh = NgaySinhNhap,
                        GioiTinh = GioiTinhNhap,
                        AnhCaNhan = (AnhCaNhanNhap ?? string.Empty).Trim()
                    };
                    ctx.KhachHangs.Add(ef);
                    ctx.SaveChanges();
                }

                dangThemMoi = false;
                TaiDanhSachKhachHang();
                KhachHangDangChon = DanhSachKhachHang.FirstOrDefault(item => item.MaKH == MaKHNhap.Trim().ToUpper());
                return;
            }

            if (KhachHangDangChon == null)
            {
                MessageBox.Show("Vui lòng chọn khách hàng để cập nhật.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            KhachHang khachHangTrungKhac;
            using (var ctx = new QuanLyBanXeMayEntities())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                string maKHNhapUpper2 = MaKHNhap.ToUpper();
                string maKHHienTai = KhachHangDangChon.MaKH;
                khachHangTrungKhac = ctx.KhachHangs.FirstOrDefault(item =>
                    item.MaKH != maKHHienTai &&
                    item.MaKH == maKHNhapUpper2);
            }

            if (khachHangTrungKhac != null)
            {
                MessageBox.Show("Mã khách hàng đã tồn tại.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (KiemTraTrungThongTinKhachHang(KhachHangDangChon))
            {
                return;
            }

            using (var ctx = new QuanLyBanXeMayEntities())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                var ef = ctx.KhachHangs.FirstOrDefault(k => k.MaKH == KhachHangDangChon.MaKH);
                if (ef != null)
                {
                    // Không gán lại MaKH (khóa chính) vì EF không cho sửa khóa và sẽ làm văng app.
                    ef.HoTen = HoTenNhap.Trim();
                    ef.SDT = SDTNhap.Trim();
                    ef.CCCD = CCCDNhap.Trim();
                    ef.Email = EmailNhap.Trim();
                    ef.DiaChi = DiaChiNhap.Trim();
                    ef.NgaySinh = NgaySinhNhap;
                    ef.GioiTinh = GioiTinhNhap;
                    ef.AnhCaNhan = (AnhCaNhanNhap ?? string.Empty).Trim();
                    ctx.SaveChanges();
                }
            }

            TaiDanhSachKhachHang();
            KhachHangDangChon = DanhSachKhachHang.FirstOrDefault(item => item.MaKH == MaKHNhap.Trim().ToUpper());
        }

        private bool KiemTraTrungThongTinKhachHang(KhachHang khachHangDangBoQua)
        {
            string sdt = SDTNhap.Trim();
            string cccd = CCCDNhap.Trim();
            string email = EmailNhap.Trim();
            // Chỉ truyền giá trị nguyên thủy (MaKH) vào query, không truyền cả entity
            // để tránh lỗi "Unable to create a constant value of type ... KhachHang".
            string maBoQua = khachHangDangBoQua != null ? khachHangDangBoQua.MaKH : null;

            using (var ctx = new QuanLyBanXeMayEntities())
            {
                ctx.Configuration.LazyLoadingEnabled = false;

                var trungSDT = ctx.KhachHangs.FirstOrDefault(item =>
                    (maBoQua == null || item.MaKH != maBoQua) &&
                    item.SDT == sdt);
                if (trungSDT != null)
                {
                    MessageBox.Show("Số điện thoại đã tồn tại trong hệ thống.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return true;
                }

                var trungCCCD = ctx.KhachHangs.FirstOrDefault(item =>
                    (maBoQua == null || item.MaKH != maBoQua) &&
                    item.CCCD == cccd);
                if (trungCCCD != null)
                {
                    MessageBox.Show("CCCD đã tồn tại trong hệ thống.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return true;
                }

                if (!string.IsNullOrWhiteSpace(email))
                {
                    var trungEmail = ctx.KhachHangs.FirstOrDefault(item =>
                        (maBoQua == null || item.MaKH != maBoQua) &&
                        item.Email == email);
                    if (trungEmail != null)
                    {
                        MessageBox.Show("Email đã tồn tại trong hệ thống.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return true;
                    }
                }
            }

            return false;
        }

        private bool KiemTraDuLieuKhachHang()
        {
            if (string.IsNullOrWhiteSpace(MaKHNhap))
            {
                MessageBox.Show("Mã khách hàng không được để trống.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(HoTenNhap))
            {
                MessageBox.Show("Họ tên không được để trống.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (HoTenNhap.Trim().Length < 2)
            {
                MessageBox.Show("Họ tên phải có ít nhất 2 ký tự.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!KiemTraChuoiSo(SDTNhap, 10, 11))
            {
                MessageBox.Show("Số điện thoại phải là 10 đến 11 chữ số.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!KiemTraChuoiSo(CCCDNhap, 9, 12))
            {
                MessageBox.Show("CCCD phải từ 9 đến 12 chữ số.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(DiaChiNhap))
            {
                MessageBox.Show("Địa chỉ không được để trống.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!string.IsNullOrWhiteSpace(EmailNhap))
            {
                string emailKiemTra = EmailNhap.Trim();
                int viTriAt = emailKiemTra.IndexOf('@');
                bool emailHopLe = viTriAt > 0 &&
                    viTriAt < emailKiemTra.Length - 1 &&
                    emailKiemTra.LastIndexOf('.') > viTriAt;
                if (!emailHopLe)
                {
                    MessageBox.Show("Email không hợp lệ.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
            }

            return true;
        }

        private bool KiemTraChuoiSo(string duLieu, int doDaiMin, int doDaiMax)
        {
            if (string.IsNullOrWhiteSpace(duLieu))
            {
                return false;
            }

            string giaTri = duLieu.Trim();
            if (giaTri.Length < doDaiMin || giaTri.Length > doDaiMax)
            {
                return false;
            }

            foreach (char kyTu in giaTri)
            {
                if (kyTu < '0' || kyTu > '9')
                {
                    return false;
                }
            }

            return true;
        }

        private string TaoMaKhachHangMoi()
        {
            int maLonNhat = 0;

            using (var ctx = new QuanLyBanXeMayEntities())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                var all = ctx.KhachHangs.Select(k => k.MaKH).ToList();
                foreach (string ma in all)
                {
                    if (string.IsNullOrWhiteSpace(ma))
                    {
                        continue;
                    }

                    string so = ma.Trim().ToUpper().Replace("KH", string.Empty);
                    int maSo;
                    if (int.TryParse(so, out maSo) && maSo > maLonNhat)
                    {
                        maLonNhat = maSo;
                    }
                }
            }

            return "KH" + (maLonNhat + 1).ToString("000");
        }

        private void ChonAnh()
        {
            var hopThoai = new OpenFileDialog
            {
                Title = "Chọn ảnh cá nhân khách hàng",
                Filter = "Tệp ảnh (*.png;*.jpg;*.jpeg;*.bmp)|*.png;*.jpg;*.jpeg;*.bmp"
            };
            if (hopThoai.ShowDialog() == true)
            {
                AnhCaNhanNhap = hopThoai.FileName;
            }
        }

        // Ánh xạ kết quả gọi FUNCTION fn_XepHangKhachHang.
        private class XepHangKH_DTO
        {
            public string MaKH { get; set; }
            public string XepHang { get; set; }
        }

        private void LamMoiNhapKhachHang()
        {
            dangThemMoi = false;
            MaKHNhap = string.Empty;
            HoTenNhap = string.Empty;
            SDTNhap = string.Empty;
            CCCDNhap = string.Empty;
            EmailNhap = string.Empty;
            DiaChiNhap = string.Empty;
            NgaySinhNhap = null;
            GioiTinhNhap = null;
            AnhCaNhanNhap = string.Empty;
        }
    }
}
