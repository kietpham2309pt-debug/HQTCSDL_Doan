using Doan.Helper;
using Doan.Model;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Doan.ViewModel
{
    // Tab Cài đặt: quản lý tài khoản đang đăng nhập (đổi mật khẩu + câu hỏi bảo mật).
    public class CaiDat_VM : BaseViewModel
    {
        private string cauHoiBaoMat;
        public string CauHoiBaoMat
        {
            get { return cauHoiBaoMat; }
            set { cauHoiBaoMat = value; OnPropertyChanged(); }
        }

        private string cauTraLoiBaoMat;
        public string CauTraLoiBaoMat
        {
            get { return cauTraLoiBaoMat; }
            set { cauTraLoiBaoMat = value; OnPropertyChanged(); }
        }

        public ICommand LenhLuuCauHoi { get; }
        private string tenDangNhap;
        public string TenDangNhap
        {
            get { return tenDangNhap; }
            set { tenDangNhap = value; OnPropertyChanged(); }
        }

        private string vaiTro;
        public string VaiTro
        {
            get { return vaiTro; }
            set { vaiTro = value; OnPropertyChanged(); }
        }

        private string tenNhanVien;
        public string TenNhanVien
        {
            get { return tenNhanVien; }
            set { tenNhanVien = value; OnPropertyChanged(); }
        }

        public CaiDat_VM()
        {
            LenhLuuCauHoi = new RelayCommand(_ => LuuCauHoi());
            TaiThongTinTaiKhoan();
        }

        private void TaiThongTinTaiKhoan()
        {
            string username = PhienDangNhap.TenDangNhap ?? string.Empty;
            TenDangNhap = username;
            VaiTro = PhienDangNhap.Role;
            TenNhanVien = "(không có)";

            using (var ctx = new QuanLyBanXeMayEntities())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                var tk = ctx.TaiKhoans.FirstOrDefault(t => t.Username == username);
                if (tk != null)
                {
                    VaiTro = tk.Role;
                    CauHoiBaoMat = tk.CauHoiBaoMat;
                    CauTraLoiBaoMat = tk.CauTraLoiBaoMat;
                    if (!string.IsNullOrEmpty(tk.MaNV))
                    {
                        var nv = ctx.NhanViens.FirstOrDefault(n => n.MaNV == tk.MaNV);
                        if (nv != null)
                        {
                            TenNhanVien = nv.HoTen;
                        }
                    }
                }
            }
        }

        private void LuuCauHoi()
        {
            string username = PhienDangNhap.TenDangNhap ?? string.Empty;
            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Không xác định được tài khoản đang đăng nhập.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(CauHoiBaoMat) || CauHoiBaoMat.Trim().Length < 6)
            {
                MessageBox.Show("Câu hỏi bảo mật phải có ít nhất 6 ký tự.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(CauTraLoiBaoMat) || CauTraLoiBaoMat.Trim().Length < 2)
            {
                MessageBox.Show("Câu trả lời bảo mật phải có ít nhất 2 ký tự.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var ctx = new QuanLyBanXeMayEntities())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                var tk = ctx.TaiKhoans.FirstOrDefault(t => t.Username == username);
                if (tk == null)
                {
                    MessageBox.Show("Không tìm thấy tài khoản.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                tk.CauHoiBaoMat = CauHoiBaoMat.Trim();
                tk.CauTraLoiBaoMat = CauTraLoiBaoMat.Trim();
                ctx.SaveChanges();
            }

            MessageBox.Show("Đã lưu câu hỏi bảo mật. Dùng để khôi phục mật khẩu khi quên.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Trả về true nếu đổi mật khẩu thành công.
        public bool DoiMatKhau(string matKhauCu, string matKhauMoi, string xacNhan)
        {
            string username = PhienDangNhap.TenDangNhap ?? string.Empty;
            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Không xác định được tài khoản đang đăng nhập.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(matKhauMoi) || matKhauMoi.Length < 3)
            {
                MessageBox.Show("Mật khẩu mới phải có ít nhất 3 ký tự.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (matKhauMoi != xacNhan)
            {
                MessageBox.Show("Xác nhận mật khẩu không khớp.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            using (var ctx = new QuanLyBanXeMayEntities())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                var tk = ctx.TaiKhoans.FirstOrDefault(t => t.Username == username);
                if (tk == null)
                {
                    MessageBox.Show("Không tìm thấy tài khoản trong hệ thống.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                // Lối tắt admin/123 vẫn được chấp nhận như mật khẩu cũ hợp lệ.
                bool matKhauCuDung = tk.Password == matKhauCu ||
                    (string.Equals(username, "admin", System.StringComparison.OrdinalIgnoreCase) && matKhauCu == "123");
                if (!matKhauCuDung)
                {
                    MessageBox.Show("Mật khẩu cũ không đúng.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                tk.Password = matKhauMoi;
                ctx.SaveChanges();
            }

            MessageBox.Show("Đổi mật khẩu thành công.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            return true;
        }
    }
}
