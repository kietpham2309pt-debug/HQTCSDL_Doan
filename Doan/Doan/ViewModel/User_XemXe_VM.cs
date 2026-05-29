using Doan.Helper;
using Doan.Model;
using Doan.View;
using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Doan.ViewModel
{
    public class User_XemXe_VM : BaseViewModel
    {
        private ObservableCollection<Xe> danhSachXe;
        public ObservableCollection<Xe> DanhSachXe
        {
            get { return danhSachXe; }
            set
            {
                danhSachXe = value;
                OnPropertyChanged();
            }
        }

        private Xe xeDangChon;
        public Xe XeDangChon
        {
            get { return xeDangChon; }
            set
            {
                xeDangChon = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CoXeDuocChon));
            }
        }

        public bool CoXeDuocChon
        {
            get { return XeDangChon != null; }
        }

        private ObservableCollection<HangXe> danhSachHangLoc;
        public ObservableCollection<HangXe> DanhSachHangLoc
        {
            get { return danhSachHangLoc; }
            set
            {
                danhSachHangLoc = value;
                OnPropertyChanged();
            }
        }

        private HangXe hangLocDangChon;
        public HangXe HangLocDangChon
        {
            get { return hangLocDangChon; }
            set
            {
                hangLocDangChon = value;
                OnPropertyChanged();
                TaiDanhSachXe();
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

        public User_XemXe_VM() : this(null)
        {
        }

        // maHangLoc: nếu khác null sẽ lọc sẵn theo hãng (khi khách bấm hãng ở Trang chủ).
        public User_XemXe_VM(string maHangLoc)
        {
            LenhTimKiem = new RelayCommand(_ => TaiDanhSachXe());
            LenhThemVaoGio = new RelayCommand(_ => ThemVaoGio());
            TaiHangLoc();

            if (!string.IsNullOrWhiteSpace(maHangLoc))
            {
                var hang = DanhSachHangLoc.FirstOrDefault(h => h.MaHang == maHangLoc);
                if (hang != null)
                {
                    // Gán setter sẽ tự gọi TaiDanhSachXe() với bộ lọc theo hãng.
                    HangLocDangChon = hang;
                }
            }
        }

        private void TaiHangLoc()
        {
            using (var ctx = new QuanLyBanXeMayEntities())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                var ds = new ObservableCollection<HangXe>();
                ds.Add(new HangXe { MaHang = "ALL", TenHang = "Tất cả các hãng" });
                foreach (var item in ctx.HangXes.ToList())
                {
                    ds.Add(item);
                }
                DanhSachHangLoc = ds;
                HangLocDangChon = ds.FirstOrDefault();
            }
        }

        private void TaiDanhSachXe()
        {
            using (var ctx = new QuanLyBanXeMayEntities())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                var query = ctx.Xes.Include("HangXe").AsQueryable();

                if (HangLocDangChon != null && HangLocDangChon.MaHang != "ALL")
                {
                    string maHang = HangLocDangChon.MaHang;
                    query = query.Where(x => x.MaHang == maHang);
                }

                var ds = query.ToList();

                if (!string.IsNullOrWhiteSpace(TuKhoaTimKiem))
                {
                    string tk = TuKhoaTimKiem.Trim().ToLower();
                    ds = ds.Where(x =>
                        (x.TenXe ?? string.Empty).ToLower().Contains(tk) ||
                        (x.LoaiXe ?? string.Empty).ToLower().Contains(tk) ||
                        (x.MauSac ?? string.Empty).ToLower().Contains(tk)
                    ).ToList();
                }

                DanhSachXe = new ObservableCollection<Xe>(ds);
            }
        }

        private void ThemVaoGio()
        {
            if (XeDangChon == null)
            {
                MessageBox.Show("Vui lòng chọn một xe để mua.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if ((XeDangChon.SoLuongTon ?? 0) <= 0)
            {
                MessageBox.Show("Xe này hiện đã hết hàng.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var gio = PhienDangNhap.GioHangKhach;
            var matHangCu = gio.FirstOrDefault(item =>
                string.Equals(item.MaMatHang, XeDangChon.MaXe, StringComparison.OrdinalIgnoreCase));

            if (matHangCu != null)
            {
                if (matHangCu.SoLuong + 1 > (XeDangChon.SoLuongTon ?? 0))
                {
                    MessageBox.Show("Vượt quá số lượng tồn kho.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                matHangCu.SoLuong = matHangCu.SoLuong + 1;
            }
            else
            {
                var matHangMoi = new MatHangGio_VM
                {
                    MaMatHang = XeDangChon.MaXe,
                    TenMatHang = XeDangChon.TenXe,
                    DonGia = (long)(XeDangChon.GiaBan ?? 0),
                    SoLuong = 1
                };
                gio.Add(matHangMoi);
            }

            MessageBox.Show("Đã thêm \"" + XeDangChon.TenXe + "\" vào giỏ hàng.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
