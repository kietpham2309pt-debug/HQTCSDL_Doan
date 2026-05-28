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

        public MainWindow_User_VM()
        {
            LenhDieuHuong = new RelayCommand(thamSo => DieuHuong(thamSo?.ToString()));
            DieuHuong("TrangChu");
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
                case "DangXuat":
                    var cuaSoDangNhap = new W_DangNhap();
                    cuaSoDangNhap.Show();
                    PhienDangNhap.KhachHangHienTai = null;
                    PhienDangNhap.TenDangNhap = null;
                    PhienDangNhap.Role = null;
                    PhienDangNhap.GioHangKhach.Clear();

                    var cuaSoUser = Application.Current.Windows
                        .OfType<Window>()
                        .FirstOrDefault(window => window is MainWindow_User);
                    cuaSoUser?.Close();
                    break;
                default:
                    ManHinhHienTai = new UC_User_TrangChu();
                    break;
            }
        }
    }
}
