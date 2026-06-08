using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Doan.ViewModel;

namespace Doan.View
{
    /// <summary>
    /// Interaction logic for UC_DichVu.xaml
    /// </summary>
    public partial class UC_DichVu : UserControl
    {
        public UC_DichVu() : this(null) { }

        // loai: "Dịch vụ" / "Phụ tùng" để mở đúng mục; null = tất cả.
        public UC_DichVu(string loai)
        {
            InitializeComponent();
            DataContext = new DichVu_VM(loai);
        }
    }
}
