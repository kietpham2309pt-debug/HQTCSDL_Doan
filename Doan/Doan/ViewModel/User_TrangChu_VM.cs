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
    public class User_TrangChu_VM : BaseViewModel
    {
        private ObservableCollection<HangXe> danhSachHangXe;
        public ObservableCollection<HangXe> DanhSachHangXe
        {
            get { return danhSachHangXe; }
            set
            {
                danhSachHangXe = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Xe> danhSachXeNoiBat;
        public ObservableCollection<Xe> DanhSachXeNoiBat
        {
            get { return danhSachXeNoiBat; }
            set
            {
                danhSachXeNoiBat = value;
                OnPropertyChanged();
            }
        }

        public string LoiChao
        {
            get
            {
                if (PhienDangNhap.KhachHangHienTai != null && !string.IsNullOrWhiteSpace(PhienDangNhap.KhachHangHienTai.HoTen))
                {
                    return "Xin chào, " + PhienDangNhap.KhachHangHienTai.HoTen + "!";
                }
                return "Xin chào, Quý khách!";
            }
        }

        public ICommand LenhMoMuaXe { get; }
        public ICommand LenhMoDichVu { get; }
        public ICommand LenhChonHang { get; }

        public User_TrangChu_VM()
        {
            LenhMoMuaXe = new RelayCommand(_ => ChuyenManHinh("XemXe"));
            LenhMoDichVu = new RelayCommand(_ => ChuyenManHinh("DichVu"));
            LenhChonHang = new RelayCommand(thamSo => ChonHang(thamSo as HangXe));
            TaiDuLieu();
        }

        private void ChonHang(HangXe hang)
        {
            if (hang == null)
            {
                return;
            }
            var cuaSoUser = Application.Current.Windows.OfType<MainWindow_User>().FirstOrDefault();
            var vm = cuaSoUser?.DataContext as MainWindow_User_VM;
            vm?.DieuHuong("XemXe", hang);
        }

        private void TaiDuLieu()
        {
            using (var ctx = new QuanLyBanXeMayEntities())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                DanhSachHangXe = new ObservableCollection<HangXe>(ctx.HangXes.ToList());

                var topXe = ctx.Xes.Include("HangXe")
                    .OrderByDescending(x => x.GiaBan)
                    .Take(6)
                    .ToList();
                DanhSachXeNoiBat = new ObservableCollection<Xe>(topXe);
            }
        }

        private void ChuyenManHinh(string ten)
        {
            var cuaSoUser = Application.Current.Windows.OfType<MainWindow_User>().FirstOrDefault();
            var vm = cuaSoUser?.DataContext as MainWindow_User_VM;
            vm?.DieuHuong(ten);
        }
    }
}
