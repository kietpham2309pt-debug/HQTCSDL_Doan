using Doan.Helper;
using Doan.Model;
using Doan.View;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Configuration;

namespace Doan.ViewModel
{
    public class DangNhap_VM : BaseViewModel
    {
        // Thuộc tính binding cho UI
        private string _username;
        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        public ICommand LenhDangNhap { get; }
        public ICommand LenhMoDangKy { get; }

        private static readonly string connectionString = GetConnectionString();

        private static string GetConnectionString()
        {
            // Ưu tiên lấy từ App.config
            var cs = ConfigurationManager.ConnectionStrings["DL_OTO"]?.ConnectionString;

            if (string.IsNullOrWhiteSpace(cs))
            {
                // Fallback nếu không tìm thấy trong App.config
                cs = @"Data Source=LAPTOP-80MIEMQ9\SQLEXPRESS;Initial Catalog=DL_OTO;Integrated Security=True";
            }

            return cs;
        }
        public DangNhap_VM()
        {
            LenhDangNhap = new RelayCommand(thamSo => DangNhap(thamSo));
            LenhMoDangKy = new RelayCommand(thamSo => MoDangKy(thamSo as Window));
        }

        private void DangNhap(object thamSo)
        {
            // 2. Tên biến ở đây phải khớp với tên ở trên (thamSo chứ không phải thamso - chữ S viết hoa)
            var pBox = thamSo as System.Windows.Controls.PasswordBox;

            string matKhau = pBox?.Password;

            // Kiểm tra trống
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(matKhau))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ tài khoản và mật khẩu!");
                return;
            }

            // Kiểm tra với Database
                if (DuLieuHeThong.KiemTraDangNhap(Username, matKhau))
            {
                var cuaSoChinh = new Doan.View.MainWindow();
                cuaSoChinh.Show();

                // Tìm và đóng cửa sổ đăng nhập hiện tại
                var currentWindow = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w is W_DangNhap);
                currentWindow?.Close();
            }
            else
            {
                MessageBox.Show("Tài khoản hoặc mật khẩu không chính xác!");
            }
        }
        private void MoDangKy(Window cuaSoDangNhap)
        {
            var cuaSoDangKy = new W_DangKy();
            cuaSoDangKy.Show();

            if (cuaSoDangNhap == null)
            {
                cuaSoDangNhap = Application.Current.Windows.OfType<W_DangNhap>().FirstOrDefault();
            }

            cuaSoDangNhap?.Close();
        }
    }
}