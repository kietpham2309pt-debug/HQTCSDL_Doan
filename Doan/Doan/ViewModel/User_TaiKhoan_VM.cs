using Doan.Helper;
using Doan.Model;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Doan.ViewModel
{
    public class User_TaiKhoan_VM : BaseViewModel
    {
        private string maKH;
        public string MaKH
        {
            get { return maKH; }
            set { maKH = value; OnPropertyChanged(); }
        }

        private string hoTen;
        public string HoTen
        {
            get { return hoTen; }
            set { hoTen = value; OnPropertyChanged(); }
        }

        private string sdt;
        public string SDT
        {
            get { return sdt; }
            set { sdt = value; OnPropertyChanged(); }
        }

        private string cccd;
        public string CCCD
        {
            get { return cccd; }
            set { cccd = value; OnPropertyChanged(); }
        }

        private string email;
        public string Email
        {
            get { return email; }
            set { email = value; OnPropertyChanged(); }
        }

        private string diaChi;
        public string DiaChi
        {
            get { return diaChi; }
            set { diaChi = value; OnPropertyChanged(); }
        }

        private string trangThai;
        public string TrangThai
        {
            get { return trangThai; }
            set { trangThai = value; OnPropertyChanged(); }
        }

        public ICommand LenhLuu { get; }

        public User_TaiKhoan_VM()
        {
            LenhLuu = new RelayCommand(_ => LuuThongTin());
            TaiThongTinTuPhien();
        }

        private void TaiThongTinTuPhien()
        {
            var kh = PhienDangNhap.KhachHangHienTai;
            if (kh != null)
            {
                MaKH = kh.MaKH;
                HoTen = kh.HoTen;
                SDT = kh.SDT;
                CCCD = kh.CCCD;
                Email = kh.Email;
                DiaChi = kh.DiaChi;
                TrangThai = "Đã đăng nhập với hồ sơ khách hàng.";
            }
            else
            {
                MaKH = "(Tự sinh khi lưu)";
                HoTen = string.Empty;
                SDT = PhienDangNhap.TenDangNhap ?? string.Empty;
                CCCD = string.Empty;
                Email = string.Empty;
                DiaChi = string.Empty;
                TrangThai = "Chưa có hồ sơ. Vui lòng điền thông tin và nhấn LƯU để tạo mới.";
            }
        }

        private void LuuThongTin()
        {
            if (string.IsNullOrWhiteSpace(HoTen))
            {
                MessageBox.Show("Vui lòng nhập họ tên.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(SDT) || SDT.Trim().Length < 9 || SDT.Trim().Length > 11)
            {
                MessageBox.Show("Số điện thoại phải từ 9-11 chữ số.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            foreach (char kyTu in SDT.Trim())
            {
                if (kyTu < '0' || kyTu > '9')
                {
                    MessageBox.Show("Số điện thoại không hợp lệ.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            // Giữ liên kết tài khoản ↔ khách hàng: SĐT hồ sơ phải trùng tên đăng nhập.
            string tenDangNhap = (PhienDangNhap.TenDangNhap ?? string.Empty).Trim();
            if (!string.IsNullOrEmpty(tenDangNhap) && SDT.Trim() != tenDangNhap)
            {
                MessageBox.Show("Số điện thoại phải trùng với tên đăng nhập (" + tenDangNhap + ").",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var ctx = new QuanLyBanXeMayEntities())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                KhachHang ef = null;

                if (PhienDangNhap.KhachHangHienTai != null)
                {
                    ef = ctx.KhachHangs.FirstOrDefault(k => k.MaKH == PhienDangNhap.KhachHangHienTai.MaKH);
                }
                else
                {
                    ef = ctx.KhachHangs.FirstOrDefault(k => k.SDT == SDT.Trim());
                }

                // Kiểm tra trùng SĐT/CCCD/Email với khách khác (bỏ qua chính hồ sơ này).
                string maHienTai = ef != null ? ef.MaKH : string.Empty;
                string sdtNhap = SDT.Trim();
                string cccdNhap = (CCCD ?? string.Empty).Trim();
                string emailNhap = (Email ?? string.Empty).Trim();

                if (ctx.KhachHangs.Any(k => k.SDT == sdtNhap && k.MaKH != maHienTai))
                {
                    MessageBox.Show("Số điện thoại này đã có hồ sơ khách hàng khác.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (cccdNhap.Length > 0 && ctx.KhachHangs.Any(k => k.CCCD == cccdNhap && k.MaKH != maHienTai))
                {
                    MessageBox.Show("CCCD này đã được dùng cho hồ sơ khác.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (emailNhap.Length > 0 && ctx.KhachHangs.Any(k => k.Email == emailNhap && k.MaKH != maHienTai))
                {
                    MessageBox.Show("Email này đã được dùng cho hồ sơ khác.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (ef == null)
                {
                    ef = new KhachHang
                    {
                        MaKH = TaoMaKhachHangMoi(ctx),
                        HoTen = HoTen.Trim(),
                        SDT = SDT.Trim(),
                        CCCD = (CCCD ?? string.Empty).Trim(),
                        Email = (Email ?? string.Empty).Trim(),
                        DiaChi = (DiaChi ?? string.Empty).Trim()
                    };
                    ctx.KhachHangs.Add(ef);
                }
                else
                {
                    ef.HoTen = HoTen.Trim();
                    ef.SDT = SDT.Trim();
                    ef.CCCD = (CCCD ?? string.Empty).Trim();
                    ef.Email = (Email ?? string.Empty).Trim();
                    ef.DiaChi = (DiaChi ?? string.Empty).Trim();
                }

                ctx.SaveChanges();
                PhienDangNhap.KhachHangHienTai = ef;
                MaKH = ef.MaKH;
            }

            TrangThai = "Đã lưu thông tin thành công.";
            MessageBox.Show("Lưu thông tin thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private string TaoMaKhachHangMoi(QuanLyBanXeMayEntities ctx)
        {
            int maLonNhat = 0;
            var tatCa = ctx.KhachHangs.Select(k => k.MaKH).ToList();
            foreach (string ma in tatCa)
            {
                if (string.IsNullOrWhiteSpace(ma)) continue;
                string so = ma.Trim().ToUpper().Replace("KH", string.Empty);
                int maSo;
                if (int.TryParse(so, out maSo) && maSo > maLonNhat)
                {
                    maLonNhat = maSo;
                }
            }
            return "KH" + (maLonNhat + 1).ToString("000");
        }
    }
}
