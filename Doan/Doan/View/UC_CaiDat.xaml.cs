using Doan.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace Doan.View
{
    /// <summary>
    /// Interaction logic for UC_CaiDat.xaml
    /// </summary>
    public partial class UC_CaiDat : UserControl
    {
        public UC_CaiDat()
        {
            InitializeComponent();
        }

        private void DoiMatKhau_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as CaiDat_VM;
            if (vm == null)
            {
                return;
            }

            bool ok = vm.DoiMatKhau(MatKhauCuBox.Password, MatKhauMoiBox.Password, XacNhanBox.Password);
            if (ok)
            {
                MatKhauCuBox.Clear();
                MatKhauMoiBox.Clear();
                XacNhanBox.Clear();
            }
        }
    }
}
