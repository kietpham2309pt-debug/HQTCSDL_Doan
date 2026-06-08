using System.Windows;
using Doan.Helper;
using Doan.ViewModel;

namespace Doan.View
{
    public partial class W_ChiTietGiaoDich : Window
    {
        public W_ChiTietGiaoDich()
        {
            InitializeComponent();
        }

        private void In_Click(object sender, RoutedEventArgs e)
        {
            HoaDonPrinter.InGiaoDich(DataContext as GiaoDich_HienThi_VM);
        }

        private void Dong_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
