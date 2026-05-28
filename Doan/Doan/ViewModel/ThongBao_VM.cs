using Doan.Helper;
using Doan.Model;
using Doan.View;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Doan.ViewModel
{
    // Tab Thông báo: hiển thị các đơn khách đặt online đang "Chờ xác nhận".
    public class ThongBao_VM : BaseViewModel
    {
        private ObservableCollection<DonChoXuLy_VM> danhSachDonCho;
        public ObservableCollection<DonChoXuLy_VM> DanhSachDonCho
        {
            get { return danhSachDonCho; }
            set { danhSachDonCho = value; OnPropertyChanged(); }
        }

        public int SoDonCho
        {
            get { return DanhSachDonCho != null ? DanhSachDonCho.Count : 0; }
        }

        public bool CoDonCho
        {
            get { return SoDonCho > 0; }
        }

        public ICommand LenhLamMoi { get; }
        public ICommand LenhXuLyDon { get; }

        public ThongBao_VM()
        {
            DanhSachDonCho = new ObservableCollection<DonChoXuLy_VM>();
            LenhLamMoi = new RelayCommand(_ => TaiDanhSachDonCho());
            LenhXuLyDon = new RelayCommand(thamSo => XuLyDon(thamSo as DonChoXuLy_VM));
            TaiDanhSachDonCho();
        }

        private void TaiDanhSachDonCho()
        {
            using (var ctx = new QuanLyBanXeMayEntities())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                var ds = ctx.HoaDons
                    .Include("KhachHang")
                    .Where(item => item.TrangThai == "Chờ xác nhận")
                    .ToList();

                // Gom theo (MaKH, NgayLap): các dòng cùng một lần đặt chia sẻ chung NgayLap.
                var nhom = ds
                    .GroupBy(item => new { item.MaKH, item.NgayLap })
                    .OrderByDescending(g => g.Key.NgayLap)
                    .Select(g => new DonChoXuLy_VM
                    {
                        MaKH = g.Key.MaKH,
                        TenKhachHang = g.Select(x => x.KhachHang != null ? x.KhachHang.HoTen : null).FirstOrDefault(),
                        SDT = g.Select(x => x.KhachHang != null ? x.KhachHang.SDT : null).FirstOrDefault(),
                        DiaChi = g.Select(x => x.KhachHang != null ? x.KhachHang.DiaChi : null).FirstOrDefault(),
                        NgayLap = g.Key.NgayLap,
                        MoTaMatHang = string.Join(", ", g.Select(x => x.TenDV_SP + " (x" + (x.SoLuong ?? 0) + ")")),
                        SoMatHang = g.Count(),
                        TongTien = g.Sum(x => x.ThanhTien ?? 0),
                        DanhSachMaHD = g.Select(x => x.MaHD).ToList()
                    })
                    .ToList();

                DanhSachDonCho = new ObservableCollection<DonChoXuLy_VM>(nhom);
                OnPropertyChanged(nameof(SoDonCho));
                OnPropertyChanged(nameof(CoDonCho));
            }
        }

        private void XuLyDon(DonChoXuLy_VM don)
        {
            if (don == null)
            {
                return;
            }

            var cuaSo = new W_XuLyDon(don);
            cuaSo.Owner = Application.Current.MainWindow;
            bool? ketQua = cuaSo.ShowDialog();

            if (ketQua == true)
            {
                // Đơn đã được xác nhận -> refresh danh sách và chuyển qua tab Lịch sử.
                TaiDanhSachDonCho();

                var mainVM = Application.Current.MainWindow != null
                    ? Application.Current.MainWindow.DataContext as MainWindows_VM
                    : null;
                if (mainVM != null)
                {
                    mainVM.DieuHuong("LichSu", null);
                }
            }
        }
    }
}
