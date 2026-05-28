using Doan.Helper;
using Doan.Model;
using Doan.View;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Mouse = System.Windows.Input.Mouse;
using Cursors = System.Windows.Input.Cursors;

namespace Doan.ViewModel
{
    public class DangNhap_VM : BaseViewModel
    {
        private string tenDangNhap;
        public string TenDangNhap
        {
            get { return tenDangNhap; }
            set
            {
                tenDangNhap = value;
                OnPropertyChanged();
            }
        }

        private bool laKhachHang;
        public bool LaKhachHang
        {
            get { return laKhachHang; }
            set
            {
                laKhachHang = value;
                OnPropertyChanged();
            }
        }

        private bool laAdmin;
        public bool LaAdmin
        {
            get { return laAdmin; }
            set
            {
                laAdmin = value;
                OnPropertyChanged();
            }
        }

        public ICommand LenhDangNhap { get; }
        public ICommand LenhMoDangKy { get; }

        public DangNhap_VM()
        {
            LaKhachHang = true;
            LaAdmin = false;
            LenhDangNhap = new RelayCommand(thamSo => DangNhap(thamSo as Window));
            LenhMoDangKy = new RelayCommand(thamSo => MoDangKy(thamSo as Window));
        }

        private void DangNhap(Window cuaSoDangNhap)
        {
            try
            {
                string ten = (TenDangNhap ?? string.Empty).Trim();
                string matKhau = LayMatKhauTuCuaSo(cuaSoDangNhap);

                if (string.IsNullOrWhiteSpace(ten))
                {
                    MessageBox.Show("Vui lòng nhập tên đăng nhập.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (string.IsNullOrWhiteSpace(matKhau))
                {
                    MessageBox.Show("Vui lòng nhập mật khẩu.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                Mouse.OverrideCursor = Cursors.Wait;
                try
                {
                    if (LaAdmin)
                    {
                        if (!XacThucAdmin(ten, matKhau))
                        {
                            Mouse.OverrideCursor = null;
                            MessageBox.Show("Sai tên đăng nhập hoặc mật khẩu admin.", "Thông báo",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }

                        PhienDangNhap.TenDangNhap = ten;
                        PhienDangNhap.Role = "Admin";
                        PhienDangNhap.KhachHangHienTai = null;
                        PhienDangNhap.GioHangKhach.Clear();

                        Mouse.OverrideCursor = null;
                        var cuaSoChinh = new MainWindow();
                        cuaSoChinh.Show();
                        DongCuaSoDangNhap(cuaSoDangNhap);
                        return;
                    }

                    KhachHang khachHang;
                    if (!XacThucKhachHang(ten, matKhau, out khachHang))
                    {
                        Mouse.OverrideCursor = null;
                        MessageBox.Show("Sai tên đăng nhập hoặc mật khẩu.\nNếu chưa có tài khoản, vui lòng nhấn 'Đăng ký ngay'.",
                            "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    PhienDangNhap.TenDangNhap = ten;
                    PhienDangNhap.Role = "KhachHang";
                    PhienDangNhap.KhachHangHienTai = khachHang;
                    PhienDangNhap.GioHangKhach.Clear();

                    Mouse.OverrideCursor = null;
                    var cuaSoUser = new MainWindow_User();
                    cuaSoUser.Show();
                    DongCuaSoDangNhap(cuaSoDangNhap);
                }
                finally
                {
                    Mouse.OverrideCursor = null;
                }
            }
            catch (Exception ex)
            {
                Mouse.OverrideCursor = null;
                MessageBox.Show("Lỗi khi đăng nhập: " + LayThongDiepLoi(ex) +
                    "\n\nKiểm tra SQL Server và database 'QuanLyBanXeMay'." +
                    "\nConnection string đang trỏ tới: " + LayConnectionInfo(),
                    "Lỗi đăng nhập", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string LayConnectionInfo()
        {
            try
            {
                using (var ctx = new QuanLyBanXeMayEntities())
                {
                    return ctx.Database.Connection.DataSource + " / " + ctx.Database.Connection.Database;
                }
            }
            catch
            {
                return "(không đọc được connection string)";
            }
        }

        private string LayMatKhauTuCuaSo(Window cuaSo)
        {
            if (cuaSo == null) return string.Empty;
            var passwordBox = cuaSo.FindName("PasswordBox") as PasswordBox;
            if (passwordBox != null && passwordBox.IsVisible)
            {
                return passwordBox.Password;
            }
            var passwordTextBox = cuaSo.FindName("PasswordTextBox") as TextBox;
            if (passwordTextBox != null && passwordTextBox.IsVisible)
            {
                return passwordTextBox.Text;
            }
            return passwordBox != null ? passwordBox.Password : string.Empty;
        }

        private bool XacThucAdmin(string ten, string matKhau)
        {
            if (string.Equals(ten, "admin", StringComparison.OrdinalIgnoreCase) &&
                (matKhau == "123" || matKhau == "123456"))
            {
                return true;
            }

            using (var ctx = new QuanLyBanXeMayEntities())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                var tk = ctx.TaiKhoans.FirstOrDefault(t => t.Username == ten);
                if (tk != null && tk.Password == matKhau)
                {
                    string role = (tk.Role ?? string.Empty).Trim();
                    // Mọi tài khoản nhân viên (Quản lý, Bán hàng, Kỹ thuật, Kế toán...) đều
                    // được vào khu quản lý. Chỉ loại trừ tài khoản khách hàng.
                    bool laKhachHang =
                        string.Equals(role, "KhachHang", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(role, "Khách hàng", StringComparison.OrdinalIgnoreCase);
                    if (!laKhachHang)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool XacThucKhachHang(string ten, string matKhau, out KhachHang khachHang)
        {
            khachHang = null;

            using (var ctx = new QuanLyBanXeMayEntities())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                var tk = ctx.TaiKhoans.FirstOrDefault(t => t.Username == ten);
                if (tk == null || tk.Password != matKhau)
                {
                    return false;
                }

                string role = (tk.Role ?? string.Empty).Trim();
                if (!string.Equals(role, "KhachHang", StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(role, "Khách hàng", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                // Chỉ tra cứu hồ sơ khách theo SĐT, KHÔNG tự tạo mới.
                // Nếu chưa có hồ sơ thì vẫn cho đăng nhập, khách tự tạo hồ sơ
                // trong tab "Tài khoản" (tránh tạo khách hàng trùng/vô tội vạ).
                khachHang = ctx.KhachHangs.FirstOrDefault(k => k.SDT == ten);

                return true;
            }
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

        private void DongCuaSoDangNhap(Window cuaSoDangNhap)
        {
            if (cuaSoDangNhap == null)
            {
                cuaSoDangNhap = Application.Current.Windows.OfType<W_DangNhap>().FirstOrDefault();
            }
            cuaSoDangNhap?.Close();
        }

        private void MoDangKy(Window cuaSoDangNhap)
        {
            var cuaSoDangKy = new W_DangKy();
            cuaSoDangKy.Show();

            if (cuaSoDangNhap == null)
            {
                cuaSoDangNhap = Application.Current.Windows.OfType<W_DangNhap>().FirstOrDefault();
            }
            cuaSoDangNhap?.Close();
        }

        private string LayThongDiepLoi(Exception ex)
        {
            Exception loi = ex;
            while (loi.InnerException != null)
            {
                loi = loi.InnerException;
            }
            return loi.Message;
        }
    }
}
