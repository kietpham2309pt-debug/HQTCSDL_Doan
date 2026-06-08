using Doan.Model;
using Doan.ViewModel;
using System.Linq;
using System.Windows;

namespace Doan.View
{
    /// <summary>
    /// Interaction logic for W_XuLyDon.xaml
    /// </summary>
    public partial class W_XuLyDon : Window
    {
        private readonly DonChoXuLy_VM don;

        public W_XuLyDon(DonChoXuLy_VM don)
        {
            InitializeComponent();
            this.don = don;

            KhachTextBlock.Text = don.TenKhachHang;
            SdtTextBlock.Text = "SĐT: " + don.SDT;
            DiaChiTextBlock.Text = "Địa chỉ: " + don.DiaChi;
            MatHangTextBlock.Text = don.MoTaMatHang;
            TongTienTextBlock.Text = string.Format("Tổng tiền: {0:N0} đ", don.TongTien);

            TaiDanhSachNhanVien();
        }

        private void TaiDanhSachNhanVien()
        {
            using (var ctx = new QuanLyBanXeMayEntities())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                var ds = ctx.NhanViens
                    .Where(nv => nv.TrangThai == "Đang làm việc")
                    .OrderBy(nv => nv.HoTen)
                    .ToList();
                NhanVienComboBox.ItemsSource = ds;
                NhanVienComboBox.SelectedItem = ds.FirstOrDefault();
            }
        }

        private void Huy_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void XacNhan_Click(object sender, RoutedEventArgs e)
        {
            var nv = NhanVienComboBox.SelectedItem as NhanVien;
            if (nv == null)
            {
                MessageBox.Show("Vui lòng chọn nhân viên phụ trách.", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var ctx = new QuanLyBanXeMayEntities())
                {
                    // Gọi STORED PROCEDURE sp_XacNhanDonChoXuLy của CSDL để
                    // xác nhận các đơn 'Chờ xác nhận' của khách và gán nhân viên phụ trách.
                    ctx.Database.ExecuteSqlCommand(
                        "EXEC sp_XacNhanDonChoXuLy {0}, {1}", don.MaKH, nv.MaNV);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Xử lý đơn thất bại: " + ex.Message, "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MessageBox.Show("Đã xác nhận đơn. Khách sẽ thấy đơn 'Đã xác nhận' và được mời đến cơ sở nhận xe.",
                "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            DialogResult = true;
            Close();
        }
    }
}
