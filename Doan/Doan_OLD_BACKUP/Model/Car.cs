using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Doan.Model
{
    public class Car
    {
        private string tenHang;
        private string tenDongXe;
        private string loaiXe;
        private int giaXe;
        private string hinhAnhFullPath;
        private string moTa;
        private string mauSac;
        private int namSX;
        private int soLuongTon;

        public string TenHang
        {
            get { return tenHang; }
            set { tenHang = value; }
        }

        public string TenDongXe
        {
            get { return tenDongXe; }
            set { tenDongXe = value; }
        }
        public string LoaiXe
        {
            get { return loaiXe; }
            set { loaiXe = value; }
        }
        public int GiaXe
        {
            get { return giaXe; }
            set { giaXe = value; }
        }
        public string HinhAnhFullPath
        {
            get { return hinhAnhFullPath; }
            set { hinhAnhFullPath = value; }
        }
        public string MoTa
        {
            get { return moTa; }
            set { moTa = value; }
        }
        public string MauSac
        {
            get { return mauSac; }
            set { mauSac = value; }
        }
        public int NamSX
        {
            get { return namSX; }
            set { namSX = value; }
        }
        public int SoLuongTon
        {
            get { return soLuongTon; }
            set { soLuongTon = value; }
        }

        public Car() { }
        public Car(string tenDongXe, string loaiXe, int namSX, int giaXe, string hinhAnhFullPath, string moTa)
        {
            TenDongXe = tenDongXe;
            LoaiXe = loaiXe;
            NamSX = namSX;
            GiaXe = giaXe;
            HinhAnhFullPath = hinhAnhFullPath;
            MoTa = moTa;
        }

        public Car(string tenHang, string tenDongXe, string loaiXe, string mauSac, int namSX, int giaXe, string hinhAnhFullPath, string moTa, int soLuongTon)
        {
            TenHang = tenHang;
            TenDongXe = tenDongXe;
            LoaiXe = loaiXe;
            MauSac = mauSac;
            NamSX = namSX;
            GiaXe = giaXe;
            HinhAnhFullPath = hinhAnhFullPath;
            MoTa = moTa;
            SoLuongTon = soLuongTon;
        }
        public ImageSource HinhAnhDisplay
        {
            get
            {
                if (string.IsNullOrEmpty(HinhAnhFullPath)) return null;

                try
                {
                    // Trường hợp 1: Nếu là chuỗi Base64
                    if (HinhAnhFullPath.Contains("base64,"))
                    {
                        string base64String = HinhAnhFullPath.Split(',')[1];
                        byte[] binaryData = Convert.FromBase64String(base64String);
                        BitmapImage bi = new BitmapImage();
                        bi.BeginInit();
                        bi.StreamSource = new MemoryStream(binaryData);
                        bi.EndInit();
                        return bi;
                    }

                    // Trường hợp 2: Nếu là URL hoặc đường dẫn file
                    return new BitmapImage(new Uri(HinhAnhFullPath, UriKind.RelativeOrAbsolute));
                }
                catch
                {
                    return null; // Hoặc trả về một ảnh lỗi mặc định
                }
            }
        }
    }
}
