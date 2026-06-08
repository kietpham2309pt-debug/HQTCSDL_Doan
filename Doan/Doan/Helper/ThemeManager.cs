using System;
using System.IO;
using System.Windows;

namespace Doan.Helper
{
    // Quản lý theme Sáng/Tối: hoán đổi bảng màu (palette) lúc chạy và lưu lựa chọn.
    public static class ThemeManager
    {
        public const string Sang = "Light";
        public const string Toi = "Dark";

        public static string ThemeHienTai { get; private set; } = Sang;

        private static string ThuMucLuu
        {
            get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DoanOto"); }
        }

        private static string FileLuu
        {
            get { return Path.Combine(ThuMucLuu, "theme.txt"); }
        }

        // Gọi khi khởi động app: đọc lựa chọn đã lưu và áp dụng.
        public static void KhoiTao()
        {
            ApDung(DocLuaChon(), luu: false);
        }

        // Áp dụng theme theo tên ("Light"/"Dark").
        public static void ApDung(string ten, bool luu = true)
        {
            ten = string.Equals(ten, Toi, StringComparison.OrdinalIgnoreCase) ? Toi : Sang;

            var paletteMoi = new ResourceDictionary
            {
                Source = new Uri("pack://application:,,,/Themes/Palette." + ten + ".xaml", UriKind.Absolute)
            };

            var dicts = Application.Current.Resources.MergedDictionaries;

            // Gỡ palette cũ (nhận diện bằng khóa "TenTheme").
            for (int i = dicts.Count - 1; i >= 0; i--)
            {
                if (dicts[i].Contains("TenTheme"))
                {
                    dicts.RemoveAt(i);
                }
            }

            // Thêm palette mới ở đầu để Controls.xaml (DynamicResource) phân giải đúng.
            dicts.Insert(0, paletteMoi);

            ThemeHienTai = ten;
            if (luu)
            {
                LuuLuaChon(ten);
            }
        }

        // Đảo Sáng <-> Tối.
        public static void DaoTheme()
        {
            ApDung(ThemeHienTai == Toi ? Sang : Toi);
        }

        private static string DocLuaChon()
        {
            try
            {
                if (File.Exists(FileLuu))
                {
                    string noiDung = File.ReadAllText(FileLuu).Trim();
                    if (string.Equals(noiDung, Toi, StringComparison.OrdinalIgnoreCase))
                    {
                        return Toi;
                    }
                }
            }
            catch { /* bỏ qua, dùng mặc định */ }
            return Sang;
        }

        private static void LuuLuaChon(string ten)
        {
            try
            {
                Directory.CreateDirectory(ThuMucLuu);
                File.WriteAllText(FileLuu, ten);
            }
            catch { /* không lưu được cũng không sao */ }
        }
    }
}
