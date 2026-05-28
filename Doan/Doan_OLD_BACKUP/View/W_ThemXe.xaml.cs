using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Doan.View
{
    /// <summary>
    /// Interaction logic for W_ThemXe.xaml
    /// </summary>
    public partial class W_ThemXe : Window
    {
        public W_ThemXe()
        {
            InitializeComponent();
        }

        private void ChonHinhAnh_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog hopThoai = new OpenFileDialog
            {
                Title = "Chọn hình ảnh xe",
                Filter = "Tệp ảnh|*.jpg;*.jpeg;*.png;*.webp;*.bmp|Tất cả tệp|*.*"
            };

            if (hopThoai.ShowDialog() == true)
            {
                HinhAnhTextBox.Text = hopThoai.FileName;
            }
        }
    }
}
