using Doan.Helper;
using Doan.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Doan.ViewModel
{
    // Mặt hàng chọn để nhập (xe hoặc phụ tùng).
    public class MatHangNhapItem
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string Loai { get; set; } // "Xe" | "PhuTung"
        public string HienThi { get { return Ma + " - " + Ten; } }
    }

    public class NhapKho_VM : BaseViewModel
    {
        public ObservableCollection<string> DanhSachLoai { get; }

        private string loaiDangChon;
        public string LoaiDangChon
        {
            get { return loaiDangChon; }
            set
            {
                loaiDangChon = value;
                OnPropertyChanged();
                TaiDanhSachMatHang();
            }
        }

        private ObservableCollection<MatHangNhapItem> danhSachMatHang;
        public ObservableCollection<MatHangNhapItem> DanhSachMatHang
        {
            get { return danhSachMatHang; }
            set { danhSachMatHang = value; OnPropertyChanged(); }
        }

        private MatHangNhapItem matHangDangChon;
        public MatHangNhapItem MatHangDangChon
        {
            get { return matHangDangChon; }
            set { matHangDangChon = value; OnPropertyChanged(); }
        }

        private string soLuongNhap = "1";
        public string SoLuongNhap
        {
            get { return soLuongNhap; }
            set { soLuongNhap = value; OnPropertyChanged(); }
        }

        private string donGiaNhap = "0";
        public string DonGiaNhap
        {
            get { return donGiaNhap; }
            set { donGiaNhap = value; OnPropertyChanged(); }
        }

        private string nhaCungCap;
        public string NhaCungCap
        {
            get { return nhaCungCap; }
            set { nhaCungCap = value; OnPropertyChanged(); }
        }

        private string ghiChu;
        public string GhiChu
        {
            get { return ghiChu; }
            set { ghiChu = value; OnPropertyChanged(); }
        }

        public ObservableCollection<DongNhapKho> GioNhap { get; }

        public long TongTienNhap
        {
            get { return (long)GioNhap.Sum(d => d.ThanhTien); }
        }

        public ICommand LenhThemDong { get; }
        public ICommand LenhXoaDong { get; }
        public ICommand LenhLuuPhieu { get; }
        public ICommand LenhLamMoi { get; }

        public NhapKho_VM()
        {
            DanhSachLoai = new ObservableCollection<string> { "Xe", "Phụ tùng" };
            GioNhap = new ObservableCollection<DongNhapKho>();
            DanhSachMatHang = new ObservableCollection<MatHangNhapItem>();

            LenhThemDong = new RelayCommand(_ => ThemDong());
            LenhXoaDong = new RelayCommand(p => XoaDong(p as DongNhapKho), p => p is DongNhapKho);
            LenhLuuPhieu = new RelayCommand(_ => LuuPhieu());
            LenhLamMoi = new RelayCommand(_ => LamMoiPhieu());

            LoaiDangChon = "Xe";
        }

        private string MaLoai
        {
            get { return LoaiDangChon == "Phụ tùng" ? "PhuTung" : "Xe"; }
        }

        private void TaiDanhSachMatHang()
        {
            var ds = new List<MatHangNhapItem>();
            using (var ctx = new QuanLyBanXeMayEntities())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                if (MaLoai == "Xe")
                {
                    ds = ctx.Xes.OrderBy(x => x.MaXe)
                        .Select(x => new MatHangNhapItem { Ma = x.MaXe, Ten = x.TenXe, Loai = "Xe" })
                        .ToList();
                }
                else
                {
                    // Chỉ phụ tùng mới quản lý tồn kho (dịch vụ không nhập kho).
                    ds = ctx.DichVuPhuTungs.Where(d => d.Loai == "PhuTung").OrderBy(d => d.MaPT)
                        .Select(d => new MatHangNhapItem { Ma = d.MaPT, Ten = d.Ten, Loai = "PhuTung" })
                        .ToList();
                }
            }
            DanhSachMatHang = new ObservableCollection<MatHangNhapItem>(ds);
            MatHangDangChon = DanhSachMatHang.FirstOrDefault();
        }

        private void ThemDong()
        {
            if (MatHangDangChon == null)
            {
                MessageBox.Show("Vui lòng chọn mặt hàng.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int soLuong;
            if (!int.TryParse((SoLuongNhap ?? string.Empty).Trim(), out soLuong) || soLuong <= 0)
            {
                MessageBox.Show("Số lượng phải là số nguyên dương.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            decimal donGia;
            string giaRaw = (DonGiaNhap ?? string.Empty).Replace(".", string.Empty).Replace(",", string.Empty).Replace(" ", string.Empty);
            if (!decimal.TryParse(giaRaw, out donGia) || donGia < 0)
            {
                MessageBox.Show("Đơn giá nhập không hợp lệ.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var item = MatHangDangChon;
            var dongCu = GioNhap.FirstOrDefault(d => d.LoaiMatHang == item.Loai && d.MaMatHang == item.Ma);
            if (dongCu != null)
            {
                dongCu.SoLuong += soLuong;
                dongCu.DonGiaNhap = donGia;
                // Làm mới hiển thị tổng (ObservableCollection không tự báo thay đổi thuộc tính item).
                int idx = GioNhap.IndexOf(dongCu);
                GioNhap.RemoveAt(idx);
                GioNhap.Insert(idx, dongCu);
            }
            else
            {
                GioNhap.Add(new DongNhapKho
                {
                    LoaiMatHang = item.Loai,
                    MaXe = item.Loai == "Xe" ? item.Ma : null,
                    MaPT = item.Loai == "PhuTung" ? item.Ma : null,
                    TenMatHang = item.Ten,
                    SoLuong = soLuong,
                    DonGiaNhap = donGia
                });
            }

            OnPropertyChanged(nameof(TongTienNhap));
            SoLuongNhap = "1";
            DonGiaNhap = "0";
        }

        private void XoaDong(DongNhapKho dong)
        {
            if (dong == null) return;
            GioNhap.Remove(dong);
            OnPropertyChanged(nameof(TongTienNhap));
        }

        private void LuuPhieu()
        {
            if (GioNhap.Count == 0)
            {
                MessageBox.Show("Phiếu nhập đang trống. Vui lòng thêm ít nhất 1 dòng.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string maNV = PhienDangNhap.NhanVienHienTai?.MaNV;

            try
            {
                string maPN = NhapKhoRepository.ThucHienNhapKho(maNV, (NhaCungCap ?? string.Empty).Trim(), (GhiChu ?? string.Empty).Trim(), GioNhap.ToList());
                MessageBox.Show("Đã lập phiếu nhập " + maPN + " và cập nhật tồn kho.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                LamMoiPhieu();
            }
            catch (Exception ex)
            {
                Exception loi = ex;
                while (loi.InnerException != null) loi = loi.InnerException;
                MessageBox.Show("Lập phiếu nhập thất bại: " + loi.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LamMoiPhieu()
        {
            GioNhap.Clear();
            NhaCungCap = string.Empty;
            GhiChu = string.Empty;
            SoLuongNhap = "1";
            DonGiaNhap = "0";
            OnPropertyChanged(nameof(TongTienNhap));
        }
    }
}
