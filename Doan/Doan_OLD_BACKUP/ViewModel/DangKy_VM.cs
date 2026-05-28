using Doan.Helper;
using Doan.View;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Doan.ViewModel
{
    public class DangKy_VM : BaseViewModel
    {
        public ICommand LenhDangKy { get; }
        public ICommand LenhMoDangNhap { get; }

        public DangKy_VM()
        {
            LenhDangKy = new RelayCommand(thamSo => DangKy(thamSo as Window));
            LenhMoDangNhap = new RelayCommand(thamSo => MoDangNhap(thamSo as Window));
        }

        private void DangKy(Window cuaSoDangKy)
        {
            MessageBox.Show("Đăng ký thành công (mô phỏng).", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            var cuaSoDangNhap = new W_DangNhap();
            cuaSoDangNhap.Show();

            if (cuaSoDangKy == null)
            {
                cuaSoDangKy = Application.Current.Windows.OfType<W_DangKy>().FirstOrDefault();
            }

            cuaSoDangKy?.Close();
        }

        private void MoDangNhap(Window cuaSoDangKy)
        {
            var cuaSoDangNhap = new W_DangNhap();
            cuaSoDangNhap.Show();

            if (cuaSoDangKy == null)
            {
                cuaSoDangKy = Application.Current.Windows.OfType<W_DangKy>().FirstOrDefault();
            }

            cuaSoDangKy?.Close();
        }
    }
}
