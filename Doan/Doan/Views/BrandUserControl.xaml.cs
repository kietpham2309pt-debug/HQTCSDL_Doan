using Doan.Services;
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
using Doan.Models;
namespace Doan.Views
{
    /// <summary>
    /// Interaction logic for BrandUserControl.xaml
    /// </summary>
    public partial class BrandUserControl : UserControl
    {
        private DatabaseService _dbService;
        public BrandUserControl()
        {
            InitializeComponent();
            _dbService = new DatabaseService();
            LoadBrands();
        }

        private void LoadBrands()
        {
            try
            {
                var brands = _dbService.GetAllHangXe();
                this.DataContext = new { HangXeList = brands };
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BrandItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Border border = sender as Border;
            HangXe brand = border?.DataContext as HangXe;

            if (brand != null)
            {
                // Load CarUserControl
                CarUserControl carControl = new CarUserControl(brand.Id, brand.TenHang);
                Window parentWindow = Window.GetWindow(this);

                if (parentWindow is MainWindow mainWindow)
                {
                    mainWindow.MainContentControl.Content = carControl;
                }
            }
        }
    }
}
