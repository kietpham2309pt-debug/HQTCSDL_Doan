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
    public partial class UC_DSXe : UserControl
    {
        public UC_DSXe()
        {
            InitializeComponent();
            DataContext = new Xe_VM();
        }

        public UC_DSXe(HangXe hangXeDuocChon)
        {
            InitializeComponent();
            DataContext = new Xe_VM(hangXeDuocChon);
        }
    }
}
