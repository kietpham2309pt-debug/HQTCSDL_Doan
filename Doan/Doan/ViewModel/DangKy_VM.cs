using Doan.Helper;
using Doan.Model;
using Doan.View;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Doan.ViewModel
{
    public class DangKy_VM : BaseViewModel
    {
        public ICommand LenhDangKy { get; }
        public ICommand LenhMoDangNhap { get; }

        public DangKy_VM()
        {
            LenhDangKy = new RelayCommand(thamSo => DangKy(thamSo as Window));
            LenhMoDangNhap = new RelayCommand(thamSo => MoDangNhap(thamSo as Window));
        }

        private void DangKy(Window cuaSoDangKy)
        {
            if (cuaSoDangKy == null)
            {
                return;
            }

            string hoTen = (LayTextBox(cuaSoDangKy, "FullNameTextBox")?.Text ?? string.Empty).Trim();
            string username = (LayTextBox(cuaSoDangKy, "RegUsernameTextBox")?.Text ?? string.Empty).Trim();
            string matKhau = LayPasswordBox(cuaSoDangKy, "RegPasswordBox")?.Password ?? string.Empty;
            string xacNhan = LayPasswordBox(cuaSoDangKy, "ConfirmPasswordBox")?.Password ?? string.Empty;

            if (string.IsNullOrWhiteSpace(hoTen))
            {
                MessageBox.Show("Vui lòng nhập họ và tên.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!LaSoDienThoaiHopLe(username))
            {
                MessageBox.Show("Tên đăng nhập phải là số điện thoại (10 đến 11 chữ số). Số này dùng để đăng nhập và lập hóa đơn.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(matKhau) || matKhau.Length < 3)
            {
                MessageBox.Show("Mật khẩu phải có ít nhất 3 ký tự.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (matKhau != xacNhan)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var ctx = new QuanLyBanXeMayEntities())
                {
                    ctx.Configuration.LazyLoadingEnabled = false;

                    var tkTrung = ctx.TaiKhoans.FirstOrDefault(t => t.Username == username);
                    if (tkTrung != null)
                    {
                        MessageBox.Show("Tên đăng nhập đã tồn tại. Vui lòng chọn tên khác.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    string maKH = TaoMaKhachHangMoi(ctx);
                    var khachHangMoi = new KhachHang
                    {
                        MaKH = maKH,
                        HoTen = hoTen,
                        SDT = username,
                        CCCD = string.Empty,
                        Email = string.Empty,
                        DiaChi = string.Empty
                    };
                    ctx.KhachHangs.Add(khachHangMoi);

                    var taiKhoanMoi = new TaiKhoan
                    {
                        Username = username,
                        Password = matKhau,
                        Role = "KhachHang",
                        MaNV = null
                    };
                    ctx.TaiKhoans.Add(taiKhoanMoi);

                    ctx.SaveChanges();
                }

                MessageBox.Show("Đăng ký thành công! Vui lòng đăng nhập.", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đăng ký thất bại: " + LayThongDiepLoi(ex), "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var cuaSoDangNhap = new W_DangNhap();
            cuaSoDangNhap.Show();
            cuaSoDangKy.Close();
        }

        private void MoDangNhap(Window cuaSoDangKy)
        {
            var cuaSoDangNhap = new W_DangNhap();
            cuaSoDangNhap.Show();

            if (cuaSoDangKy == null)
            {
                cuaSoDangKy = Application.Current.Windows.OfType<W_DangKy>().FirstOrDefault();
            }
            cuaSoDangKy?.Close();
        }

        private TextBox LayTextBox(Window cuaSo, string ten)
        {
            return cuaSo.FindName(ten) as TextBox;
        }

        private PasswordBox LayPasswordBox(Window cuaSo, string ten)
        {
            return cuaSo.FindName(ten) as PasswordBox;
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

        private bool LaSoDienThoaiHopLe(string duLieu)
        {
            if (string.IsNullOrWhiteSpace(duLieu))
            {
                return false;
            }

            string giaTri = duLieu.Trim();
            if (giaTri.Length < 10 || giaTri.Length > 11)
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
