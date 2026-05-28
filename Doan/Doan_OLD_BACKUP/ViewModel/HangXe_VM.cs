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
    public class HangXe_VM : BaseViewModel
    {
        private ObservableCollection<Doan.Model.HangXe> danhSachHangXe;
        public ObservableCollection<Doan.Model.HangXe> DanhSachHangXe
        {
            get { return danhSachHangXe; }
            set
            {
                danhSachHangXe = value;
                OnPropertyChanged("DanhSachHangXe");
            }
        }

        private Doan.Model.HangXe hangXeDangChon;
        public Doan.Model.HangXe HangXeDangChon
        {
            get { return hangXeDangChon; }
            set
            {
                hangXeDangChon = value;
                OnPropertyChanged();
                if (hangXeDangChon != null)
                {
                    TenHangNhap = hangXeDangChon.TenHang;
                    QuocGiaNhap = hangXeDangChon.QuocGia;
                    LogoNhap = hangXeDangChon.LogoFullPath;
                }
                lenhMoSuaHangXe?.RaiseCanExecuteChanged();
                lenhXoaHangXe?.RaiseCanExecuteChanged();
            }
        }

        private string tenHangNhap;
        public string TenHangNhap
        {
            get { return tenHangNhap; }
            set
            {
                tenHangNhap = value;
                OnPropertyChanged();
            }
        }

        private string quocGiaNhap;
        public string QuocGiaNhap
        {
            get { return quocGiaNhap; }
            set
            {
                quocGiaNhap = value;
                OnPropertyChanged();
            }
        }

        private string logoNhap;
        public string LogoNhap
        {
            get { return logoNhap; }
            set
            {
                logoNhap = value;
                OnPropertyChanged();
            }
        }

        private bool dangSuaHangXe;

        private readonly RelayCommand lenhMoSuaHangXe;
        private readonly RelayCommand lenhXoaHangXe;

        public ICommand LenhMoDanhSachXeTheoHang { get; }
        public ICommand LenhMoThemHangXe { get; }
        public ICommand LenhMoSuaHangXe
        {
            get { return lenhMoSuaHangXe; }
        }
        public ICommand LenhXoaHangXe
        {
            get { return lenhXoaHangXe; }
        }
        public ICommand LenhLuuHangXe { get; }
        public ICommand LenhHuyFormHangXe { get; }

        public HangXe_VM()
        {
            LenhMoDanhSachXeTheoHang = new RelayCommand(
                parameter => MoDanhSachXeTheoHang(parameter as HangXe),
                parameter => parameter is HangXe);

            LenhMoThemHangXe = new RelayCommand(_ => MoThemHangXe());
            lenhMoSuaHangXe = new RelayCommand(_ => MoSuaHangXe(), _ => HangXeDangChon != null);
            lenhXoaHangXe = new RelayCommand(_ => XoaHangXe(), _ => HangXeDangChon != null);
            LenhLuuHangXe = new RelayCommand(parameter => LuuHangXe(parameter as Window));
            LenhHuyFormHangXe = new RelayCommand(parameter => DongFormHangXe(parameter as Window));

            // CHỈNH SỬA: Load dữ liệu từ Database thông qua hàm tĩnh
            LoadData();
            LamMoiNhapHangXe();
        }

        // CHỈNH SỬA: Hàm load dữ liệu thực tế từ DB
        private void LoadData()
        {
            DanhSachHangXe = DuLieuHeThong.LayDanhSachHangXe();
        }

        private void MoDanhSachXeTheoHang(HangXe hangXeDuocChon)
        {
            if (hangXeDuocChon == null) return;
            NavigationService.Navigate("DanhSachXeTheoHang", hangXeDuocChon);
        }

        private void MoThemHangXe()
        {
            dangSuaHangXe = false;
            LamMoiNhapHangXe();
            var cuaSoThemHangXe = new W_ThemHangXe();
            cuaSoThemHangXe.DataContext = this;
            cuaSoThemHangXe.ShowDialog();
        }

        private void MoSuaHangXe()
        {
            if (HangXeDangChon == null) return;
            dangSuaHangXe = true;
            TenHangNhap = HangXeDangChon.TenHang;
            QuocGiaNhap = HangXeDangChon.QuocGia;
            LogoNhap = HangXeDangChon.LogoFullPath;
            var cuaSoSuaHangXe = new W_SuaHangXe();
            cuaSoSuaHangXe.DataContext = this;
            cuaSoSuaHangXe.ShowDialog();
        }

        private void LuuHangXe(Window cuaSo)
        {
            try
            {
                if (!KiemTraDuLieuHangXe()) return;

                // CHỈNH SỬA: Gọi hàm lưu thực tế vào SQL (Thêm mới hoặc Cập nhật)
                // Tham số: Tên, Quốc gia, Logo, làThêmMới (true nếu thêm, false nếu sửa)
                DuLieuHeThong.LuuHangXeSQL(TenHangNhap.Trim(), QuocGiaNhap.Trim(), LogoNhap.Trim(), !dangSuaHangXe);

                MessageBox.Show(dangSuaHangXe ? "Cập nhật thành công!" : "Thêm mới thành công!");

                LoadData(); // Load lại danh sách từ DB để đồng bộ
                dangSuaHangXe = false;
                LamMoiNhapHangXe();
                cuaSo?.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu dữ liệu: " + ex.Message, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void XoaHangXe()
        {
            try
            {
                if (HangXeDangChon == null) return;

                var ketQua = MessageBox.Show($"Bạn có chắc muốn xóa hãng {HangXeDangChon.TenHang}?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (ketQua != MessageBoxResult.Yes) return;

                // CHỈNH SỬA: Gọi hàm xóa thực tế trong DB
                DuLieuHeThong.XoaHangXeSQL(HangXeDangChon.TenHang);

                LoadData(); // Load lại danh sách sau khi xóa
                HangXeDangChon = null;
                LamMoiNhapHangXe();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể xóa hãng xe (có thể đang có xe thuộc hãng này). \nChi tiết: " + ex.Message, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DongFormHangXe(Window cuaSo)
        {
            dangSuaHangXe = false;
            LamMoiNhapHangXe();
            cuaSo?.Close();
        }

        private bool KiemTraDuLieuHangXe()
        {
            if (string.IsNullOrWhiteSpace(TenHangNhap))
            {
                MessageBox.Show("Tên hãng xe không được để trống.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(QuocGiaNhap))
            {
                MessageBox.Show("Quốc gia không được để trống.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        private void LamMoiNhapHangXe()
        {
            TenHangNhap = string.Empty;
            QuocGiaNhap = string.Empty;
            LogoNhap = "https://via.placeholder.com/80";
        }

    }
}