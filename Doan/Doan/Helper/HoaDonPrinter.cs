using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Doan.ViewModel;

namespace Doan.Helper
{
    // Dựng hóa đơn dạng FlowDocument và in qua PrintDialog.
    public static class HoaDonPrinter
    {
        private static readonly CultureInfo VN = new CultureInfo("vi-VN");

        public static void InHoaDon(HoaDon_HienThi_VM hd)
        {
            if (hd == null)
            {
                MessageBox.Show("Vui lòng chọn một hóa đơn để in.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            HienThiXemTruoc("Hóa đơn " + hd.MaHD, w => TaoTaiLieu(hd, w));
        }

        // In cả giao dịch (nhiều mặt hàng mua 1 lần).
        public static void InGiaoDich(GiaoDich_HienThi_VM gd)
        {
            if (gd == null)
            {
                MessageBox.Show("Vui lòng chọn một giao dịch để in.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            HienThiXemTruoc("Hóa đơn " + gd.MaGiaoDich, w => TaoTaiLieuGiaoDich(gd, w));
        }

        // Cửa sổ XEM TRƯỚC + nút In (tránh hộp thoại in hệ thống báo "no preview").
        private static void HienThiXemTruoc(string tenIn, Func<double, FlowDocument> taoTaiLieu)
        {
            var viewer = new FlowDocumentScrollViewer
            {
                Document = taoTaiLieu(760),
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                Background = Brushes.WhiteSmoke
            };

            var btnIn = new Button { Content = "🖨  In / Lưu PDF", Padding = new Thickness(20, 8, 20, 8), Margin = new Thickness(0, 0, 10, 0), MinWidth = 150 };
            var btnDong = new Button { Content = "Đóng", Padding = new Thickness(20, 8, 20, 8) };
            ApDungStyle(btnIn, "SuccessButton");
            ApDungStyle(btnDong, "GhostButton");

            var thanhNut = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(12) };
            thanhNut.Children.Add(btnIn);
            thanhNut.Children.Add(btnDong);

            var bo = new DockPanel();
            DockPanel.SetDock(thanhNut, Dock.Bottom);
            bo.Children.Add(thanhNut);
            bo.Children.Add(viewer);

            var cuaSo = new Window
            {
                Title = "Xem trước " + tenIn,
                Width = 820,
                Height = 740,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Content = bo
            };
            ApDungBrush(cuaSo);

            btnIn.Click += (s, e) =>
            {
                var printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    FlowDocument docIn = taoTaiLieu(printDialog.PrintableAreaWidth);
                    IDocumentPaginatorSource idp = docIn;
                    idp.DocumentPaginator.PageSize = new Size(printDialog.PrintableAreaWidth, printDialog.PrintableAreaHeight);
                    printDialog.PrintDocument(idp.DocumentPaginator, tenIn);
                }
            };
            btnDong.Click += (s, e) => cuaSo.Close();

            cuaSo.ShowDialog();
        }

        private static FlowDocument TaoTaiLieuGiaoDich(GiaoDich_HienThi_VM gd, double pageWidth)
        {
            var doc = new FlowDocument
            {
                PagePadding = new Thickness(50),
                ColumnWidth = double.PositiveInfinity,
                FontFamily = new FontFamily("Segoe UI"),
                FontSize = 13,
                PageWidth = pageWidth,
                Background = Brushes.White,
                Foreground = Brushes.Black
            };

            doc.Blocks.Add(DoanVan("CỬA HÀNG XE Ô TÔ", 20, FontWeights.Bold, TextAlignment.Center, Color.FromRgb(0x0E, 0x7C, 0x66)));
            doc.Blocks.Add(DoanVan("Địa chỉ: 123 Đường Ô Tô, TP. Hồ Chí Minh  •  ĐT: 1900 1234", 11, FontWeights.Normal, TextAlignment.Center, Colors.Gray));
            doc.Blocks.Add(DoanVan("HÓA ĐƠN BÁN HÀNG", 18, FontWeights.Bold, TextAlignment.Center, Color.FromRgb(0x2C, 0x3E, 0x50)));
            doc.Blocks.Add(DoanVan("Số: " + gd.MaGiaoDich + "          Ngày lập: " + DinhDangNgay(gd.NgayLap), 12, FontWeights.Normal, TextAlignment.Center, Colors.DimGray));

            doc.Blocks.Add(Dong("Khách hàng:", string.IsNullOrWhiteSpace(gd.TenKhachHang) ? "(Khách lẻ)" : gd.TenKhachHang));
            doc.Blocks.Add(Dong("Số điện thoại:", string.IsNullOrWhiteSpace(gd.SDT) ? "—" : gd.SDT));
            doc.Blocks.Add(Dong("Nhân viên lập:", string.IsNullOrWhiteSpace(gd.TenNhanVien) ? "—" : gd.TenNhanVien));
            doc.Blocks.Add(Dong("Trạng thái:", gd.TrangThai ?? "—"));

            var table = new Table { CellSpacing = 0, Margin = new Thickness(0, 14, 0, 14) };
            table.Columns.Add(new TableColumn { Width = new GridLength(3, GridUnitType.Star) });
            table.Columns.Add(new TableColumn { Width = new GridLength(1, GridUnitType.Star) });
            table.Columns.Add(new TableColumn { Width = new GridLength(2, GridUnitType.Star) });
            var rowGroup = new TableRowGroup();
            table.RowGroups.Add(rowGroup);
            rowGroup.Rows.Add(HangTieuDe());

            if (gd.DanhSachMatHang != null)
            {
                foreach (var mh in gd.DanhSachMatHang)
                {
                    long tt = (long)(mh.ThanhTien ?? 0);
                    rowGroup.Rows.Add(HangDuLieu(mh.TenDV_SP ?? "", (mh.SoLuong ?? 0).ToString(), tt.ToString("N0", VN) + " đ"));
                }
            }
            doc.Blocks.Add(table);

            doc.Blocks.Add(DoanVan("TỔNG THANH TOÁN: " + ((long)gd.TongTien).ToString("N0", VN) + " đ", 16, FontWeights.Bold, TextAlignment.Right, Color.FromRgb(0xC0, 0x39, 0x2B)));
            doc.Blocks.Add(Dong("Phương thức thanh toán:", gd.PhuongThucThanhToan ?? "—"));

            var footer = DoanVan("Cảm ơn quý khách. Hẹn gặp lại!", 12, FontWeights.Normal, TextAlignment.Center, Colors.Gray);
            footer.Margin = new Thickness(0, 24, 0, 0);
            doc.Blocks.Add(footer);

            return doc;
        }

        private static void ApDungStyle(Button b, string tenStyle)
        {
            var st = Application.Current != null ? Application.Current.TryFindResource(tenStyle) as Style : null;
            if (st != null) b.Style = st;
        }

        private static void ApDungBrush(Window w)
        {
            var bg = Application.Current != null ? Application.Current.TryFindResource("BrushBackground") as Brush : null;
            if (bg != null) w.Background = bg;
        }

        private static FlowDocument TaoTaiLieu(HoaDon_HienThi_VM hd, double pageWidth)
        {
            var doc = new FlowDocument
            {
                PagePadding = new Thickness(50),
                ColumnWidth = double.PositiveInfinity,
                FontFamily = new FontFamily("Segoe UI"),
                FontSize = 13,
                PageWidth = pageWidth,
                Background = Brushes.White,
                Foreground = Brushes.Black
            };

            // Tiêu đề cửa hàng
            doc.Blocks.Add(DoanVan("CỬA HÀNG XE Ô TÔ", 20, FontWeights.Bold, TextAlignment.Center, Color.FromRgb(0x0E, 0x7C, 0x66)));
            doc.Blocks.Add(DoanVan("Địa chỉ: 123 Đường Ô Tô, TP. Hồ Chí Minh  •  ĐT: 1900 1234", 11, FontWeights.Normal, TextAlignment.Center, Colors.Gray));

            doc.Blocks.Add(DoanVan("HÓA ĐƠN BÁN HÀNG", 18, FontWeights.Bold, TextAlignment.Center, Color.FromRgb(0x2C, 0x3E, 0x50)) );
            doc.Blocks.Add(DoanVan("Số: " + hd.MaHD + "          Ngày lập: " + DinhDangNgay(hd.NgayLap), 12, FontWeights.Normal, TextAlignment.Center, Colors.DimGray));

            // Thông tin khách / nhân viên
            doc.Blocks.Add(Dong("Khách hàng:", string.IsNullOrWhiteSpace(hd.TenKhachHang) ? "(Khách lẻ)" : hd.TenKhachHang));
            doc.Blocks.Add(Dong("Số điện thoại:", string.IsNullOrWhiteSpace(hd.SDT) ? "—" : hd.SDT));
            doc.Blocks.Add(Dong("Nhân viên lập:", string.IsNullOrWhiteSpace(hd.TenNhanVien) ? "—" : hd.TenNhanVien));
            doc.Blocks.Add(Dong("Trạng thái:", hd.TrangThai ?? "—"));

            // Bảng chi tiết
            var table = new Table { CellSpacing = 0, Margin = new Thickness(0, 14, 0, 14) };
            table.Columns.Add(new TableColumn { Width = new GridLength(3, GridUnitType.Star) });
            table.Columns.Add(new TableColumn { Width = new GridLength(1, GridUnitType.Star) });
            table.Columns.Add(new TableColumn { Width = new GridLength(2, GridUnitType.Star) });

            var rowGroup = new TableRowGroup();
            table.RowGroups.Add(rowGroup);

            rowGroup.Rows.Add(HangTieuDe());

            long thanhTien = (long)(hd.ThanhTien ?? 0);
            rowGroup.Rows.Add(HangDuLieu(
                hd.TenDV_SP ?? "",
                (hd.SoLuong ?? 0).ToString(),
                thanhTien.ToString("N0", VN) + " đ"));

            doc.Blocks.Add(table);

            // Tổng tiền
            doc.Blocks.Add(DoanVan("TỔNG THANH TOÁN: " + thanhTien.ToString("N0", VN) + " đ", 16, FontWeights.Bold, TextAlignment.Right, Color.FromRgb(0xC0, 0x39, 0x2B)));
            doc.Blocks.Add(Dong("Phương thức thanh toán:", hd.PhuongThucThanhToan ?? "—"));

            // Chân trang
            var footer = DoanVan("Cảm ơn quý khách. Hẹn gặp lại!", 12, FontWeights.Normal, TextAlignment.Center, Colors.Gray);
            footer.Margin = new Thickness(0, 24, 0, 0);
            doc.Blocks.Add(footer);

            return doc;
        }

        private static TableRow HangTieuDe()
        {
            var row = new TableRow { Background = new SolidColorBrush(Color.FromRgb(0x0E, 0x7C, 0x66)) };
            row.Cells.Add(OCell("Tên dịch vụ / sản phẩm", true, TextAlignment.Left));
            row.Cells.Add(OCell("Số lượng", true, TextAlignment.Center));
            row.Cells.Add(OCell("Thành tiền", true, TextAlignment.Right));
            return row;
        }

        private static TableRow HangDuLieu(string ten, string sl, string tien)
        {
            var row = new TableRow();
            row.Cells.Add(OCell(ten, false, TextAlignment.Left));
            row.Cells.Add(OCell(sl, false, TextAlignment.Center));
            row.Cells.Add(OCell(tien, false, TextAlignment.Right));
            return row;
        }

        private static TableCell OCell(string text, bool header, TextAlignment align)
        {
            var p = new Paragraph(new Run(text))
            {
                Margin = new Thickness(6, 4, 6, 4),
                TextAlignment = align,
                FontWeight = header ? FontWeights.Bold : FontWeights.Normal,
                Foreground = header ? Brushes.White : Brushes.Black
            };
            return new TableCell(p)
            {
                BorderBrush = Brushes.LightGray,
                BorderThickness = new Thickness(0, 0, 0, 0.5)
            };
        }

        private static Paragraph DoanVan(string text, double size, FontWeight weight, TextAlignment align, Color color)
        {
            return new Paragraph(new Run(text))
            {
                FontSize = size,
                FontWeight = weight,
                TextAlignment = align,
                Foreground = new SolidColorBrush(color),
                Margin = new Thickness(0, 2, 0, 2)
            };
        }

        private static Paragraph Dong(string nhan, string giaTri)
        {
            var p = new Paragraph { Margin = new Thickness(0, 2, 0, 2) };
            p.Inlines.Add(new Run(nhan + " ") { FontWeight = FontWeights.Bold });
            p.Inlines.Add(new Run(giaTri));
            return p;
        }

        private static string DinhDangNgay(DateTime? ngay)
        {
            return ngay.HasValue ? ngay.Value.ToString("dd/MM/yyyy HH:mm") : "—";
        }
    }
}
