using Doan.Helper;
using Doan.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Doan.View;
using System.Windows;

namespace Doan.ViewModel
{
    public class HangXe_VM : BaseViewModel
    {
        private ObservableCollection<HangXe> danhSachHangXe;
        public ObservableCollection<HangXe> DanhSachHangXe
        {
            get { return danhSachHangXe; }
            set
            {
                danhSachHangXe = value;
                OnPropertyChanged("DanhSachHangXe");
            }
        }

        private HangXe hangXeDangChon;
        public HangXe HangXeDangChon
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
                    LogoNhap = hangXeDangChon.LogoPath;
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

            TaiDanhSachHangXe();
            LamMoiNhapHangXe();
        }

        private void TaiDanhSachHangXe()
        {
            using (var ctx = new QuanLyBanXeMayEntities())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                DanhSachHangXe = new ObservableCollection<HangXe>(ctx.HangXes.ToList());
            }
        }

        private void MoDanhSachXeTheoHang(HangXe hangXeDuocChon)
        {
            if (hangXeDuocChon == null)
            {
                return;
            }

            DieuHuongTuMain("DanhSachXeTheoHang", hangXeDuocChon);
        }

        private void DieuHuongTuMain(string tenManHinh, object duLieu = null)
        {
            var mainVm = Application.Current?.MainWindow?.DataContext as MainWindows_VM;
            if (mainVm != null)
            {
                mainVm.DieuHuong(tenManHinh, duLieu);
            }
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
            if (HangXeDangChon == null)
            {
                return;
            }

            dangSuaHangXe = true;
            TenHangNhap = HangXeDangChon.TenHang;
            QuocGiaNhap = HangXeDangChon.QuocGia;
            LogoNhap = HangXeDangChon.LogoPath;
            var cuaSoSuaHangXe = new W_SuaHangXe();
            cuaSoSuaHangXe.DataContext = this;
            cuaSoSuaHangXe.ShowDialog();
        }

        private void LuuHangXe(Window cuaSo)
        {
            if (!KiemTraDuLieuHangXe())
            {
                return;
            }

            if (dangSuaHangXe && HangXeDangChon != null)
            {
                string tenHangNhapTrim = TenHangNhap.Trim();
                HangXe hangTrung = DanhSachHangXe.FirstOrDefault(item =>
                    item.MaHang != HangXeDangChon.MaHang &&
                    (item.TenHang ?? string.Empty).Trim() == tenHangNhapTrim);

                if (hangTrung != null)
                {
                    MessageBox.Show("Tên hãng xe đã tồn tại.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                using (var ctx = new QuanLyBanXeMayEntities())
                {
                    ctx.Configuration.LazyLoadingEnabled = false;
                    var efHang = ctx.HangXes.FirstOrDefault(h => h.MaHang == HangXeDangChon.MaHang);
                    if (efHang != null)
                    {
                        efHang.TenHang = TenHangNhap.Trim();
                        efHang.QuocGia = QuocGiaNhap.Trim();
                        efHang.LogoPath = LogoNhap.Trim();
                        ctx.SaveChanges();
                    }
                }

                TaiDanhSachHangXe();
            }
            else
            {
                string tenHangNhapTrim2 = TenHangNhap.Trim();
                HangXe hangTrung = DanhSachHangXe.FirstOrDefault(item =>
                    (item.TenHang ?? string.Empty).Trim() == tenHangNhapTrim2);

                if (hangTrung != null)
                {
                    MessageBox.Show("Tên hãng xe đã tồn tại.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                using (var ctx = new QuanLyBanXeMayEntities())
                {
                    ctx.Configuration.LazyLoadingEnabled = false;
                    string maMoi = TaoMaHangXeMoi(ctx);
                    var efHang = new HangXe
                    {
                        MaHang = maMoi,
                        TenHang = TenHangNhap.Trim(),
                        QuocGia = QuocGiaNhap.Trim(),
                        LogoPath = LogoNhap.Trim()
                    };
                    ctx.HangXes.Add(efHang);
                    ctx.SaveChanges();
                }

                TaiDanhSachHangXe();
            }

            dangSuaHangXe = false;
            LamMoiNhapHangXe();
            cuaSo?.Close();
        }

        private string TaoMaHangXeMoi(QuanLyBanXeMayEntities ctx)
        {
            int soLonNhat = 0;
            var tatCaMa = ctx.HangXes.Select(h => h.MaHang).ToList();
            foreach (string ma in tatCaMa)
            {
                if (string.IsNullOrWhiteSpace(ma))
                {
                    continue;
                }
                string so = ma.Trim().ToUpper().Replace("HX", string.Empty);
                int maSo;
                if (int.TryParse(so, out maSo) && maSo > soLonNhat)
                {
                    soLonNhat = maSo;
                }
            }
            return "HX" + (soLonNhat + 1).ToString("000");
        }

        private void XoaHangXe()
        {
            if (HangXeDangChon == null)
            {
                return;
            }

            var ketQua = MessageBox.Show("Bạn có chắc muốn xóa hãng xe đang chọn? Toàn bộ xe thuộc hãng này cũng sẽ bị xóa.", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (ketQua != MessageBoxResult.Yes)
            {
                return;
            }

            using (var ctx = new QuanLyBanXeMayEntities())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                var efHang = ctx.HangXes.FirstOrDefault(h => h.MaHang == HangXeDangChon.MaHang);
                if (efHang != null)
                {
                    var xeLienQuan = ctx.Xes.Where(x => x.MaHang == efHang.MaHang).ToList();
                    foreach (var xe in xeLienQuan)
                    {
                        ctx.Xes.Remove(xe);
                    }

                    ctx.HangXes.Remove(efHang);
                    ctx.SaveChanges();
                }
            }

            TaiDanhSachHangXe();
            HangXeDangChon = null;
            LamMoiNhapHangXe();
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

            if (TenHangNhap.Trim().Length < 2)
            {
                MessageBox.Show("Tên hãng xe phải có ít nhất 2 ký tự.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(QuocGiaNhap))
            {
                MessageBox.Show("Quốc gia không được để trống.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(LogoNhap))
            {
                LogoNhap = "https://upload.wikimedia.org/wikipedia/commons/thumb/a/ac/No_image_available.svg/320px-No_image_available.svg.png";
            }

            if (!LaUrlHopLe(LogoNhap.Trim()))
            {
                MessageBox.Show("Đường dẫn logo phải là URL hợp lệ (http/https) hoặc đường dẫn file ảnh có thật.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

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

        private void LamMoiNhapHangXe()
        {
            TenHangNhap = string.Empty;
            QuocGiaNhap = string.Empty;
            LogoNhap = "https://upload.wikimedia.org/wikipedia/commons/thumb/a/ac/No_image_available.svg/320px-No_image_available.svg.png";
        }
    }
}
