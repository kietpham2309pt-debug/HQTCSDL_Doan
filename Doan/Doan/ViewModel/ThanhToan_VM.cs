using Doan.Helper;
using Doan.Model;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Doan.ViewModel
{
    // Màn hình lập hóa đơn / thanh toán (tách riêng khỏi lịch sử giao dịch).
    public class ThanhToan_VM : BaseViewModel
    {
        public ObservableCollection<MatHangGio_VM> GioHangHienTai
        {
            get { return DichVu_VM.GioHangDungChung; }
        }

        private string sdtKhachNhap;
        public string SDTKhachNhap
        {
            get { return sdtKhachNhap; }
            set
            {
                sdtKhachNhap = value;
                OnPropertyChanged();

                if (KhachHangDuocChon != null &&
                    KhachHangDuocChon.SDT != (sdtKhachNhap ?? string.Empty).Trim())
                {
                    KhachHangDuocChon = null;
                }
            }
        }

        private KhachHang khachHangDuocChon;
        public KhachHang KhachHangDuocChon
        {
            get { return khachHangDuocChon; }
            set
            {
                khachHangDuocChon = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DiaChiKhachHang));
                OnPropertyChanged(nameof(CoKhachHang));
            }
        }

        // Dùng để hiện/ẩn panel thông tin khách trong giao diện.
        public bool CoKhachHang
        {
            get { return KhachHangDuocChon != null; }
        }

        // Danh sách nhân viên ĐANG LÀM VIỆC cho dropdown chọn nhanh người lập.
        public ObservableCollection<NhanVien> DanhSachNhanVienLap { get; }

        private NhanVien nhanVienLapDuocChon;
        public NhanVien NhanVienLapDuocChon
        {
            get { return nhanVienLapDuocChon; }
            set
            {
                nhanVienLapDuocChon = value;
                OnPropertyChanged();
            }
        }

        private DateTime ngayLapNhap;
        public DateTime NgayLapNhap
        {
            get { return ngayLapNhap; }
            set
            {
                ngayLapNhap = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> DanhSachHinhThucThanhToan { get; }

        private string hinhThucThanhToanDangChon;
        public string HinhThucThanhToanDangChon
        {
            get { return hinhThucThanhToanDangChon; }
            set
            {
                hinhThucThanhToanDangChon = value;
                OnPropertyChanged();
            }
        }

        public long TongTienHang
        {
            get { return GioHangHienTai.Sum(item => item.ThanhTien); }
        }

        public long ThanhTienThanhToan
        {
            get { return TongTienHang; }
        }

        public string DiaChiKhachHang
        {
            get
            {
                if (KhachHangDuocChon == null || string.IsNullOrWhiteSpace(KhachHangDuocChon.DiaChi))
                {
                    return "Chưa có thông tin địa chỉ";
                }

                return KhachHangDuocChon.DiaChi;
            }
        }

        public ICommand LenhKiemTraSDTKhach { get; }
        public ICommand LenhHuyHoaDon { get; }
        public ICommand LenhXacNhanThanhToan { get; }

        public ThanhToan_VM()
        {
            DanhSachHinhThucThanhToan = new ObservableCollection<string>
            {
                "Tiền mặt",
                "Chuyển khoản",
                "Trả góp"
            };
            DanhSachNhanVienLap = new ObservableCollection<NhanVien>();

            HinhThucThanhToanDangChon = DanhSachHinhThucThanhToan.FirstOrDefault();
            NgayLapNhap = DateTime.Now;
            TaiDanhSachNhanVienLap();

            LenhKiemTraSDTKhach = new RelayCommand(_ => KiemTraSDTKhach());
            LenhHuyHoaDon = new RelayCommand(_ => HuyHoaDon());
            LenhXacNhanThanhToan = new RelayCommand(_ => XacNhanThanhToan());

            DangKySuKienGioHang();
            CapNhatTienThanhToan();
        }

        private void TaiDanhSachNhanVienLap()
        {
            using (var ctx = new QuanLyBanXeMayEntities())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                var ds = ctx.NhanViens
                    .Where(item => item.TrangThai == "Đang làm việc")
                    .OrderBy(item => item.HoTen)
                    .ToList();

                DanhSachNhanVienLap.Clear();
                foreach (var nv in ds)
                {
                    DanhSachNhanVienLap.Add(nv);
                }
            }

            NhanVienLapDuocChon = DanhSachNhanVienLap.FirstOrDefault();
        }

        private void DangKySuKienGioHang()
        {
            GioHangHienTai.CollectionChanged -= XuLyThayDoiGioHang;
            GioHangHienTai.CollectionChanged += XuLyThayDoiGioHang;

            foreach (MatHangGio_VM item in GioHangHienTai)
            {
                item.PropertyChanged -= XuLyThayDoiMatHang;
                item.PropertyChanged += XuLyThayDoiMatHang;
            }
        }

        private void XuLyThayDoiGioHang(object nguon, NotifyCollectionChangedEventArgs suKien)
        {
            if (suKien.NewItems != null)
            {
                foreach (MatHangGio_VM item in suKien.NewItems)
                {
                    item.PropertyChanged -= XuLyThayDoiMatHang;
                    item.PropertyChanged += XuLyThayDoiMatHang;
                }
            }

            if (suKien.OldItems != null)
            {
                foreach (MatHangGio_VM item in suKien.OldItems)
                {
                    item.PropertyChanged -= XuLyThayDoiMatHang;
                }
            }

            CapNhatTienThanhToan();
        }

        private void XuLyThayDoiMatHang(object nguon, PropertyChangedEventArgs suKien)
        {
            if (suKien.PropertyName == nameof(MatHangGio_VM.SoLuong) ||
                suKien.PropertyName == nameof(MatHangGio_VM.DonGia) ||
                suKien.PropertyName == nameof(MatHangGio_VM.ThanhTien))
            {
                CapNhatTienThanhToan();
            }
        }

        private void KiemTraSDTKhach()
        {
            if (!KiemTraChuoiSo(SDTKhachNhap, 10, 11))
            {
                MessageBox.Show("Số điện thoại khách phải từ 10 đến 11 chữ số.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string sdt = SDTKhachNhap.Trim();

            using (var ctx = new QuanLyBanXeMayEntities())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                var khachHang = ctx.KhachHangs.FirstOrDefault(item =>
                    item.SDT == sdt);
                if (khachHang == null)
                {
                    KhachHangDuocChon = null;
                    MessageBox.Show("Chưa có hồ sơ khách hàng theo số điện thoại này.\nVào tab Khách hàng để tạo hồ sơ trước khi lập hóa đơn.",
                        "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                KhachHangDuocChon = khachHang;
            }
        }

        private void HuyHoaDon()
        {
            bool coGioHang = GioHangHienTai.Count > 0;
            if (coGioHang)
            {
                MessageBoxResult ketQua = MessageBox.Show(
                    "Bạn có chắc muốn xóa toàn bộ giỏ hàng và làm mới thông tin thanh toán?",
                    "Xác nhận",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (ketQua != MessageBoxResult.Yes)
                {
                    return;
                }
            }

            LamMoiThongTinHoaDon(coGioHang);
        }

        private void XacNhanThanhToan()
        {
            if (!KiemTraDuLieuThanhToan())
            {
                return;
            }

            if (!KiemTraTonKhoTruocThanhToan())
            {
                return;
            }

            DateTime ngayLap = NgayLapNhap;
            int soDonDaTao = 0;

            try
            {
                // MaHD là khóa chính nên mỗi mặt hàng phải mang một mã hóa đơn riêng.
                int soBatDau = LaySoHoaDonLonNhat();

                using (var ctx = new QuanLyBanXeMayEntities())
                {
                    ctx.Configuration.LazyLoadingEnabled = false;
                    string maNV = NhanVienLapDuocChon != null ? NhanVienLapDuocChon.MaNV : null;
                    string maKH = KhachHangDuocChon != null ? KhachHangDuocChon.MaKH : null;

                    foreach (MatHangGio_VM item in GioHangHienTai.ToList())
                    {
                        // CSDL ràng buộc ThanhTien > 0 nên bỏ qua dòng giá 0.
                        if (item.ThanhTien <= 0)
                        {
                            continue;
                        }

                        soDonDaTao++;
                        string maHoaDonMoi = "HD" + (soBatDau + soDonDaTao).ToString("000");

                        var hd = new HoaDon
                        {
                            MaHD = maHoaDonMoi,
                            NgayLap = ngayLap,
                            MaNV = maNV,
                            MaKH = maKH,
                            TenDV_SP = item.TenMatHang,
                            SoLuong = item.SoLuong,
                            ThanhTien = item.ThanhTien,
                            PhuongThucThanhToan = HinhThucThanhToanDangChon,
                            TrangThai = "Đã xác nhận"
                        };
                        ctx.HoaDons.Add(hd);

                        // Trừ tồn kho ngay trong cùng giao dịch để dữ liệu nhất quán.
                        if (item.LaPhuTung)
                        {
                            var phuTung = ctx.DichVuPhuTungs.FirstOrDefault(dv => dv.MaPT == item.MaMatHang);
                            if (phuTung != null)
                            {
                                phuTung.TonKho = (phuTung.TonKho ?? 0) - item.SoLuong;
                                if (phuTung.TonKho < 0)
                                {
                                    phuTung.TonKho = 0;
                                }
                            }
                        }
                        else if ((item.MaMatHang ?? string.Empty).Trim().StartsWith("XE", StringComparison.OrdinalIgnoreCase))
                        {
                            var xe = ctx.Xes.FirstOrDefault(x => x.MaXe == item.MaMatHang);
                            if (xe != null)
                            {
                                xe.SoLuongTon = (xe.SoLuongTon ?? 0) - item.SoLuong;
                                if (xe.SoLuongTon < 0)
                                {
                                    xe.SoLuongTon = 0;
                                }
                            }
                        }
                    }

                    ctx.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Thanh toán thất bại: " + ex.Message, "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            GioHangHienTai.Clear();
            CapNhatTienThanhToan();

            MessageBox.Show("Thanh toán thành công. Đã tạo " + soDonDaTao + " hóa đơn.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            LamMoiThongTinHoaDon(false);
        }

        private bool KiemTraDuLieuThanhToan()
        {
            if (GioHangHienTai.Count == 0)
            {
                MessageBox.Show("Giỏ hàng đang trống.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(SDTKhachNhap))
            {
                MessageBox.Show("Vui lòng nhập số điện thoại khách hàng.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!KiemTraChuoiSo(SDTKhachNhap, 10, 11))
            {
                MessageBox.Show("Số điện thoại khách phải từ 10 đến 11 chữ số.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (KhachHangDuocChon == null)
            {
                MessageBox.Show("Chưa có hồ sơ khách. Vào tab Khách hàng để tạo hồ sơ rồi nhấn 'Kiểm tra' lại.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // Bắt buộc có địa chỉ giao hàng.
            if (string.IsNullOrWhiteSpace(KhachHangDuocChon.DiaChi))
            {
                MessageBox.Show("Khách chưa có địa chỉ giao hàng.\nVào tab Khách hàng để cập nhật địa chỉ trước khi lập hóa đơn.",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (NhanVienLapDuocChon == null)
            {
                MessageBox.Show("Vui lòng chọn nhân viên lập hóa đơn.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (NgayLapNhap.Date > DateTime.Now.Date)
            {
                MessageBox.Show("Ngày lập hóa đơn không hợp lệ.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(HinhThucThanhToanDangChon))
            {
                MessageBox.Show("Vui lòng chọn hình thức thanh toán.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private bool KiemTraTonKhoTruocThanhToan()
        {
            foreach (MatHangGio_VM item in GioHangHienTai)
            {
                string ma = (item.MaMatHang ?? string.Empty).Trim();

                using (var ctx = new QuanLyBanXeMayEntities())
                {
                    ctx.Configuration.LazyLoadingEnabled = false;

                    if (item.LaPhuTung)
                    {
                        var phuTung = ctx.DichVuPhuTungs.FirstOrDefault(dv =>
                            dv.MaPT == item.MaMatHang);
                        if (phuTung == null)
                        {
                            MessageBox.Show("Không tìm thấy phụ tùng " + item.TenMatHang + " trong hệ thống.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return false;
                        }

                        if (item.SoLuong > (phuTung.TonKho ?? 0))
                        {
                            MessageBox.Show("Phụ tùng " + item.TenMatHang + " không đủ tồn kho.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return false;
                        }
                    }
                    else if (ma.StartsWith("XE", StringComparison.OrdinalIgnoreCase))
                    {
                        var xe = ctx.Xes.FirstOrDefault(x => x.MaXe == item.MaMatHang);
                        if (xe == null)
                        {
                            MessageBox.Show("Không tìm thấy xe " + item.TenMatHang + " trong hệ thống.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return false;
                        }

                        if (item.SoLuong > (xe.SoLuongTon ?? 0))
                        {
                            MessageBox.Show("Xe " + item.TenMatHang + " không đủ tồn kho.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private int LaySoHoaDonLonNhat()
        {
            int maLonNhat = 0;

            using (var ctx = new QuanLyBanXeMayEntities())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                var all = ctx.HoaDons.Select(h => h.MaHD).ToList();
                foreach (string ma in all)
                {
                    if (string.IsNullOrWhiteSpace(ma))
                    {
                        continue;
                    }

                    string so = ma.Trim().ToUpper().Replace("HD", string.Empty);
                    int maSo;
                    if (int.TryParse(so, out maSo) && maSo > maLonNhat)
                    {
                        maLonNhat = maSo;
                    }
                }
            }

            return maLonNhat;
        }

        private bool KiemTraChuoiSo(string duLieu, int doDaiMin, int doDaiMax)
        {
            if (string.IsNullOrWhiteSpace(duLieu))
            {
                return false;
            }

            string giaTri = duLieu.Trim();
            if (giaTri.Length < doDaiMin || giaTri.Length > doDaiMax)
            {
                return false;
            }

            foreach (char kyTu in giaTri)
            {
                if (kyTu < '0' || kyTu > '9')
                {
                    return false;
                }
            }

            return true;
        }

        private void LamMoiThongTinHoaDon(bool xoaGioHang)
        {
            if (xoaGioHang)
            {
                GioHangHienTai.Clear();
            }

            SDTKhachNhap = string.Empty;
            KhachHangDuocChon = null;
            NgayLapNhap = DateTime.Now;
            HinhThucThanhToanDangChon = DanhSachHinhThucThanhToan.FirstOrDefault();
            NhanVienLapDuocChon = DanhSachNhanVienLap.FirstOrDefault();
            CapNhatTienThanhToan();
        }

        private void CapNhatTienThanhToan()
        {
            OnPropertyChanged(nameof(TongTienHang));
            OnPropertyChanged(nameof(ThanhTienThanhToan));
        }
    }
}
