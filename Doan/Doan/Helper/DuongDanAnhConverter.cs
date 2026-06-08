using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Doan.Helper
{
    /// <summary>
    /// Chuyển giá trị HinhAnh (URL http/https hoặc đường dẫn file) thành ảnh hiển thị.
    /// Nếu đường dẫn file không tồn tại (ví dụ dữ liệu seed lưu đường dẫn tuyệt đối của máy khác),
    /// converter sẽ tự tìm file theo tên trong thư mục images\cars trên máy hiện tại.
    /// </summary>
    public class DuongDanAnhConverter : IValueConverter
    {
        private const string AnhMacDinhUrl =
            "https://upload.wikimedia.org/wikipedia/commons/thumb/a/ac/No_image_available.svg/320px-No_image_available.svg.png";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string duongDan = value as string;
            if (string.IsNullOrWhiteSpace(duongDan))
            {
                return TaoBitmap(AnhMacDinhUrl);
            }

            duongDan = duongDan.Trim();

            // 1) URL http/https -> dùng trực tiếp.
            Uri uri;
            if (Uri.TryCreate(duongDan, UriKind.Absolute, out uri)
                && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
            {
                return TaoBitmap(duongDan);
            }

            // 2) File tồn tại đúng theo đường dẫn đã lưu.
            if (File.Exists(duongDan))
            {
                return TaoBitmap(duongDan);
            }

            // 3) Tìm theo tên file trong thư mục images\cars trên máy hiện tại.
            string tenFile = LayTenFile(duongDan);
            if (!string.IsNullOrEmpty(tenFile))
            {
                string ungVien = TimTrongThuMucAnh(tenFile);
                if (ungVien != null)
                {
                    return TaoBitmap(ungVien);
                }
            }

            return TaoBitmap(AnhMacDinhUrl);
        }

        private static string LayTenFile(string duongDan)
        {
            try
            {
                return Path.GetFileName(duongDan);
            }
            catch
            {
                return null;
            }
        }

        private static string TimTrongThuMucAnh(string tenFile)
        {
            // Đi ngược từ thư mục chạy (bin\Debug) lên trên để tìm images\cars\<tenFile>.
            DirectoryInfo thuMuc = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            for (int i = 0; i < 6 && thuMuc != null; i++)
            {
                string ungVien = Path.Combine(thuMuc.FullName, "images", "cars", tenFile);
                if (File.Exists(ungVien))
                {
                    return ungVien;
                }
                thuMuc = thuMuc.Parent;
            }
            return null;
        }

        private static BitmapImage TaoBitmap(string nguon)
        {
            try
            {
                var bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.UriSource = new Uri(nguon, UriKind.Absolute);
                bmp.EndInit();
                bmp.Freeze();
                return bmp;
            }
            catch
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
