using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Doan.Helper;
using Doan.Model;
using Doan.View;

namespace Doan.ViewModel
{
    public class MainWindows_VM : BaseViewModel
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

        public ICommand LenhDieuHuong { get; }

        public MainWindows_VM()
        {
            LenhDieuHuong = new RelayCommand(thamSo => DieuHuong(thamSo?.ToString(), null));
            // Mở màn hình mặc định hợp lệ theo vai trò đang đăng nhập.
            DieuHuong(QuyenHan.TabMacDinh(PhienDangNhap.Role), null);
        }

        public void DieuHuong(string tenManHinh, object duLieu = null)
        {
            if (tenManHinh == "DanhSachXeTheoHang")
            {
                var hangXeDuocChon = duLieu as HangXe;
                ManHinhHienTai = new UC_DSXe(hangXeDuocChon);
                return;
            }

            switch (tenManHinh)
            {
                case "QuanLyXe":
                    ManHinhHienTai = new UC_DSHangXe();
                    break;
                case "TatCaXe":
                    ManHinhHienTai = new UC_DSXe();
                    break;
                case "KhachHang":
                    ManHinhHienTai = new UC_KhachHang();
                    break;
                case "NhanVien":
                    ManHinhHienTai = new UC_NhanVien();
                    break;
                case "DichVu":
                    ManHinhHienTai = new UC_DichVu("Dịch vụ");
                    break;
                case "PhuTung":
                    ManHinhHienTai = new UC_DichVu("Phụ tùng");
                    break;
                case "NhapKho":
                    ManHinhHienTai = new UC_NhapKho();
                    break;
                case "LichSuNhap":
                    ManHinhHienTai = new UC_LichSuNhap();
                    break;
                case "ThanhToan":
                    ManHinhHienTai = new UC_ThanhToan();
                    break;
                case "TraGop":
                    ManHinhHienTai = new UC_TheoDoiTraGop();
                    break;
                case "LichSu":
                case "DonHang":
                    ManHinhHienTai = new UC_HoaDon();
                    break;
                case "ThongBao":
                    ManHinhHienTai = new UC_ThongBao();
                    break;
                case "ThongKe":
                    ManHinhHienTai = new UC_ThongKe();
                    break;
                case "CaiDat":
                    ManHinhHienTai = new UC_CaiDat();
                    break;
                case "DangXuat":
                    // Đăng xuất nhân viên -> quay về cổng khách (trang chủ của app).
                    var congKhach = new MainWindow_User();
                    congKhach.Show();

                    var cuaSoChinh = Application.Current.Windows
                        .OfType<Window>()
                        .FirstOrDefault(window => window is MainWindow);
                    cuaSoChinh?.Close();
                    break;
                default:
                    ManHinhHienTai = new UC_DSHangXe();
                    break;
            }
        }
    }
}
