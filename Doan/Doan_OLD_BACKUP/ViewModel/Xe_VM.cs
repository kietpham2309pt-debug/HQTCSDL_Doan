using Doan.Helper;
using Doan.Model;
using Doan.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Doan.ViewModel
{
    public class Xe_VM : BaseViewModel
    {
        private readonly HangXe hangXeDuocChon;
        private readonly RelayCommand lenhMoSuaXe;
        private readonly RelayCommand lenhXoaXe;
        private bool dangSuaXe;

        private ObservableCollection<Car> danhSachXe;
        public ObservableCollection<Car> DanhSachXe
        {
            get { return danhSachXe; }
            set
            {
                danhSachXe = value;
                OnPropertyChanged("DanhSachXe");
            }
        }

        private Car xeDangChon;
        public Car XeDangChon
        {
            get { return xeDangChon; }
            set
            {
                xeDangChon = value;
                OnPropertyChanged();
                if (xeDangChon != null)
                {
                    TenDongXeNhap = xeDangChon.TenDongXe;
                    LoaiXeNhap = xeDangChon.LoaiXe;
                    MauSacNhap = xeDangChon.MauSac;
                    NamSXNhap = xeDangChon.NamSX.ToString();
                    GiaXeNhap = string.Format("{0:N0}", xeDangChon.GiaXe).Replace(",", ".");
                    HinhAnhNhap = xeDangChon.HinhAnhFullPath;
                    MoTaNhap = xeDangChon.MoTa;
                    SoLuongTonNhap = xeDangChon.SoLuongTon.ToString();
                }
                lenhMoSuaXe?.RaiseCanExecuteChanged();
                lenhXoaXe?.RaiseCanExecuteChanged();
                OnPropertyChanged(nameof(CoXeDuocChon));
            }
        }

        private string tieuDeDanhSachXe;
        public string TieuDeDanhSachXe { get { return tieuDeDanhSachXe; } set { tieuDeDanhSachXe = value; OnPropertyChanged(); } }
        private string tuKhoaTimKiem;
        public string TuKhoaTimKiem { get { return tuKhoaTimKiem; } set { tuKhoaTimKiem = value; OnPropertyChanged(); } }
        private string tenDongXeNhap;
        public string TenDongXeNhap { get { return tenDongXeNhap; } set { tenDongXeNhap = value; OnPropertyChanged(); } }
        private string loaiXeNhap;
        public string LoaiXeNhap { get { return loaiXeNhap; } set { loaiXeNhap = value; OnPropertyChanged(); } }
        private string mauSacNhap;
        public string MauSacNhap { get { return mauSacNhap; } set { mauSacNhap = value; OnPropertyChanged(); } }
        private string namSXNhap;
        public string NamSXNhap { get { return namSXNhap; } set { namSXNhap = value; OnPropertyChanged(); } }
        private string giaXeNhap;
        public string GiaXeNhap { get { return giaXeNhap; } set { giaXeNhap = value; OnPropertyChanged(); } }
        private string hinhAnhNhap;
        public string HinhAnhNhap { get { return hinhAnhNhap; } set { hinhAnhNhap = value; OnPropertyChanged(); } }
        private string moTaNhap;
        public string MoTaNhap { get { return moTaNhap; } set { moTaNhap = value; OnPropertyChanged(); } }
        private string soLuongTonNhap;
        public string SoLuongTonNhap { get { return soLuongTonNhap; } set { soLuongTonNhap = value; OnPropertyChanged(); } }

        public bool CoXeDuocChon { get { return XeDangChon != null; } }

        public ICommand LenhQuayLaiHangXe { get; }
        public ICommand LenhMoThemXe { get; }
        public ICommand LenhMoSuaXe { get { return lenhMoSuaXe; } }
        public ICommand LenhXoaXe { get { return lenhXoaXe; } }
        public ICommand LenhLuuXe { get; }
        public ICommand LenhHuyFormXe { get; }
        public ICommand LenhTimKiemXe { get; }
        public ICommand LenhMuaNgay { get; }

        public Xe_VM() : this(null) { }

        public Xe_VM(HangXe hangXe)
        {
            hangXeDuocChon = hangXe;
            LenhQuayLaiHangXe = new RelayCommand(_ => NavigationService.Navigate("QuanLyXe"));
            LenhMoThemXe = new RelayCommand(_ => MoThemXe());
            lenhMoSuaXe = new RelayCommand(_ => MoSuaXe(), _ => XeDangChon != null);
            lenhXoaXe = new RelayCommand(_ => XoaXe(), _ => XeDangChon != null);
            LenhLuuXe = new RelayCommand(parameter => LuuXe(parameter as Window));
            LenhHuyFormXe = new RelayCommand(parameter => DongFormXe(parameter as Window));
            LenhTimKiemXe = new RelayCommand(_ => TaiDanhSachXe());

            CapNhatTieuDe();
            TaiDanhSachXe();
            LamMoiNhapXe();
        }
        private void CapNhatTieuDe()
        {
            if (hangXeDuocChon == null || string.IsNullOrWhiteSpace(hangXeDuocChon.TenHang))
            {
                TieuDeDanhSachXe = "DANH SÁCH XE";
                return;
            }
            TieuDeDanhSachXe = "DANH SÁCH XE - " + hangXeDuocChon.TenHang;
        }

        private void TaiDanhSachXe()
        {
            // Lấy trực tiếp từ Database qua hàm SQL
            var tatCaXe = DuLieuHeThong.LayDanhSachXe();
            IEnumerable<Car> danhSachLoc = tatCaXe.Cast<Car>();

            if (hangXeDuocChon != null && !string.IsNullOrWhiteSpace(hangXeDuocChon.TenHang))
            {
                danhSachLoc = danhSachLoc.Where(item =>
                    string.Equals(item.TenHang, hangXeDuocChon.TenHang, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(TuKhoaTimKiem))
            {
                string tuKhoa = TuKhoaTimKiem.Trim().ToLower();
                danhSachLoc = danhSachLoc.Where(item =>
                    (item.TenDongXe ?? string.Empty).ToLower().Contains(tuKhoa) ||
                    (item.LoaiXe ?? string.Empty).ToLower().Contains(tuKhoa) ||
                    (item.MauSac ?? string.Empty).ToLower().Contains(tuKhoa));
            }

            DanhSachXe = new ObservableCollection<Car>(danhSachLoc);
            lenhMoSuaXe.RaiseCanExecuteChanged();
            lenhXoaXe.RaiseCanExecuteChanged();
            OnPropertyChanged(nameof(CoXeDuocChon));
        }

        private void LuuXe(Window cuaSo)
        {
            try
            {
                int namSX, soLuongTon, giaXe;
                if (!KiemTraDuLieuXe(out namSX, out soLuongTon, out giaXe)) return;

                string tenHangLuu = LayTenHangLuuXe();
                string tenDongXeCu = dangSuaXe ? XeDangChon.TenDongXe : TenDongXeNhap.Trim();

                // Gọi hàm lưu trực tiếp vào SQL
                DuLieuHeThong.LuuXeSQL(
                    tenHangLuu,
                    TenDongXeNhap.Trim(),
                    LoaiXeNhap.Trim(),
                    MauSacNhap.Trim(),
                    namSX,
                    giaXe,
                    HinhAnhNhap.Trim(),
                    MoTaNhap.Trim(),
                    soLuongTon,
                    !dangSuaXe, // isNew = true nếu không phải đang sửa
                    tenDongXeCu
                );

                dangSuaXe = false;
                TaiDanhSachXe();
                cuaSo?.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi Database: " + ex.Message, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void XoaXe()
        {
            try
            {
                if (XeDangChon == null) return;

                var ketQua = MessageBox.Show("Bạn có chắc muốn xóa xe đang chọn?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (ketQua == MessageBoxResult.Yes)
                {
                    // Gọi hàm xóa trực tiếp từ SQL
                    DuLieuHeThong.XoaXeSQL(XeDangChon.TenHang, XeDangChon.TenDongXe);

                    XeDangChon = null;
                    LamMoiNhapXe();
                    TaiDanhSachXe();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể xóa xe: " + ex.Message, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool KiemTraDuLieuXe(out int namSX, out int soLuongTon, out int giaXe)
        {
            namSX = 0; soLuongTon = 0; giaXe = 0;
            if (string.IsNullOrWhiteSpace(TenDongXeNhap)) return false;
            if (string.IsNullOrWhiteSpace(LoaiXeNhap)) return false;
            if (!int.TryParse(NamSXNhap, out namSX)) return false;
            if (!int.TryParse(SoLuongTonNhap, out soLuongTon)) return false;
            string giaRaw = GiaXeNhap.Trim().Replace(".", string.Empty).Replace(",", string.Empty);
            if (!int.TryParse(giaRaw, out giaXe)) return false;
            return true;
        }

        private string LayTenHangLuuXe()
        {
            if (hangXeDuocChon != null) return hangXeDuocChon.TenHang;
            if (XeDangChon != null) return XeDangChon.TenHang;
            return "Honda";
        }

        private void LamMoiNhapXe()
        {
            TenDongXeNhap = LoaiXeNhap = MauSacNhap = MoTaNhap = string.Empty;
            NamSXNhap = DateTime.Now.Year.ToString();
            GiaXeNhap = SoLuongTonNhap = "0";
            HinhAnhNhap = "https://upload.wikimedia.org/wikipedia/commons/thumb/a/ac/No_image_available.svg/320px-No_image_available.svg.png";
        }

        private void MoThemXe() { dangSuaXe = false; LamMoiNhapXe(); var f = new W_ThemXe { DataContext = this }; f.ShowDialog(); }
        private void MoSuaXe() { if (XeDangChon == null) return; dangSuaXe = true; var f = new W_SuaXe { DataContext = this }; f.ShowDialog(); }
        private void DongFormXe(Window cuaSo) { dangSuaXe = false; LamMoiNhapXe(); cuaSo?.Close(); }
    }
}