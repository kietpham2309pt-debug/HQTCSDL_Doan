using Doan.Helper;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Doan.ViewModel
{
    public class LichSuNhap_VM : BaseViewModel
    {
        private ObservableCollection<DongLichSuNhap> danhSach;
        public ObservableCollection<DongLichSuNhap> DanhSach
        {
            get { return danhSach; }
            set { danhSach = value; OnPropertyChanged(); }
        }

        private string tuKhoa;
        public string TuKhoa
        {
            get { return tuKhoa; }
            set { tuKhoa = value; OnPropertyChanged(); }
        }

        public long TongChiNhap
        {
            get { return DanhSach == null ? 0 : (long)DanhSach.Sum(d => d.ThanhTien ?? 0); }
        }

        public int SoDong
        {
            get { return DanhSach?.Count ?? 0; }
        }

        public int SoPhieu
        {
            get { return DanhSach == null ? 0 : DanhSach.Select(d => d.MaPN).Distinct().Count(); }
        }

        public long ChiNhapXe
        {
            get { return DanhSach == null ? 0 : (long)DanhSach.Where(d => d.LoaiMatHang == "Xe").Sum(d => d.ThanhTien ?? 0); }
        }

        public long ChiNhapPhuTung
        {
            get { return DanhSach == null ? 0 : (long)DanhSach.Where(d => d.LoaiMatHang == "PhuTung").Sum(d => d.ThanhTien ?? 0); }
        }

        public ICommand LenhTaiLai { get; }
        public ICommand LenhTimKiem { get; }

        public LichSuNhap_VM()
        {
            LenhTaiLai = new RelayCommand(_ => TaiDanhSach());
            LenhTimKiem = new RelayCommand(_ => TaiDanhSach());
            TaiDanhSach();
        }

        private void TaiDanhSach()
        {
            var all = NhapKhoRepository.LayLichSuNhap();

            if (!string.IsNullOrWhiteSpace(TuKhoa))
            {
                string k = TuKhoa.Trim().ToLower();
                all = all.Where(d =>
                    (d.MaPN ?? string.Empty).ToLower().Contains(k) ||
                    (d.NhaCungCap ?? string.Empty).ToLower().Contains(k) ||
                    (d.TenMatHang ?? string.Empty).ToLower().Contains(k) ||
                    (d.TenNhanVien ?? string.Empty).ToLower().Contains(k)).ToList();
            }

            DanhSach = new ObservableCollection<DongLichSuNhap>(all);
            OnPropertyChanged(nameof(TongChiNhap));
            OnPropertyChanged(nameof(SoDong));
            OnPropertyChanged(nameof(SoPhieu));
            OnPropertyChanged(nameof(ChiNhapXe));
            OnPropertyChanged(nameof(ChiNhapPhuTung));
        }
    }
}
