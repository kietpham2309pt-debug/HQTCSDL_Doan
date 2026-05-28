using System.Windows;

namespace Doan.View
{
    public partial class MainWindow_User : Window
    {
        public MainWindow_User()
        {
            InitializeComponent();

            // Tương tự MainWindow: gán lại để cửa sổ này là MainWindow hiện hành sau đăng nhập.
            Application.Current.MainWindow = this;
        }
    }
}
