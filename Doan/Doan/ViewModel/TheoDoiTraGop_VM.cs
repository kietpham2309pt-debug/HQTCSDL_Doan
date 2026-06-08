using Doan.Helper;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Doan.ViewModel
{
    public class TheoDoiTraGop_VM : BaseViewModel
    {
        private ObservableCollection<TheoDoiTraGop_DTO> danhSach;
        public ObservableCollection<TheoDoiTraGop_DTO> DanhSach
        {
            get { return danhSach; }
            set { danhSach = value; OnPropertyChanged(); }
        }

        private TheoDoiTraGop_DTO phieuDangChon;
        public TheoDoiTraGop_DTO PhieuDangChon
        {
            get { return phieuDangChon; }
            set
            {
                phieuDangChon = value;
                OnPropertyChanged();
                if (phieuDangChon != null)
                {
                    SoTienGhiNhan = string.Format("{0:N0}", (long)(phieuDangChon.SoTienMoiKy ?? 0)).Replace(",", ".");
                }
                lenhGhiNhan?.RaiseCanExecuteChanged();
                lenhTatToan?.RaiseCanExecuteChanged();
            }
        }

        private string soTienGhiNhan = "0";
        public string SoTienGhiNhan
        {
            get { return soTienGhiNhan; }
            set { soTienGhiNhan = value; OnPropertyChanged(); }
        }

        private string ghiChu;
        public string GhiChu
        {
            get { return ghiChu; }
            set { ghiChu = value; OnPropertyChanged(); }
        }

        public long TongConLai
        {
            get { return DanhSach == null ? 0 : (long)DanhSach.Where(d => d.TrangThai == "Đang trả").Sum(d => d.ConLai ?? 0); }
        }

        public int SoPhieuDangTra
        {
            get { return DanhSach == null ? 0 : DanhSach.Count(d => d.TrangThai == "Đang trả"); }
        }

        private readonly RelayCommand lenhGhiNhan;
        private readonly RelayCommand lenhTatToan;
        public ICommand LenhTaiLai { get; }
        public ICommand LenhGhiNhan { get { return lenhGhiNhan; } }
        public ICommand LenhTatToan { get { return lenhTatToan; } }

        public TheoDoiTraGop_VM()
        {
            LenhTaiLai = new RelayCommand(_ => TaiDanhSach());
            lenhGhiNhan = new RelayCommand(_ => GhiNhan(), _ => PhieuDangChon != null);
            lenhTatToan = new RelayCommand(_ => TatToan(), _ => PhieuDangChon != null);
            DanhSach = new ObservableCollection<TheoDoiTraGop_DTO>();
            TaiDanhSach();
        }

        private void TaiDanhSach()
        {
            string maGiu = PhieuDangChon != null ? PhieuDangChon.MaTraGop : null;
            DanhSach = new ObservableCollection<TheoDoiTraGop_DTO>(TraGopRepository.LayTheoDoi());
            OnPropertyChanged(nameof(TongConLai));
            OnPropertyChanged(nameof(SoPhieuDangTra));
            if (maGiu != null)
            {
                PhieuDangChon = DanhSach.FirstOrDefault(d => d.MaTraGop == maGiu);
            }
        }

        private decimal? DocSoTien()
        {
            string raw = (SoTienGhiNhan ?? string.Empty).Replace(".", string.Empty).Replace(",", string.Empty).Trim();
            decimal st;
            if (!decimal.TryParse(raw, out st) || st <= 0)
            {
                MessageBox.Show("Số tiền ghi nhận không hợp lệ.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }
            return st;
        }

        private void GhiNhan()
        {
            if (PhieuDangChon == null) return;
            if (PhieuDangChon.TrangThai == "Đã tất toán")
            {
                MessageBox.Show("Phiếu này đã tất toán.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var st = DocSoTien();
            if (st == null) return;

            decimal conLai = PhieuDangChon.ConLai ?? 0;
            if (st.Value > conLai) st = conLai; // không thu quá số còn lại

            try
            {
                TraGopRepository.GhiNhan(PhieuDangChon.MaTraGop, st.Value, GhiChu);
                MessageBox.Show("Đã ghi nhận thanh toán " + string.Format("{0:N0}", st.Value) + " đ.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                GhiChu = string.Empty;
                TaiDanhSach();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ghi nhận thất bại: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TatToan()
        {
            if (PhieuDangChon == null) return;
            decimal conLai = PhieuDangChon.ConLai ?? 0;
            if (conLai <= 0)
            {
                MessageBox.Show("Phiếu này đã tất toán.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var kq = MessageBox.Show("Tất toán toàn bộ số còn lại " + string.Format("{0:N0}", conLai) + " đ?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (kq != MessageBoxResult.Yes) return;

            try
            {
                TraGopRepository.GhiNhan(PhieuDangChon.MaTraGop, conLai, "Tất toán");
                MessageBox.Show("Đã tất toán phiếu trả góp.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                TaiDanhSach();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Tất toán thất bại: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
