using Doan.Helper;
using Doan.View;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Doan.ViewModel
{
    public class MainWindow_User_VM : BaseViewModel
    {
        private UserControl manHinhHienTai;
        public UserControl ManHinhHienTai
        {
            get { return manHinhHienTai; }
            set
            {
                manHinhHienTai = value;
                OnPropertyChanged();
            }
        }

        public string TenKhachHienThi
        {
            get
            {
                if (PhienDangNhap.KhachHangHienTai != null && !string.IsNullOrWhiteSpace(PhienDangNhap.KhachHangHienTai.HoTen))
                {
                    return PhienDangNhap.KhachHangHienTai.HoTen;
                }
                if (!string.IsNullOrWhiteSpace(PhienDangNhap.TenDangNhap))
                {
                    return PhienDangNhap.TenDangNhap;
                }
                return "Khách";
            }
        }

        public ICommand LenhDieuHuong { get; }
        public ICommand LenhDangNhapNhanVien { get; }

        public MainWindow_User_VM()
        {
            LenhDieuHuong = new RelayCommand(thamSo => DieuHuong(thamSo?.ToString()));
            LenhDangNhapNhanVien = new RelayCommand(_ => MoDangNhapNhanVien());
            DieuHuong("TrangChu");
        }

        // Mở màn hình đăng nhập nội bộ (nhân viên/admin) mà KHÔNG đóng cổng khách.
        // Nếu đăng nhập thành công, DangNhap_VM sẽ tự mở khu quản trị và đóng cổng khách.
        private void MoDangNhapNhanVien()
        {
            var daMo = Application.Current.Windows.OfType<W_DangNhap>().FirstOrDefault();
            if (daMo != null) { daMo.Activate(); return; }

            var cuaSoDangNhap = new W_DangNhap();
            cuaSoDangNhap.Show();
        }

        public void DieuHuong(string tenManHinh)
        {
            DieuHuong(tenManHinh, null);
        }

        public void DieuHuong(string tenManHinh, object duLieu)
        {
            switch (tenManHinh)
            {
                case "TrangChu":
                    ManHinhHienTai = new UC_User_TrangChu();
                    break;
                case "XemXe":
                    var hang = duLieu as Model.HangXe;
                    ManHinhHienTai = hang != null
                        ? new UC_User_XemXe(hang.MaHang)
                        : new UC_User_XemXe();
                    break;
                case "DichVu":
                    ManHinhHienTai = new UC_User_DichVu();
                    break;
                case "GioHang":
                    ManHinhHienTai = new UC_User_GioHang();
                    break;
                case "DonHang":
                    ManHinhHienTai = new UC_User_DonHang();
                    break;
                case "TaiKhoan":
                    ManHinhHienTai = new UC_User_TaiKhoan();
                    break;
                case "DangNhapNhanVien":
                case "DangXuat":
                    MoDangNhapNhanVien();
                    break;
                default:
                    ManHinhHienTai = new UC_User_TrangChu();
                    break;
            }
        }
    }
}
