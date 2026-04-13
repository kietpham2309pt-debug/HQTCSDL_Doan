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
            LenhDieuHuong = new RelayCommand(thamSo => DieuHuong(thamSo?.ToString()));
            NavigationService.NavigateRequested += XuLyYeuCauDieuHuong;
            DieuHuong("QuanLyXe");
        }

        private void XuLyYeuCauDieuHuong(string duongDan, object duLieu)
        {
            if (duongDan == "DanhSachXeTheoHang")
            {
                var hangXeDuocChon = duLieu as HangXe;
                ManHinhHienTai = new UC_DSXe(hangXeDuocChon);
                return;
            }

            DieuHuong(duongDan);
        }

        private void DieuHuong(string tenManHinh)
        {
            switch (tenManHinh)
            {
                case "QuanLyXe":
                    ManHinhHienTai = new UC_DSHangXe();
                    break;
                case "KhachHang":
                    ManHinhHienTai = new UC_KhachHang();
                    break;
                //case "NhanVien":
                //    ManHinhHienTai = new UC_NhanVien();
                //    break;
                case "DonHang":
                    ManHinhHienTai = new UC_HoaDon();
                    break;
                //case "ThongKe":
                //    ManHinhHienTai = new UC_ThongKe();
                //    break;
                
                case "DangXuat":
                    var cuaSoDangNhap = new W_DangNhap();
                    cuaSoDangNhap.Show();

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
