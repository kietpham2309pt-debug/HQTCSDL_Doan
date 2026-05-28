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
    /// Interaction logic for UC_HoaDon.xaml
    /// </summary>
    public partial class UC_HoaDon : UserControl
    {
        public UC_HoaDon()
        {
            InitializeComponent();
            this.DataContext = new HoaDon_VM();
        }
    }
}
