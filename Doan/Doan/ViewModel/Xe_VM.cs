using Doan.Helper;
using Doan.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Windows.Input;
using Doan.View;
using System.Windows;

namespace Doan.ViewModel
{
    public class Xe_VM : BaseViewModel
    {
        private readonly HangXe hangXeDuocChon;
        private readonly RelayCommand lenhMoSuaXe;
        private readonly RelayCommand lenhXoaXe;
        private bool dangSuaXe;

        private ObservableCollection<Xe> danhSachXe;
        public ObservableCollection<Xe> DanhSachXe
        {
            get { return danhSachXe; }
            set
            {
                danhSachXe = value;
                OnPropertyChanged(nameof(DanhSachXe));
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
                if (xeDangChon != null)
                {
                    TenDongXeNhap = xeDangChon.TenXe;
                    LoaiXeNhap = xeDangChon.LoaiXe;
                    MauSacNhap = xeDangChon.MauSac;
                    NamSXNhap = xeDangChon.NamSX?.ToString() ?? "";
                    GiaXeNhap = xeDangChon.GiaBan.HasValue
                        ? string.Format("{0:N0}", xeDangChon.GiaBan.Value).Replace(",", ".")
                        : "0";
                    HinhAnhNhap = xeDangChon.HinhAnh;
                    MoTaNhap = xeDangChon.MoTa;
                    SoLuongTonNhap = xeDangChon.SoLuongTon?.ToString() ?? "0";
                }
                lenhMoSuaXe?.RaiseCanExecuteChanged();
                lenhXoaXe?.RaiseCanExecuteChanged();
                OnPropertyChanged(nameof(CoXeDuocChon));
            }
        }

        private string tieuDeDanhSachXe;
        public string TieuDeDanhSachXe
        {
            get { return tieuDeDanhSachXe; }
            set
            {
                tieuDeDanhSachXe = value;
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

        private string tenDongXeNhap;
        public string TenDongXeNhap
        {
            get { return tenDongXeNhap; }
            set
            {
                tenDongXeNhap = value;
                OnPropertyChanged();
            }
        }

        private string loaiXeNhap;
        public string LoaiXeNhap
        {
            get { return loaiXeNhap; }
            set
            {
                loaiXeNhap = value;
                OnPropertyChanged();
            }
        }

        private string mauSacNhap;
        public string MauSacNhap
        {
            get { return mauSacNhap; }
            set
            {
                mauSacNhap = value;
                OnPropertyChanged();
            }
        }

        private string namSXNhap;
        public string NamSXNhap
        {
            get { return namSXNhap; }
            set
            {
                namSXNhap = value;
                OnPropertyChanged();
            }
        }

        private string giaXeNhap;
        public string GiaXeNhap
        {
            get { return giaXeNhap; }
            set
            {
                giaXeNhap = value;
                OnPropertyChanged();
            }
        }

        private string hinhAnhNhap;
        public string HinhAnhNhap
        {
            get { return hinhAnhNhap; }
            set
            {
                hinhAnhNhap = value;
                OnPropertyChanged();
            }
        }

        private string moTaNhap;
        public string MoTaNhap
        {
            get { return moTaNhap; }
            set
            {
                moTaNhap = value;
                OnPropertyChanged();
            }
        }

        private string soLuongTonNhap;
        public string SoLuongTonNhap
        {
            get { return soLuongTonNhap; }
            set
            {
                soLuongTonNhap = value;
                OnPropertyChanged();
            }
        }

        public bool CoXeDuocChon => XeDangChon != null;

        public ICommand LenhQuayLaiHangXe { get; }
        public ICommand LenhMoThemXe { get; }
        public ICommand LenhMoSuaXe => lenhMoSuaXe;
        public ICommand LenhXoaXe => lenhXoaXe;
        public ICommand LenhLuuXe { get; }
        public ICommand LenhHuyFormXe { get; }
        public ICommand LenhTimKiemXe { get; }
        public ICommand LenhBanXe { get; }

        public Xe_VM() : this(null) { }

        public Xe_VM(HangXe hangXe)
        {
            hangXeDuocChon = hangXe;
            LenhQuayLaiHangXe = new RelayCommand(_ => DieuHuongTuMain("QuanLyXe"));
            LenhMoThemXe = new RelayCommand(_ => MoThemXe());
            lenhMoSuaXe = new RelayCommand(_ => MoSuaXe(), _ => XeDangChon != null);
            lenhXoaXe = new RelayCommand(_ => XoaXe(), _ => XeDangChon != null);
            LenhLuuXe = new RelayCommand(parameter => LuuXe(parameter as Window));
            LenhHuyFormXe = new RelayCommand(parameter => DongFormXe(parameter as Window));
            LenhTimKiemXe = new RelayCommand(_ => TaiDanhSachXe());
            LenhBanXe = new RelayCommand(_ => BanXe());

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

        private void DieuHuongTuMain(string tenManHinh, object duLieu = null)
        {
            var mainVm = Application.Current?.MainWindow?.DataContext as MainWindows_VM;
            mainVm?.DieuHuong(tenManHinh, duLieu);
        }

        private void TaiDanhSachXe()
        {
            using (var ctx = new QuanLyBanXeMayEntities())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                var xeEntities = ctx.Xes.Include("HangXe").ToList();

                // Lọc theo hãng nếu có chọn
                if (hangXeDuocChon != null && !string.IsNullOrWhiteSpace(hangXeDuocChon.TenHang))
                {
                    xeEntities = xeEntities
                        .Where(x => x.HangXe != null &&
                                    string.Equals(x.HangXe.TenHang, hangXeDuocChon.TenHang, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                // Lọc theo từ khóa tìm kiếm
                if (!string.IsNullOrWhiteSpace(TuKhoaTimKiem))
                {
                    string tuKhoa = TuKhoaTimKiem.Trim().ToLower();
                    xeEntities = xeEntities.Where(x =>
                        (x.TenXe ?? string.Empty).ToLower().Contains(tuKhoa) ||
                        (x.LoaiXe ?? string.Empty).ToLower().Contains(tuKhoa) ||
                        (x.MauSac ?? string.Empty).ToLower().Contains(tuKhoa)
                    ).ToList();
                }

                DanhSachXe = new ObservableCollection<Xe>(xeEntities);

                // Giữ lại xe đang chọn nếu có
                if (XeDangChon != null)
                {
                    XeDangChon = DanhSachXe.FirstOrDefault(item =>
                        string.Equals(item.MaXe, XeDangChon.MaXe, StringComparison.OrdinalIgnoreCase));
                }
            }

            lenhMoSuaXe.RaiseCanExecuteChanged();
            lenhXoaXe.RaiseCanExecuteChanged();
            OnPropertyChanged(nameof(CoXeDuocChon));
        }

        private void LamMoiNhapXe()
        {
            TenDongXeNhap = string.Empty;
            LoaiXeNhap = string.Empty;
            MauSacNhap = string.Empty;
            NamSXNhap = DateTime.Now.Year.ToString();
            GiaXeNhap = "0";
            HinhAnhNhap = "https://upload.wikimedia.org/wikipedia/commons/thumb/a/ac/No_image_available.svg/320px-No_image_available.svg.png";
            MoTaNhap = string.Empty;
            SoLuongTonNhap = "0";
        }

        private bool KiemTraDuLieuXe(out int namSX, out int soLuongTon, out int giaXe)
        {
            namSX = 0;
            soLuongTon = 0;
            giaXe = 0;

            if (string.IsNullOrWhiteSpace(TenDongXeNhap))
            {
                MessageBox.Show("Tên dòng xe không được để trống.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(LoaiXeNhap))
            {
                MessageBox.Show("Loại xe không được để trống.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!int.TryParse(NamSXNhap, out namSX) || namSX < 1900)
            {
                MessageBox.Show("Năm sản xuất không hợp lệ.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            int namHienTai = DateTime.Now.Year;
            if (namSX > namHienTai + 1)
            {
                MessageBox.Show("Năm sản xuất không được lớn hơn năm hiện tại + 1.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!int.TryParse(SoLuongTonNhap, out soLuongTon) || soLuongTon < 0)
            {
                MessageBox.Show("Số lượng tồn không hợp lệ.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            string giaChuanHoa;
            if (!ThuChuanHoaGiaXe(GiaXeNhap, out giaXe, out giaChuanHoa))
            {
                MessageBox.Show("Giá xe không hợp lệ.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            GiaXeNhap = giaChuanHoa;

            if (string.IsNullOrWhiteSpace(HinhAnhNhap))
            {
                HinhAnhNhap = "https://upload.wikimedia.org/wikipedia/commons/thumb/a/ac/No_image_available.svg/320px-No_image_available.svg.png";
            }

            if (!LaUrlHopLe(HinhAnhNhap.Trim()))
            {
                MessageBox.Show("Đường dẫn hình ảnh phải là URL hợp lệ (http/https) hoặc đường dẫn file ảnh có thật.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private bool ThuChuanHoaGiaXe(string giaNhap, out int gia, out string giaChuanHoa)
        {
            gia = 0;
            giaChuanHoa = "0";

            if (string.IsNullOrWhiteSpace(giaNhap))
            {
                return false;
            }

            string giaRaw = giaNhap.Trim().Replace(".", string.Empty).Replace(",", string.Empty).Replace(" ", string.Empty);
            if (!int.TryParse(giaRaw, out gia) || gia < 0)
            {
                return false;
            }

            giaChuanHoa = string.Format("{0:N0}", gia).Replace(",", ".");
            return true;
        }

        private bool LaUrlHopLe(string duongDan)
        {
            if (string.IsNullOrWhiteSpace(duongDan))
            {
                return false;
            }

            Uri uri;
            if (Uri.TryCreate(duongDan, UriKind.Absolute, out uri)
                && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
            {
                return true;
            }

            // Cho phép chọn file ảnh có sẵn trong máy (nút "Chọn file").
            return System.IO.File.Exists(duongDan);
        }

        private string LayTenHangLuuXe()
        {
            if (hangXeDuocChon != null && !string.IsNullOrWhiteSpace(hangXeDuocChon.TenHang))
            {
                return hangXeDuocChon.TenHang;
            }

            if (XeDangChon != null && XeDangChon.HangXe != null && !string.IsNullOrWhiteSpace(XeDangChon.HangXe.TenHang))
            {
                return XeDangChon.HangXe.TenHang;
            }

            using (var ctx = new QuanLyBanXeMayEntities())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                var hangMacDinh = ctx.HangXes.FirstOrDefault();
                return hangMacDinh != null ? hangMacDinh.TenHang : string.Empty;
            }
        }

        private void LuuXe(Window cuaSo)
        {
            int namSX;
            int soLuongTon;
            int giaXe;
            if (!KiemTraDuLieuXe(out namSX, out soLuongTon, out giaXe))
            {
                return;
            }

            string tenHangLuu = LayTenHangLuuXe();

            if (dangSuaXe && XeDangChon != null)
            {
                using (var ctx = new QuanLyBanXeMayEntities())
                {
                    ctx.Configuration.LazyLoadingEnabled = false;
                    string tenDongXeNhapTrim = TenDongXeNhap.Trim();
                    string tenXeCu = XeDangChon.TenXe;
                    var xeMoi = ctx.Xes.Include("HangXe").FirstOrDefault(x =>
                        x.TenXe == tenDongXeNhapTrim &&
                        x.TenXe != tenXeCu &&
                        x.HangXe != null &&
                        x.HangXe.TenHang == tenHangLuu);

                    if (xeMoi != null)
                    {
                        MessageBox.Show("Tên dòng xe đã tồn tại trong hãng này.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    var efXe = ctx.Xes.Include("HangXe").FirstOrDefault(x =>
                        x.MaXe == XeDangChon.MaXe &&
                        x.HangXe != null &&
                        x.HangXe.TenHang == tenHangLuu);

                    if (efXe != null)
                    {
                        efXe.TenXe = TenDongXeNhap.Trim();
                        efXe.LoaiXe = LoaiXeNhap.Trim();
                        efXe.MauSac = MauSacNhap.Trim();
                        efXe.NamSX = namSX;
                        efXe.GiaBan = giaXe;
                        efXe.HinhAnh = HinhAnhNhap.Trim();
                        efXe.MoTa = MoTaNhap.Trim();
                        efXe.SoLuongTon = soLuongTon;
                        ctx.SaveChanges();
                    }
                }

                dangSuaXe = false;
                TaiDanhSachXe();
                XeDangChon = DanhSachXe.FirstOrDefault(d => d.TenXe == TenDongXeNhap.Trim());
                cuaSo?.Close();
            }
            else
            {
                using (var ctx = new QuanLyBanXeMayEntities())
                {
                    ctx.Configuration.LazyLoadingEnabled = false;
                    string tenDongXeNhapTrim2 = TenDongXeNhap.Trim();
                    var xeTrung = ctx.Xes.Include("HangXe").FirstOrDefault(x =>
                        x.TenXe == tenDongXeNhapTrim2 &&
                        x.HangXe != null &&
                        x.HangXe.TenHang == tenHangLuu);

                    if (xeTrung != null)
                    {
                        MessageBox.Show("Tên dòng xe đã tồn tại trong hãng này.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    var hang = ctx.HangXes.FirstOrDefault(h => h.TenHang == tenHangLuu);
                    if (hang == null)
                    {
                        MessageBox.Show("Không tìm thấy hãng xe. Vui lòng kiểm tra lại.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    string maMoiXe = TaoMaXeMoi(ctx);
                    var efXe = new Xe
                    {
                        MaXe = maMoiXe,
                        TenXe = TenDongXeNhap.Trim(),
                        LoaiXe = LoaiXeNhap.Trim(),
                        MauSac = MauSacNhap.Trim(),
                        NamSX = namSX,
                        GiaBan = giaXe,
                        HinhAnh = HinhAnhNhap.Trim(),
                        MoTa = MoTaNhap.Trim(),
                        MaHang = hang.MaHang,
                        SoLuongTon = soLuongTon
                    };
                    ctx.Xes.Add(efXe);
                    ctx.SaveChanges();
                }

                dangSuaXe = false;
                TaiDanhSachXe();
                XeDangChon = DanhSachXe.FirstOrDefault(item => item.TenXe == TenDongXeNhap.Trim());
                cuaSo?.Close();
            }
        }

        private string TaoMaXeMoi(QuanLyBanXeMayEntities ctx)
        {
            int soLonNhat = 0;
            var tatCaMaXe = ctx.Xes.Select(x => x.MaXe).ToList();
            foreach (string ma in tatCaMaXe)
            {
                if (string.IsNullOrWhiteSpace(ma))
                {
                    continue;
                }
                string so = ma.Trim().ToUpper().Replace("XE", string.Empty);
                int maSo;
                if (int.TryParse(so, out maSo) && maSo > soLonNhat)
                {
                    soLonNhat = maSo;
                }
            }
            return "XE" + (soLonNhat + 1).ToString("000");
        }

        private void XoaXe()
        {
            if (XeDangChon == null)
            {
                return;
            }

            var ketQua = MessageBox.Show("Bạn có chắc muốn xóa xe đang chọn?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (ketQua != MessageBoxResult.Yes)
            {
                return;
            }

            using (var ctx = new QuanLyBanXeMayEntities())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                var ef = ctx.Xes.FirstOrDefault(x => x.MaXe == XeDangChon.MaXe);
                if (ef != null)
                {
                    ctx.Xes.Remove(ef);
                    ctx.SaveChanges();
                }
            }

            XeDangChon = null;
            LamMoiNhapXe();
            TaiDanhSachXe();
        }

        private void MoThemXe()
        {
            dangSuaXe = false;
            LamMoiNhapXe();
            var cuaSoThemXe = new W_ThemXe();
            cuaSoThemXe.DataContext = this;
            cuaSoThemXe.ShowDialog();
        }

        private void MoSuaXe()
        {
            if (XeDangChon == null)
            {
                return;
            }

            dangSuaXe = true;
            TenDongXeNhap = XeDangChon.TenXe;
            LoaiXeNhap = XeDangChon.LoaiXe;
            MauSacNhap = XeDangChon.MauSac;
            NamSXNhap = XeDangChon.NamSX?.ToString() ?? "";
            GiaXeNhap = XeDangChon.GiaBan.HasValue
                ? string.Format("{0:N0}", XeDangChon.GiaBan.Value).Replace(",", ".")
                : "0";
            HinhAnhNhap = XeDangChon.HinhAnh;
            MoTaNhap = XeDangChon.MoTa;
            SoLuongTonNhap = XeDangChon.SoLuongTon?.ToString() ?? "0";
            var cuaSoSuaXe = new W_SuaXe();
            cuaSoSuaXe.DataContext = this;
            cuaSoSuaXe.ShowDialog();
        }

        private void BanXe()
        {
            if (XeDangChon == null)
            {
                MessageBox.Show("Vui lòng chọn xe cần bán.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if ((XeDangChon.SoLuongTon ?? 0) <= 0)
            {
                MessageBox.Show("Xe đã hết hàng trong kho.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Thêm xe vào giỏ hàng dùng chung rồi chuyển sang màn hình lập hóa đơn.
            var gioHang = DichVu_VM.GioHangDungChung;
            var matHang = gioHang.FirstOrDefault(item => item.MaMatHang == XeDangChon.MaXe);
            if (matHang == null)
            {
                gioHang.Add(new MatHangGio_VM
                {
                    MaMatHang = XeDangChon.MaXe,
                    TenMatHang = XeDangChon.TenXe,
                    DonGia = (int)(XeDangChon.GiaBan ?? 0),
                    SoLuong = 1
                });
            }
            else
            {
                if (matHang.SoLuong + 1 > (XeDangChon.SoLuongTon ?? 0))
                {
                    MessageBox.Show("Số lượng trong giỏ vượt quá tồn kho hiện có.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                matHang.SoLuong += 1;
            }

            DieuHuongTuMain("ThanhToan");
        }

        private void DongFormXe(Window cuaSo)
        {
            dangSuaXe = false;
            LamMoiNhapXe();
            cuaSo?.Close();
        }
    }
}
