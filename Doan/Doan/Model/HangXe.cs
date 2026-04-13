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
    public class HangXe
    {
        private string tenHang;
        private string quocGia;
        private string logoFullPath;
        public string TenHang
        {
            get { return tenHang; }
            set { tenHang = value; }
        }
        public string QuocGia
        {
            get { return quocGia; }
            set { quocGia = value; }
        }
        public string LogoFullPath
        {
            get { return logoFullPath; }
            set { logoFullPath = value; }
        }

        public HangXe() { }
        public HangXe(string tenHang, string quocGia, string logoFullPath)
        {
            TenHang = tenHang;
            QuocGia = quocGia;
            LogoFullPath = logoFullPath;
        }
        public ImageSource LogoDisplay
        {
            get
            {
                if (string.IsNullOrEmpty(LogoFullPath)) return null;

                try
                {
                    // Trường hợp 1: Nếu là chuỗi Base64
                    if (LogoFullPath.Contains("base64,"))
                    {
                        string base64String = LogoFullPath.Split(',')[1];
                        byte[] binaryData = Convert.FromBase64String(base64String);
                        BitmapImage bi = new BitmapImage();
                        bi.BeginInit();
                        bi.StreamSource = new MemoryStream(binaryData);
                        bi.EndInit();
                        return bi;
                    }

                    // Trường hợp 2: Nếu là URL hoặc đường dẫn file
                    return new BitmapImage(new Uri(LogoFullPath, UriKind.RelativeOrAbsolute));
                }
                catch
                {
                    return null; // Hoặc trả về một ảnh lỗi mặc định
                }
            }
        }
    }
}
