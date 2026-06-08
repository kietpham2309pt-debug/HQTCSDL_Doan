using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Doan.Helper;
using Doan.Model;

namespace Doan.View
{
    // Form thu thập đầy đủ thông tin khách hàng khi đặt mua (khách vãng lai không cần tài khoản).
    // Khi xác nhận: tìm/tạo KhachHang theo SĐT và gán vào phiên hiện tại.
    public partial class W_ThongTinKhachHang : Window
    {
        public KhachHang KhachHangKetQua { get; private set; }

        public W_ThongTinKhachHang()
        {
            InitializeComponent();
        }

        private void BtnHuy_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void BtnXacNhan_Click(object sender, RoutedEventArgs e)
        {
            string hoTen = (HoTenBox.Text ?? string.Empty).Trim();
            string sdt = (SdtBox.Text ?? string.Empty).Trim();
            string cccd = (CccdBox.Text ?? string.Empty).Trim();
            string email = (EmailBox.Text ?? string.Empty).Trim();
            string diaChi = (DiaChiBox.Text ?? string.Empty).Trim();
            string gioiTinh = (GioiTinhBox.SelectedValue as string) ?? "Nam";
            DateTime? ngaySinh = NgaySinhPicker.SelectedDate;

            if (hoTen.Length < 2)
            {
                MessageBox.Show("Vui lòng nhập họ tên (ít nhất 2 ký tự).", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!LaChuoiSo(sdt, 10, 11))
            {
                MessageBox.Show("Số điện thoại phải là 10 đến 11 chữ số.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!LaChuoiSo(cccd, 9, 12))
            {
                MessageBox.Show("CCCD phải là 9 đến 12 chữ số.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(diaChi))
            {
                MessageBox.Show("Vui lòng nhập địa chỉ.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var ctx = new QuanLyBanXeMayEntities())
                {
                    ctx.Configuration.LazyLoadingEnabled = false;

                    // Đã có khách theo SĐT -> cập nhật thông tin, dùng lại hồ sơ.
                    var kh = ctx.KhachHangs.FirstOrDefault(k => k.SDT == sdt);

                    // CCCD trùng người khác?
                    var trungCccd = ctx.KhachHangs.FirstOrDefault(k => k.CCCD == cccd && k.SDT != sdt);
                    if (trungCccd != null)
                    {
                        MessageBox.Show("CCCD này đã được dùng cho khách hàng khác.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    if (kh == null)
                    {
                        kh = new KhachHang { MaKH = TaoMaKhachHangMoi(ctx) };
                        ctx.KhachHangs.Add(kh);
                    }

                    kh.HoTen = hoTen;
                    kh.SDT = sdt;
                    kh.CCCD = cccd;
                    kh.Email = string.IsNullOrWhiteSpace(email) ? null : email;
                    kh.DiaChi = diaChi;
                    kh.NgaySinh = ngaySinh;
                    kh.GioiTinh = gioiTinh;

                    ctx.SaveChanges();

                    // Tách khỏi context để dùng an toàn ở phiên.
                    KhachHangKetQua = new KhachHang
                    {
                        MaKH = kh.MaKH,
                        HoTen = kh.HoTen,
                        SDT = kh.SDT,
                        CCCD = kh.CCCD,
                        Email = kh.Email,
                        DiaChi = kh.DiaChi,
                        NgaySinh = kh.NgaySinh,
                        GioiTinh = kh.GioiTinh
                    };
                }

                PhienDangNhap.KhachHangHienTai = KhachHangKetQua;
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lưu thông tin thất bại: " + LayLoi(ex), "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static string LayLoi(Exception ex)
        {
            Exception loi = ex;
            while (loi.InnerException != null) loi = loi.InnerException;
            return loi.Message;
        }

        private static bool LaChuoiSo(string s, int min, int max)
        {
            if (string.IsNullOrWhiteSpace(s)) return false;
            s = s.Trim();
            if (s.Length < min || s.Length > max) return false;
            return s.All(c => c >= '0' && c <= '9');
        }

        private static string TaoMaKhachHangMoi(QuanLyBanXeMayEntities ctx)
        {
            int maxSo = 0;
            foreach (string ma in ctx.KhachHangs.Select(k => k.MaKH).ToList())
            {
                if (string.IsNullOrWhiteSpace(ma)) continue;
                string so = ma.Trim().ToUpper().Replace("KH", string.Empty);
                int n;
                if (int.TryParse(so, out n) && n > maxSo) maxSo = n;
            }
            return "KH" + (maxSo + 1).ToString("000");
        }
    }
}
