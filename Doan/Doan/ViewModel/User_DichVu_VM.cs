using Doan.Helper;
using Doan.Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Doan.ViewModel
{
    public class User_DichVu_VM : BaseViewModel
    {
        private ObservableCollection<DichVuPhuTung> danhSachDichVu;
        public ObservableCollection<DichVuPhuTung> DanhSachDichVu
        {
            get { return danhSachDichVu; }
            set
            {
                danhSachDichVu = value;
                OnPropertyChanged();
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

        public ICommand LenhTimKiem { get; }
        public ICommand LenhThemVaoGio { get; }

        public User_DichVu_VM()
        {
            LenhTimKiem = new RelayCommand(_ => TaiDanhSach());
            LenhThemVaoGio = new RelayCommand(parameter => ThemVaoGio(parameter as DichVuPhuTung),
                parameter => parameter is DichVuPhuTung);
            TaiDanhSach();
        }

        private void TaiDanhSach()
        {
            using (var ctx = new QuanLyBanXeMayEntities())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                var ds = ctx.DichVuPhuTungs.ToList();
                if (!string.IsNullOrWhiteSpace(TuKhoaTimKiem))
                {
                    string tk = TuKhoaTimKiem.Trim().ToLower();
                    ds = ds.Where(d => (d.Ten ?? string.Empty).ToLower().Contains(tk)).ToList();
                }
                DanhSachDichVu = new ObservableCollection<DichVuPhuTung>(ds);
            }
        }

        private void ThemVaoGio(DichVuPhuTung dichVu)
        {
            if (dichVu == null)
            {
                return;
            }

            int tonKho = dichVu.TonKho ?? 0;
            bool laPhuTung = (dichVu.MaPT ?? string.Empty).Trim().ToUpper().StartsWith("PT");
            if (laPhuTung && tonKho <= 0)
            {
                MessageBox.Show("Phụ tùng \"" + dichVu.Ten + "\" hiện đã hết hàng.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var gio = PhienDangNhap.GioHangKhach;
            var cu = gio.FirstOrDefault(item =>
                string.Equals(item.MaMatHang, dichVu.MaPT, StringComparison.OrdinalIgnoreCase));

            if (cu != null)
            {
                if (laPhuTung && cu.SoLuong + 1 > tonKho)
                {
                    MessageBox.Show("Vượt quá tồn kho.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                cu.SoLuong = cu.SoLuong + 1;
            }
            else
            {
                gio.Add(new MatHangGio_VM
                {
                    MaMatHang = dichVu.MaPT,
                    TenMatHang = dichVu.Ten,
                    DonGia = (long)(dichVu.Gia ?? 0),
                    SoLuong = 1
                });
            }

            MessageBox.Show("Đã thêm \"" + dichVu.Ten + "\" vào giỏ hàng.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
