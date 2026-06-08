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
    // Đăng nhập chỉ dành cho NHÂN VIÊN / ADMIN. Khách hàng vào thẳng cổng khách,
    // không cần đăng nhập (xem MainWindow_User).
    public class DangNhap_VM : BaseViewModel
    {
        private string tenDangNhap;
        public string TenDangNhap
        {
            get { return tenDangNhap; }
            set { tenDangNhap = value; OnPropertyChanged(); }
        }

        // Loại tài khoản đang chọn ở màn đăng nhập: false = Nhân viên, true = Quản trị (Admin).
        private bool laAdmin;
        public bool LaAdmin
        {
            get { return laAdmin; }
            set { laAdmin = value; OnPropertyChanged(); }
        }

        public ICommand LenhDangNhap { get; }
        public ICommand LenhQuenMatKhau { get; }
        public ICommand LenhChonNhanVien { get; }
        public ICommand LenhChonAdmin { get; }

        public DangNhap_VM()
        {
            LenhDangNhap = new RelayCommand(thamSo => DangNhap(thamSo as Window));
            LenhQuenMatKhau = new RelayCommand(thamSo => MoQuenMatKhau(thamSo as Window));
            LenhChonNhanVien = new RelayCommand(_ => ChonLoai(false));
            LenhChonAdmin = new RelayCommand(_ => ChonLoai(true));
        }

        // Chọn loại tài khoản. Khi chọn Admin sẽ điền sẵn 'admin' cho tiện;
        // việc phân quyền thực tế vẫn dựa trên vai trò trong CSDL.
        private void ChonLoai(bool admin)
        {
            LaAdmin = admin;
            if (admin)
            {
                if (string.IsNullOrWhiteSpace(TenDangNhap))
                {
                    TenDangNhap = "admin";
                }
            }
            else
            {
                if (string.Equals(TenDangNhap, "admin", StringComparison.OrdinalIgnoreCase))
                {
                    TenDangNhap = string.Empty;
                }
            }
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
                    string role;
                    string thongBao;
                    NhanVien nhanVien;
                    if (!XacThucNhanVien(ten, matKhau, out role, out thongBao, out nhanVien))
                    {
                        Mouse.OverrideCursor = null;
                        MessageBox.Show(thongBao, "Đăng nhập thất bại", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    PhienDangNhap.TenDangNhap = ten;
                    PhienDangNhap.Role = role;
                    PhienDangNhap.NhanVienHienTai = nhanVien;
                    PhienDangNhap.KhachHangHienTai = null;
                    PhienDangNhap.GioHangKhach.Clear();

                    Mouse.OverrideCursor = null;
                    var khuQuanTri = new MainWindow();
                    khuQuanTri.Show();

                    // Đóng cửa sổ đăng nhập và cổng khách (nếu đang mở).
                    DongCuaSoDangNhap(cuaSoDangNhap);
                    DongCongKhach();
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
                    "\n\nKiểm tra SQL Server và database 'DL_OTO'." +
                    "\nConnection string đang trỏ tới: " + LayConnectionInfo(),
                    "Lỗi đăng nhập", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Xác thực tài khoản nhân viên/admin. Trả về vai trò + nhân viên gắn với tài khoản.
        private bool XacThucNhanVien(string ten, string matKhau, out string role, out string thongBao, out NhanVien nhanVien)
        {
            role = null;
            thongBao = "Sai tên đăng nhập hoặc mật khẩu.";
            nhanVien = null;

            // Tài khoản admin mặc định (tiện dụng khi chưa có CSDL hoàn chỉnh).
            if (string.Equals(ten, "admin", StringComparison.OrdinalIgnoreCase) &&
                (matKhau == "123" || matKhau == "123456"))
            {
                role = "Quản lý";
            }

            using (var ctx = new QuanLyBanXeMayEntities())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                var tk = ctx.TaiKhoans.FirstOrDefault(t => t.Username == ten);

                if (role == null)
                {
                    if (tk == null || tk.Password != matKhau)
                    {
                        return false;
                    }

                    string r = (tk.Role ?? string.Empty).Trim();
                    bool laKhachHang =
                        string.Equals(r, "KhachHang", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(r, "Khách hàng", StringComparison.OrdinalIgnoreCase);
                    if (laKhachHang)
                    {
                        thongBao = "Tài khoản này không phải nhân viên.";
                        return false;
                    }

                    if (string.Equals((tk.TrangThai ?? string.Empty).Trim(), "Đã khóa", StringComparison.OrdinalIgnoreCase))
                    {
                        thongBao = "Tài khoản đã bị khóa. Vui lòng liên hệ quản lý.";
                        return false;
                    }

                    if (QuyenHan.CamDangNhap(r))
                    {
                        thongBao = "Chức vụ này không sử dụng hệ thống nên không được đăng nhập.";
                        return false;
                    }

                    role = string.IsNullOrWhiteSpace(r) ? "Nhân viên" : r;
                }

                if (tk != null && !string.IsNullOrWhiteSpace(tk.MaNV))
                {
                    nhanVien = ctx.NhanViens.FirstOrDefault(nv => nv.MaNV == tk.MaNV);
                }
            }
            return true;
        }

        private void MoQuenMatKhau(Window cuaSoDangNhap)
        {
            var cuaSo = new W_QuenMatKhau();
            cuaSo.Owner = cuaSoDangNhap;
            cuaSo.ShowDialog();
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

        private void DongCuaSoDangNhap(Window cuaSoDangNhap)
        {
            if (cuaSoDangNhap == null)
            {
                cuaSoDangNhap = Application.Current.Windows.OfType<W_DangNhap>().FirstOrDefault();
            }
            cuaSoDangNhap?.Close();
        }

        private void DongCongKhach()
        {
            var congKhach = Application.Current.Windows.OfType<MainWindow_User>().FirstOrDefault();
            congKhach?.Close();
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
