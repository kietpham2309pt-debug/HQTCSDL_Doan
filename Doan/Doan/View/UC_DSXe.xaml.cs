using Doan.Model;
using Doan.ViewModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Doan.View
{
    /// <summary>
    /// Interaction logic for UC_DSXe.xaml
    /// </summary>
    public partial class UC_DSXe : UserControl
    {
        public UC_DSXe()
        {
            InitializeComponent();
            if (DataContext == null)
            {
                DataContext = new Xe_VM();
            }
        }

        public UC_DSXe(HangXe hangXeDuocChon) : this()
        {
            DataContext = new Xe_VM(hangXeDuocChon);
        }

        public UC_DSXe(string brandName, string brandCountry) : this()
        {
            var hangXe = new HangXe
            {
                TenHang = brandName,
                QuocGia = brandCountry
            };
            DataContext = new Xe_VM(hangXe);
        }
    }
}
