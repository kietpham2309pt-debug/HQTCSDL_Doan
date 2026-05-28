using Doan.ViewModel;
using System.Windows.Controls;

namespace Doan.View
{
    /// <summary>
    /// Interaction logic for UC_KhachHang.xaml
    /// </summary>
    public partial class UC_KhachHang : UserControl
    {
        public UC_KhachHang()
        {
            InitializeComponent();
            DataContext = new KhachHang_VM();
        }
    }
}
