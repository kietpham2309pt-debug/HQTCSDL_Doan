using Doan.Helper;
using Doan.Model;
using Doan.Services;
using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace Doan.ViewModel
{
    public class KhachHang_VM : BaseViewModel
    {
        private readonly KhachHangRepository repository;
        private readonly RelayCommand lenhSuaKhachHang;
        private readonly RelayCommand lenhXoaKhachHang;

        private ObservableCollection<KhachHang> danhSachKhachHang;
        public ObservableCollection<KhachHang> DanhSachKhachHang
        {
            get { return danhSachKhachHang; }
            set
            {
                danhSachKhachHang = value;
                OnPropertyChanged();
            }
        }

        private KhachHang khachHangDangChon;
        public KhachHang KhachHangDangChon
        {
            get { return khachHangDangChon; }
            set
            {
                khachHangDangChon = value;
                OnPropertyChanged();

                if (khachHangDangChon != null)
                {
                    HoTenNhap = khachHangDangChon.HoTen;
                    GioiTinhNhap = khachHangDangChon.GioiTinh;
                    SoDienThoaiNhap = khachHangDangChon.SoDienThoai;
                    NgaySinhNhap = khachHangDangChon.NgaySinh;
                    EmailNhap = khachHangDangChon.Email;
                }

                lenhSuaKhachHang?.RaiseCanExecuteChanged();
                lenhXoaKhachHang?.RaiseCanExecuteChanged();
                OnPropertyChanged(nameof(CoKhachHangDuocChon));
            }
        }

        private string hoTenNhap;
        public string HoTenNhap
        {
            get { return hoTenNhap; }
            set { hoTenNhap = value; OnPropertyChanged(); }
        }

        private string gioiTinhNhap;
        public string GioiTinhNhap
        {
            get { return gioiTinhNhap; }
            set { gioiTinhNhap = value; OnPropertyChanged(); }
        }

        private string soDienThoaiNhap;
        public string SoDienThoaiNhap
        {
            get { return soDienThoaiNhap; }
            set { soDienThoaiNhap = value; OnPropertyChanged(); }
        }

        private DateTime? ngaySinhNhap;
        public DateTime? NgaySinhNhap
        {
            get { return ngaySinhNhap; }
            set { ngaySinhNhap = value; OnPropertyChanged(); }
        }

        private string emailNhap;
        public string EmailNhap
        {
            get { return emailNhap; }
            set { emailNhap = value; OnPropertyChanged(); }
        }

        private string tuKhoaSoDienThoai;
        public string TuKhoaSoDienThoai
        {
            get { return tuKhoaSoDienThoai; }
            set { tuKhoaSoDienThoai = value; OnPropertyChanged(); }
        }

        public ObservableCollection<string> DanhSachGioiTinh { get; }

        public bool CoKhachHangDuocChon
        {
            get { return KhachHangDangChon != null; }
        }

        public ICommand LenhThemKhachHang { get; }
        public ICommand LenhSuaKhachHang { get { return lenhSuaKhachHang; } }
        public ICommand LenhXoaKhachHang { get { return lenhXoaKhachHang; } }
        public ICommand LenhLamMoi { get; }
        public ICommand LenhTimKiem { get; }
        public ICommand LenhXoaTheoSoDienThoai { get; }

        public KhachHang_VM()
        {
            repository = new KhachHangRepository();
            DanhSachGioiTinh = new ObservableCollection<string> { "Nam", "Nữ" };

            LenhThemKhachHang = new RelayCommand(_ => ThemKhachHang());
            lenhSuaKhachHang = new RelayCommand(_ => SuaKhachHang(), _ => KhachHangDangChon != null);
            lenhXoaKhachHang = new RelayCommand(_ => XoaKhachHangTheoMa(), _ => KhachHangDangChon != null);
            LenhLamMoi = new RelayCommand(_ => LamMoi());
            LenhTimKiem = new RelayCommand(_ => TimKiem());
            LenhXoaTheoSoDienThoai = new RelayCommand(_ => XoaKhachHangTheoSoDienThoai());

            TaiDanhSachKhachHang();
            LamMoiFormNhap();
        }

        private void TaiDanhSachKhachHang()
        {
            try
            {
                DanhSachKhachHang = repository.LayTatCa();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể tải danh sách khách hàng: " + ex.Message, "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TimKiem()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(TuKhoaSoDienThoai))
                {
                    TaiDanhSachKhachHang();
                    return;
                }

                DanhSachKhachHang = repository.TimTheoSoDienThoai(TuKhoaSoDienThoai.Trim());
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tìm kiếm khách hàng: " + ex.Message, "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ThemKhachHang()
        {
            if (!KiemTraDuLieuNhap()) return;

            try
            {
                repository.Them(new KhachHang
                {
                    HoTen = HoTenNhap.Trim(),
                    GioiTinh = GioiTinhNhap.Trim(),
                    SoDienThoai = SoDienThoaiNhap.Trim(),
                    NgaySinh = NgaySinhNhap,
                    Email = string.IsNullOrWhiteSpace(EmailNhap) ? string.Empty : EmailNhap.Trim()
                });

                MessageBox.Show("Thêm khách hàng thành công.", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                TaiDanhSachKhachHang();
                LamMoiFormNhap();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm khách hàng: " + ex.Message, "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SuaKhachHang()
        {
            if (KhachHangDangChon == null) return;
            if (!KiemTraDuLieuNhap()) return;

            try
            {
                repository.SuaTheoSoDienThoai(KhachHangDangChon.SoDienThoai, new KhachHang
                {
                    HoTen = HoTenNhap.Trim(),
                    GioiTinh = GioiTinhNhap.Trim(),
                    NgaySinh = NgaySinhNhap,
                    Email = string.IsNullOrWhiteSpace(EmailNhap) ? string.Empty : EmailNhap.Trim()
                });

                MessageBox.Show("Cập nhật khách hàng thành công.", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                TaiDanhSachKhachHang();
                LamMoiFormNhap();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi sửa khách hàng: " + ex.Message, "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void XoaKhachHangTheoMa()
        {
            if (KhachHangDangChon == null) return;

            var xacNhan = MessageBox.Show("Bạn có chắc muốn xóa khách hàng đang chọn?", "Xác nhận",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (xacNhan != MessageBoxResult.Yes) return;

            try
            {
                repository.XoaTheoMaKhachHang(KhachHangDangChon.MaKH);

                MessageBox.Show("Xóa khách hàng thành công.", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                TaiDanhSachKhachHang();
                LamMoiFormNhap();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xóa khách hàng: " + ex.Message, "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void XoaKhachHangTheoSoDienThoai()
        {
            if (string.IsNullOrWhiteSpace(TuKhoaSoDienThoai))
            {
                MessageBox.Show("Vui lòng nhập số điện thoại ở ô tìm kiếm để xóa theo SĐT.", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var xacNhan = MessageBox.Show("Bạn có chắc muốn xóa khách hàng theo số điện thoại đã nhập?", "Xác nhận",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (xacNhan != MessageBoxResult.Yes) return;

            try
            {
                repository.XoaTheoSoDienThoai(TuKhoaSoDienThoai.Trim());

                MessageBox.Show("Xóa khách hàng theo số điện thoại thành công.", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                TaiDanhSachKhachHang();
                LamMoiFormNhap();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xóa khách hàng theo số điện thoại: " + ex.Message, "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool KiemTraDuLieuNhap()
        {
            if (string.IsNullOrWhiteSpace(HoTenNhap))
            {
                MessageBox.Show("Họ tên không được để trống.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(GioiTinhNhap))
            {
                MessageBox.Show("Vui lòng chọn giới tính.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(SoDienThoaiNhap))
            {
                MessageBox.Show("Số điện thoại không được để trống.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!Regex.IsMatch(SoDienThoaiNhap.Trim(), @"^0\d{9,10}$"))
            {
                MessageBox.Show("Số điện thoại không hợp lệ. Ví dụ: 0912345678", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!string.IsNullOrWhiteSpace(EmailNhap) &&
                !Regex.IsMatch(EmailNhap.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Email không đúng định dạng.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void LamMoi()
        {
            TaiDanhSachKhachHang();
            LamMoiFormNhap();
        }

        private void LamMoiFormNhap()
        {
            HoTenNhap = string.Empty;
            GioiTinhNhap = null;
            SoDienThoaiNhap = string.Empty;
            NgaySinhNhap = null;
            EmailNhap = string.Empty;
            TuKhoaSoDienThoai = string.Empty;
            KhachHangDangChon = null;

            lenhSuaKhachHang.RaiseCanExecuteChanged();
            lenhXoaKhachHang.RaiseCanExecuteChanged();
            OnPropertyChanged(nameof(CoKhachHangDuocChon));
        }
    }
}
