using Doan.Services;
using Doan.Models;
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
using System.Collections.ObjectModel;
namespace Doan.Views
{
    /// <summary>
    /// Interaction logic for CarUserControl.xaml
    /// </summary>
    public partial class CarUserControl : UserControl
    {
        private int _idHang;
        private string _tenHang;
        private DatabaseService _dbService;
        private ObservableCollection<Xe> _xeList;

        public CarUserControl(int idHang, string tenHang)
        {
            InitializeComponent();
            _idHang = idHang;
            _tenHang = tenHang;
            _dbService = new DatabaseService();

            TitleTextBlock.Text = $"Danh sách xe - {_tenHang}";
            LoadCars();
        }

        private void LoadCars()
        {
            try
            {
                var allCars = _dbService.GetXeByHang(_idHang);

                // 👉 GROUP theo dòng xe (IdDongXe)
                _xeList = new ObservableCollection<Xe>(
                    allCars.GroupBy(x => x.IdDongXe)
                           .Select(g => g.First()) // lấy 1 xe đại diện
                );

                this.DataContext = new { XeList = _xeList };
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}");
            }
        }

        private void CarItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Border border = sender as Border;
            Xe car = border?.DataContext as Xe;

            if (car != null)
            {
                DisplayCarDetail(car);
            }
        }

        private void DisplayCarDetail(Xe car)
        {
            DetailPanel.Children.Clear();

            try
            {
                // Ảnh xe lớn
                string fullPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory,car.HinhAnh);

                Image carImage = new Image
                {
                    Source = new BitmapImage(new Uri(fullPath, UriKind.Absolute)),
                    Height = 180,
                    Stretch = Stretch.Uniform,
                    Margin = new Thickness(0, 0, 0, 20)
                };
                DetailPanel.Children.Add(carImage);

                // Tên dòng xe
                TextBlock nameBlock = new TextBlock
                {
                    Text = car.TenDong,
                    FontSize = 18,
                    FontWeight = FontWeights.Bold,
                    Foreground = new SolidColorBrush(Color.FromRgb(44, 62, 80)),
                    Margin = new Thickness(0, 0, 0, 15)
                };
                DetailPanel.Children.Add(nameBlock);

                // Năm sản xuất
                TextBlock yearBlock = new TextBlock
                {
                    Text = $"Năm: {car.NamSanXuat}",
                    FontSize = 12,
                    Foreground = new SolidColorBrush(Color.FromRgb(149, 165, 166)),
                    Margin = new Thickness(0, 0, 0, 10)
                };
                DetailPanel.Children.Add(yearBlock);

                // Giá
                TextBlock priceBlock = new TextBlock
                {
                    Text = $"Giá: {car.GiaBan:N0} VND",
                    FontSize = 14,
                    FontWeight = FontWeights.Bold,
                    Foreground = new SolidColorBrush(Color.FromRgb(231, 76, 60)),
                    Margin = new Thickness(0, 0, 0, 20)
                };
                DetailPanel.Children.Add(priceBlock);

                // Tiêu đề màu sắc
                TextBlock colorTitle = new TextBlock
                {
                    Text = "CHỌN MÀU SẮC",
                    FontSize = 12,
                    FontWeight = FontWeights.Bold,
                    Foreground = new SolidColorBrush(Color.FromRgb(44, 62, 80)),
                    Margin = new Thickness(0, 0, 0, 12)
                };
                DetailPanel.Children.Add(colorTitle);

                // Danh sách màu sắc
                var colors = _dbService.GetMauSacByDongXe(car.IdDongXe);

                WrapPanel colorPanel = new WrapPanel
                {
                    Margin = new Thickness(0, 0, 0, 20)
                };

                foreach (var color in colors)
                {
                    // Button màu sắc
                    Button colorButton = new Button
                    {
                        Width = 50,
                        Height = 50,
                        Margin = new Thickness(0, 0, 10, 10),
                        Background = GetColorBrush(color),
                        Cursor = Cursors.Hand,
                        ToolTip = color
                    };

                    colorButton.Click += (s, e) =>
                    {
                        var xeTheoMau = _dbService
                            .GetXeByHang(_idHang)
                            .FirstOrDefault(x => x.IdDongXe == car.IdDongXe && x.MauSac == color);

                        if (xeTheoMau != null)
                        {
                            string path = System.IO.Path.Combine(
                                AppDomain.CurrentDomain.BaseDirectory,
                                xeTheoMau.HinhAnh
                            );

                            carImage.Source = new BitmapImage(new Uri(path));
                        }
                    };

                    colorPanel.Children.Add(colorButton);
                }

                DetailPanel.Children.Add(colorPanel);

                // Số lượng tồn kho
                TextBlock stockBlock = new TextBlock
                {
                    Text = $"Tồn kho: {car.SoLuong} chiếc",
                    FontSize = 12,
                    Foreground = new SolidColorBrush(Color.FromRgb(46, 204, 113)),
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 0, 0, 15)
                };
                DetailPanel.Children.Add(stockBlock);

                // Mô tả
                if (!string.IsNullOrEmpty(car.MoTa))
                {
                    TextBlock descBlock = new TextBlock
                    {
                        Text = car.MoTa,
                        FontSize = 11,
                        Foreground = new SolidColorBrush(Color.FromRgb(127, 140, 141)),
                        TextWrapping = TextWrapping.Wrap,
                        Margin = new Thickness(0, 0, 0, 15)
                    };
                    DetailPanel.Children.Add(descBlock);
                }

                // Nút Mua
                Button buyButton = new Button
                {
                    Content = "MUA NGAY",
                    Background = new SolidColorBrush(Color.FromRgb(231, 76, 60)),
                    Foreground = Brushes.White,
                    Padding = new Thickness(15, 10, 15, 10),
                    FontSize = 12,
                    FontWeight = FontWeights.Bold,
                    Cursor = Cursors.Hand,
                    BorderThickness = new Thickness(0)
                };
                buyButton.Click += (s, e) => BuyButton_Click(car);
                DetailPanel.Children.Add(buyButton);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi hiển thị chi tiết: {ex.Message}");
            }
        }

        private Brush GetColorBrush(string colorName)
        {
            switch (colorName.ToLower())
            {
                case "đen":
                    return new SolidColorBrush(Colors.Black);

                case "trắng":
                    return new SolidColorBrush(Colors.White);

                case "đỏ":
                    return new SolidColorBrush(Color.FromRgb(217, 40, 40));

                case "xanh":
                    return new SolidColorBrush(Color.FromRgb(127, 140, 141));

                default:
                    return new SolidColorBrush(Color.FromRgb(149, 165, 166));
            }
        }

        private void BuyButton_Click(Xe car)
        {
            MessageBox.Show($"Bạn chọn mua: {car.TenDong}\nGiá: {car.GiaBan:N0} VND", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            BrandUserControl brandControl = new BrandUserControl();
            Window parentWindow = Window.GetWindow(this);

            if (parentWindow is MainWindow mainWindow)
            {
                mainWindow.MainContentControl.Content = brandControl;
            }
        }
    }
}
