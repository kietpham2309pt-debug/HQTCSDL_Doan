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

namespace Doan.ViewModel
{
    public class NhanVien_VM : BaseViewModel
    {
        private ObservableCollection<NhanVien> danhSachNhanVien;
        public ObservableCollection<NhanVien> DanhSachNhanVien
        {
            get { return danhSachNhanVien; }
            set
            {
                danhSachNhanVien = value;
                OnPropertyChanged();
            }
        }

        private NhanVien nhanVienDangChon;
        public NhanVien NhanVienDangChon
        {
            get { return nhanVienDangChon; }
            set
            {
                nhanVienDangChon = value;
                OnPropertyChanged();
                if (nhanVienDangChon != null)
                {
                    MaNVNhap = nhanVienDangChon.MaNV;
                    HoTenNhap = nhanVienDangChon.HoTen;
                    SDTNhap = nhanVienDangChon.SDT;
                    ChucVuNhap = nhanVienDangChon.ChucVu;
                    NgayVaoLamNhap = nhanVienDangChon.NgayVaoLam ?? DateTime.Now;
                    TrangThaiNhap = nhanVienDangChon.TrangThai;
                    dangThemMoi = false;
                    dangSua = false;
                }
                lenhSuaNhanVien?.RaiseCanExecuteChanged();
                lenhXoaNhanVien?.RaiseCanExecuteChanged();
                lenhChoNghiViec?.RaiseCanExecuteChanged();
                lenhCapMatKhau?.RaiseCanExecuteChanged();
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
                TaiDanhSachNhanVien(); // lọc trực tiếp khi gõ
            }
        }

        // Gợi ý tìm kiếm (tên / mã / SĐT nhân viên).
        public ObservableCollection<string> GoiYTimKiem { get; } = new ObservableCollection<string>();

        private string maNVNhap;
        public string MaNVNhap
        {
            get { return maNVNhap; }
            set
            {
                maNVNhap = value;
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

        private string chucVuNhap;
        public string ChucVuNhap
        {
            get { return chucVuNhap; }
            set
            {
                chucVuNhap = value;
                OnPropertyChanged();
            }
        }

        private DateTime ngayVaoLamNhap;
        public DateTime NgayVaoLamNhap
        {
            get { return ngayVaoLamNhap; }
            set
            {
                ngayVaoLamNhap = value;
                OnPropertyChanged();
            }
        }

        private string trangThaiNhap;
        public string TrangThaiNhap
        {
            get { return trangThaiNhap; }
            set
            {
                trangThaiNhap = value;
                OnPropertyChanged();
            }
        }

        private bool dangThemMoi;
        private bool dangSua;
        private readonly RelayCommand lenhSuaNhanVien;
        private readonly RelayCommand lenhXoaNhanVien;
        private readonly RelayCommand lenhChoNghiViec;
        private readonly RelayCommand lenhCapMatKhau;

        public ICommand LenhThemNhanVien { get; }
        public ICommand LenhSuaNhanVien
        {
            get { return lenhSuaNhanVien; }
        }
        public ICommand LenhXoaNhanVien
        {
            get { return lenhXoaNhanVien; }
        }
        public ICommand LenhChoNghiViec
        {
            get { return lenhChoNghiViec; }
        }
        public ICommand LenhCapMatKhau
        {
            get { return lenhCapMatKhau; }
        }
        public ICommand LenhLuuNhanVien { get; }
        public ICommand LenhTimKiemNhanVien { get; }
        public ICommand LenhHuyNhapNhanVien { get; }

        public NhanVien_VM()
        {
            LenhThemNhanVien = new RelayCommand(_ => ThemNhanVien());
            lenhSuaNhanVien = new RelayCommand(_ => SuaNhanVien(), _ => NhanVienDangChon != null);
            lenhXoaNhanVien = new RelayCommand(_ => XoaNhanVien(), _ => NhanVienDangChon != null);
            lenhChoNghiViec = new RelayCommand(_ => ChoNghiViec(), _ => NhanVienDangChon != null);
            lenhCapMatKhau = new RelayCommand(_ => CapLaiMatKhau(), _ => NhanVienDangChon != null);
            LenhLuuNhanVien = new RelayCommand(_ => LuuNhanVien());
            LenhTimKiemNhanVien = new RelayCommand(_ => TaiDanhSachNhanVien());
            LenhHuyNhapNhanVien = new RelayCommand(_ => HuyNhapNhanVien());

            DanhSachNhanVien = new ObservableCollection<NhanVien>();
            NgayVaoLamNhap = DateTime.Now;
            TrangThaiNhap = "Đang làm việc";
            TaiDanhSachNhanVien();
        }

        private void TaiDanhSachNhanVien()
        {
            List<NhanVien> danhSachLoc;

            using (var ctx = new QuanLyBanXeMayEntities())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                danhSachLoc = ctx.NhanViens.ToList();
            }

            CapNhatGoiY(danhSachLoc);

            if (!string.IsNullOrWhiteSpace(TuKhoaTimKiem))
            {
                string tuKhoa = TuKhoaTimKiem.Trim().ToLower();
                danhSachLoc = danhSachLoc.Where(item =>
                    (item.MaNV ?? string.Empty).ToLower().Contains(tuKhoa) ||
                    (item.HoTen ?? string.Empty).ToLower().Contains(tuKhoa) ||
                    (item.SDT ?? string.Empty).ToLower().Contains(tuKhoa) ||
                    (item.ChucVu ?? string.Empty).ToLower().Contains(tuKhoa)).ToList();
            }

            DanhSachNhanVien = new ObservableCollection<NhanVien>(danhSachLoc);

            if (NhanVienDangChon != null)
            {
                string maDangChon = NhanVienDangChon.MaNV;
                NhanVienDangChon = DanhSachNhanVien.FirstOrDefault(item =>
                    string.Equals(item.MaNV, maDangChon, StringComparison.OrdinalIgnoreCase));
            }

            lenhSuaNhanVien.RaiseCanExecuteChanged();
            lenhXoaNhanVien.RaiseCanExecuteChanged();
        }

        private void CapNhatGoiY(List<NhanVien> danhSachDayDu)
        {
            if (GoiYTimKiem.Count > 0) return; // chỉ dựng 1 lần
            foreach (var nv in danhSachDayDu)
            {
                if (!string.IsNullOrWhiteSpace(nv.HoTen)) GoiYTimKiem.Add(nv.HoTen);
                if (!string.IsNullOrWhiteSpace(nv.MaNV)) GoiYTimKiem.Add(nv.MaNV);
                if (!string.IsNullOrWhiteSpace(nv.SDT)) GoiYTimKiem.Add(nv.SDT);
            }
        }

        private void ThemNhanVien()
        {
            dangThemMoi = true;
            dangSua = false;
            MaNVNhap = TaoMaNhanVienMoi();
            HoTenNhap = string.Empty;
            SDTNhap = string.Empty;
            ChucVuNhap = string.Empty;
            NgayVaoLamNhap = DateTime.Now;
            TrangThaiNhap = "Đang làm việc";
            NhanVienDangChon = null;
        }

        private void SuaNhanVien()
        {
            if (NhanVienDangChon == null)
            {
                return;
            }

            dangThemMoi = false;
            dangSua = true;
            MaNVNhap = NhanVienDangChon.MaNV;
            HoTenNhap = NhanVienDangChon.HoTen;
            SDTNhap = NhanVienDangChon.SDT;
            ChucVuNhap = NhanVienDangChon.ChucVu;
            NgayVaoLamNhap = NhanVienDangChon.NgayVaoLam ?? DateTime.Now;
            TrangThaiNhap = NhanVienDangChon.TrangThai;
        }

        private void XoaNhanVien()
        {
            if (NhanVienDangChon == null)
            {
                return;
            }

            MessageBoxResult ketQua = MessageBox.Show("Bạn có chắc muốn xóa nhân viên đang chọn?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (ketQua != MessageBoxResult.Yes)
            {
                return;
            }

            using (var ctx = new QuanLyBanXeMayEntities())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                var ef = ctx.NhanViens.FirstOrDefault(n => n.MaNV == NhanVienDangChon.MaNV);
                if (ef != null)
                {
                    ctx.NhanViens.Remove(ef);
                    ctx.SaveChanges();
                }
            }

            NhanVienDangChon = null;
            HuyNhapNhanVien();
            TaiDanhSachNhanVien();
        }

        private void LuuNhanVien()
        {
            if (!KiemTraDuLieuNhanVien())
            {
                return;
            }

            if (dangThemMoi)
            {
                NhanVien nhanVienTrung;
                using (var ctx = new QuanLyBanXeMayEntities())
                {
                    ctx.Configuration.LazyLoadingEnabled = false;
                    string maNVNhapUpper = MaNVNhap.ToUpper();
                    nhanVienTrung = ctx.NhanViens.FirstOrDefault(item =>
                        item.MaNV == maNVNhapUpper);
                }

                if (nhanVienTrung != null)
                {
                    MessageBox.Show("Mã nhân viên đã tồn tại.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (KiemTraTrungSDTNhanVien(null))
                {
                    return;
                }

                using (var ctx = new QuanLyBanXeMayEntities())
                {
                    ctx.Configuration.LazyLoadingEnabled = false;
                    var ef = new NhanVien
                    {
                        MaNV = MaNVNhap.Trim().ToUpper(),
                        HoTen = HoTenNhap.Trim(),
                        SDT = SDTNhap.Trim(),
                        ChucVu = ChucVuNhap.Trim(),
                        NgayVaoLam = NgayVaoLamNhap,
                        TrangThai = ChuanHoaTrangThai(TrangThaiNhap)
                    };
                    ctx.NhanViens.Add(ef);
                    ctx.SaveChanges();
                }

                dangThemMoi = false;
                TaiDanhSachNhanVien();
                NhanVienDangChon = DanhSachNhanVien.FirstOrDefault(item => item.MaNV == MaNVNhap.Trim().ToUpper());
                return;
            }

            if (dangSua && NhanVienDangChon != null)
            {
                NhanVien nhanVienTrungKhac;
                using (var ctx = new QuanLyBanXeMayEntities())
                {
                    ctx.Configuration.LazyLoadingEnabled = false;
                    string maNVNhapUpper2 = MaNVNhap.ToUpper();
                    string maNVHienTai = NhanVienDangChon.MaNV;
                    nhanVienTrungKhac = ctx.NhanViens.FirstOrDefault(item =>
                        item.MaNV != maNVHienTai &&
                        item.MaNV == maNVNhapUpper2);
                }

                if (nhanVienTrungKhac != null)
                {
                    MessageBox.Show("Mã nhân viên đã tồn tại.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (KiemTraTrungSDTNhanVien(NhanVienDangChon))
                {
                    return;
                }

                using (var ctx = new QuanLyBanXeMayEntities())
                {
                    ctx.Configuration.LazyLoadingEnabled = false;
                    var ef = ctx.NhanViens.FirstOrDefault(n => n.MaNV == NhanVienDangChon.MaNV);
                    if (ef != null)
                    {
                        // Không gán lại MaNV (khóa chính) vì EF không cho sửa khóa và sẽ làm văng app.
                        ef.HoTen = HoTenNhap.Trim();
                        ef.SDT = SDTNhap.Trim();
                        ef.ChucVu = ChucVuNhap.Trim();
                        ef.NgayVaoLam = NgayVaoLamNhap;
                        ef.TrangThai = ChuanHoaTrangThai(TrangThaiNhap);
                        ctx.SaveChanges();
                    }
                }

                dangSua = false;
                TaiDanhSachNhanVien();
                NhanVienDangChon = DanhSachNhanVien.FirstOrDefault(item => item.MaNV == MaNVNhap.Trim().ToUpper());
                return;
            }

            MessageBox.Show("Vui lòng bấm Thêm hoặc Sửa trước khi Lưu.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private bool KiemTraTrungSDTNhanVien(NhanVien nhanVienDangBoQua)
        {
            string sdt = SDTNhap.Trim();
            // Chỉ truyền giá trị nguyên thủy (MaNV) vào query, không truyền cả entity
            // để tránh lỗi "Unable to create a constant value of type ... NhanVien".
            string maBoQua = nhanVienDangBoQua != null ? nhanVienDangBoQua.MaNV : null;

            using (var ctx = new QuanLyBanXeMayEntities())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                var nhanVienTrungSDT = ctx.NhanViens.FirstOrDefault(item =>
                    (maBoQua == null || item.MaNV != maBoQua) &&
                    item.SDT == sdt);

                if (nhanVienTrungSDT != null)
                {
                    MessageBox.Show("Số điện thoại đã tồn tại trong hệ thống.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return true;
                }
            }

            return false;
        }

        private string ChuanHoaTrangThai(string trangThai)
        {
            string giaTri = (trangThai ?? string.Empty).Trim();

            if (string.Equals(giaTri, "Da nghi viec", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(giaTri, "Đã nghỉ việc", StringComparison.OrdinalIgnoreCase))
            {
                return "Đã nghỉ việc";
            }

            if (string.Equals(giaTri, "Tam nghi", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(giaTri, "Tạm nghỉ", StringComparison.OrdinalIgnoreCase))
            {
                return "Tạm nghỉ";
            }

            return "Đang làm việc";
        }

        // Cho nhân viên nghỉ việc -> đặt trạng thái 'Đã nghỉ việc' và KHÓA tài khoản đăng nhập.
        // (Trigger trg_NhanVien_KhoaTaiKhoan cũng tự khóa; ở đây khóa luôn trong app cho chắc chắn.)
        private void ChoNghiViec()
        {
            if (NhanVienDangChon == null)
            {
                return;
            }

            string maNV = NhanVienDangChon.MaNV;
            string hoTen = NhanVienDangChon.HoTen;

            var ketQua = MessageBox.Show(
                "Cho nhân viên '" + hoTen + "' nghỉ việc?\nTài khoản đăng nhập của nhân viên này (nếu có) sẽ bị KHÓA.",
                "Xác nhận nghỉ việc", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (ketQua != MessageBoxResult.Yes)
            {
                return;
            }

            using (var ctx = new QuanLyBanXeMayEntities())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                var nv = ctx.NhanViens.FirstOrDefault(n => n.MaNV == maNV);
                if (nv != null)
                {
                    nv.TrangThai = "Đã nghỉ việc";
                }
                var tk = ctx.TaiKhoans.FirstOrDefault(t => t.MaNV == maNV);
                if (tk != null)
                {
                    tk.TrangThai = "Đã khóa";
                }
                ctx.SaveChanges();
            }

            TaiDanhSachNhanVien();
            NhanVienDangChon = DanhSachNhanVien.FirstOrDefault(item => item.MaNV == maNV);
            MessageBox.Show("Đã cho nghỉ việc và khóa tài khoản đăng nhập (nếu có).", "Hoàn tất", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Admin cấp lại mật khẩu cho tài khoản của nhân viên đang chọn (đặt về mật khẩu mặc định).
        private void CapLaiMatKhau()
        {
            if (NhanVienDangChon == null)
            {
                return;
            }

            string maNV = NhanVienDangChon.MaNV;
            const string matKhauMacDinh = "123456";

            using (var ctx = new QuanLyBanXeMayEntities())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                var tk = ctx.TaiKhoans.FirstOrDefault(t => t.MaNV == maNV);
                if (tk == null)
                {
                    MessageBox.Show("Nhân viên này chưa có tài khoản đăng nhập.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var ketQua = MessageBox.Show(
                    "Cấp lại mật khẩu cho tài khoản '" + tk.Username + "' về mặc định '" + matKhauMacDinh + "'?",
                    "Xác nhận cấp mật khẩu", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (ketQua != MessageBoxResult.Yes)
                {
                    return;
                }

                tk.Password = matKhauMacDinh;
                ctx.SaveChanges();

                MessageBox.Show("Đã cấp lại mật khẩu cho '" + tk.Username + "' = " + matKhauMacDinh +
                    "\nNhân viên nên đăng nhập và đổi lại mật khẩu.", "Hoàn tất", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private bool KiemTraDuLieuNhanVien()
        {
            if (string.IsNullOrWhiteSpace(MaNVNhap))
            {
                MessageBox.Show("Mã nhân viên không được để trống.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
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

            if (string.IsNullOrWhiteSpace(ChucVuNhap))
            {
                MessageBox.Show("Chức vụ không được để trống.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (NgayVaoLamNhap > DateTime.Now)
            {
                MessageBox.Show("Ngày vào làm không hợp lệ.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(TrangThaiNhap))
            {
                MessageBox.Show("Trạng thái không được để trống.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            string ttChuan = ChuanHoaTrangThai(TrangThaiNhap);
            if (ttChuan != "Đang làm việc" && ttChuan != "Tạm nghỉ" && ttChuan != "Đã nghỉ việc")
            {
                MessageBox.Show("Trạng thái chỉ được là Đang làm việc, Tạm nghỉ hoặc Đã nghỉ việc.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
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

        private string TaoMaNhanVienMoi()
        {
            int maLonNhat = 0;

            using (var ctx = new QuanLyBanXeMayEntities())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                var all = ctx.NhanViens.Select(n => n.MaNV).ToList();
                foreach (string ma in all)
                {
                    if (string.IsNullOrWhiteSpace(ma))
                    {
                        continue;
                    }

                    string so = ma.Trim().ToUpper().Replace("NV", string.Empty);
                    int maSo;
                    if (int.TryParse(so, out maSo) && maSo > maLonNhat)
                    {
                        maLonNhat = maSo;
                    }
                }
            }

            return "NV" + (maLonNhat + 1).ToString("000");
        }

        private void HuyNhapNhanVien()
        {
            dangThemMoi = false;
            dangSua = false;
            MaNVNhap = string.Empty;
            HoTenNhap = string.Empty;
            SDTNhap = string.Empty;
            ChucVuNhap = string.Empty;
            NgayVaoLamNhap = DateTime.Now;
            TrangThaiNhap = "Đang làm việc";
        }
    }
}
