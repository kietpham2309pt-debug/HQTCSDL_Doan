using Doan.Helper;
using System;

namespace Doan.ViewModel
{
    public class MatHangGio_VM : BaseViewModel
    {
        private string maMatHang;
        public string MaMatHang
        {
            get { return maMatHang; }
            set
            {
                maMatHang = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(LaPhuTung));
                OnPropertyChanged(nameof(LaDichVu));
            }
        }

        private string tenMatHang;
        public string TenMatHang
        {
            get { return tenMatHang; }
            set
            {
                tenMatHang = value;
                OnPropertyChanged();
            }
        }

        // Giá xe ô tô có thể lên tới vài tỷ đồng, vượt giới hạn của kiểu int
        // (khoảng 2,1 tỷ) nên phải dùng long để không bị tràn số.
        private long donGia;
        public long DonGia
        {
            get { return donGia; }
            set
            {
                donGia = value < 0 ? 0 : value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ThanhTien));
            }
        }

        private int soLuong;
        public int SoLuong
        {
            get { return soLuong; }
            set
            {
                soLuong = value < 0 ? 0 : value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ThanhTien));
            }
        }

        public long ThanhTien
        {
            get { return DonGia * SoLuong; }
        }

        public bool LaPhuTung
        {
            get
            {
                return (MaMatHang ?? string.Empty).Trim().StartsWith("PT", StringComparison.OrdinalIgnoreCase);
            }
        }

        public bool LaDichVu
        {
            get
            {
                return (MaMatHang ?? string.Empty).Trim().StartsWith("DV", StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}
