using System;
using System.Linq;
using System.Windows;
using Doan.Model;

namespace Doan.View
{
    public partial class W_QuenMatKhau : Window
    {
        private string usernameDaXacNhan;

        public W_QuenMatKhau()
        {
            InitializeComponent();
        }

        private void BtnLayCauHoi_Click(object sender, RoutedEventArgs e)
        {
            string username = (UsernameBox.Text ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Vui lòng nhập tên đăng nhập.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var ctx = new QuanLyBanXeMayEntities())
                {
                    ctx.Configuration.LazyLoadingEnabled = false;
                    var tk = ctx.TaiKhoans.FirstOrDefault(t => t.Username == username);
                    if (tk == null)
                    {
                        CauHoiText.Text = "(Không tìm thấy tài khoản)";
                        usernameDaXacNhan = null;
                        return;
                    }
                    if (string.IsNullOrWhiteSpace(tk.CauHoiBaoMat))
                    {
                        CauHoiText.Text = "(Tài khoản chưa thiết lập câu hỏi bảo mật. Vui lòng nhờ quản lý cấp lại mật khẩu.)";
                        usernameDaXacNhan = null;
                        return;
                    }
                    CauHoiText.Text = tk.CauHoiBaoMat;
                    usernameDaXacNhan = username;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải câu hỏi: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnDatLai_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(usernameDaXacNhan))
            {
                MessageBox.Show("Vui lòng bấm 'Lấy câu hỏi' và nhập đúng tên đăng nhập trước.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string traLoi = (TraLoiBox.Text ?? string.Empty).Trim();
            string matKhauMoi = MatKhauMoiBox.Password ?? string.Empty;
            string xacNhan = XacNhanBox.Password ?? string.Empty;

            if (string.IsNullOrWhiteSpace(traLoi))
            {
                MessageBox.Show("Vui lòng nhập câu trả lời bảo mật.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (matKhauMoi.Length < 3)
            {
                MessageBox.Show("Mật khẩu mới phải có ít nhất 3 ký tự.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (matKhauMoi != xacNhan)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var ctx = new QuanLyBanXeMayEntities())
                {
                    ctx.Configuration.LazyLoadingEnabled = false;
                    var tk = ctx.TaiKhoans.FirstOrDefault(t => t.Username == usernameDaXacNhan);
                    if (tk == null)
                    {
                        MessageBox.Show("Không tìm thấy tài khoản.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    string dapAnDung = (tk.CauTraLoiBaoMat ?? string.Empty).Trim();
                    if (!string.Equals(dapAnDung, traLoi, StringComparison.OrdinalIgnoreCase))
                    {
                        MessageBox.Show("Câu trả lời bảo mật không đúng.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    tk.Password = matKhauMoi;
                    ctx.SaveChanges();
                }

                MessageBox.Show("Đặt lại mật khẩu thành công. Vui lòng đăng nhập lại.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi đặt lại mật khẩu: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
