using System;
using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;

namespace Doan.Helper
{
    // Gợi ý tự động (autocomplete) cho TextBox: gõ vài chữ -> hiện popup danh sách gợi ý.
    // Dùng: helper:AutoCompleteBehavior.Suggestions="{Binding GoiYTimKiem}"
    public static class AutoCompleteBehavior
    {
        public static readonly DependencyProperty SuggestionsProperty =
            DependencyProperty.RegisterAttached(
                "Suggestions", typeof(IEnumerable), typeof(AutoCompleteBehavior),
                new PropertyMetadata(null, OnSuggestionsChanged));

        public static void SetSuggestions(DependencyObject o, IEnumerable v) { o.SetValue(SuggestionsProperty, v); }
        public static IEnumerable GetSuggestions(DependencyObject o) { return (IEnumerable)o.GetValue(SuggestionsProperty); }

        private static readonly DependencyProperty PopupProperty =
            DependencyProperty.RegisterAttached("Popup", typeof(Popup), typeof(AutoCompleteBehavior));

        private static bool dangCapNhat;

        private static void OnSuggestionsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tb = d as TextBox;
            if (tb == null) return;

            if (e.OldValue == null && e.NewValue != null)
            {
                tb.TextChanged += Tb_TextChanged;
                tb.LostFocus += Tb_LostFocus;
                tb.PreviewKeyDown += Tb_PreviewKeyDown;
            }
        }

        private static Popup LayPopup(TextBox tb, bool taoMoi)
        {
            var popup = tb.GetValue(PopupProperty) as Popup;
            if (popup == null && taoMoi)
            {
                var list = new ListBox { MaxHeight = 220, BorderThickness = new Thickness(0) };
                list.SetResourceReference(Control.BackgroundProperty, "BrushCard");
                list.SetResourceReference(Control.ForegroundProperty, "BrushText");
                list.PreviewMouseLeftButtonUp += (s, e) => ChonGoiY(tb);

                var border = new Border
                {
                    Child = list,
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(6)
                };
                border.SetResourceReference(Border.BackgroundProperty, "BrushCard");
                border.SetResourceReference(Border.BorderBrushProperty, "BrushBorder");

                popup = new Popup
                {
                    PlacementTarget = tb,
                    Placement = PlacementMode.Bottom,
                    StaysOpen = false,
                    AllowsTransparency = true,
                    Child = border
                };
                tb.SetValue(PopupProperty, popup);
            }
            return popup;
        }

        private static ListBox LayDanhSach(Popup popup)
        {
            var border = popup.Child as Border;
            return border != null ? border.Child as ListBox : null;
        }

        private static void Tb_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (dangCapNhat) return;
            var tb = (TextBox)sender;
            var nguon = GetSuggestions(tb);
            if (nguon == null) return;

            string text = (tb.Text ?? string.Empty).Trim();
            if (text.Length < 1)
            {
                DongPopup(tb);
                return;
            }

            var khop = nguon.Cast<object>()
                .Select(x => x == null ? string.Empty : x.ToString())
                .Where(s => !string.IsNullOrEmpty(s) && s.IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0)
                .Distinct()
                .Take(8)
                .ToList();

            if (khop.Count == 0 || (khop.Count == 1 && string.Equals(khop[0], text, StringComparison.OrdinalIgnoreCase)))
            {
                DongPopup(tb);
                return;
            }

            var popup = LayPopup(tb, true);
            var list = LayDanhSach(popup);
            list.ItemsSource = khop;
            list.SelectedIndex = -1;
            popup.Width = tb.ActualWidth;
            popup.IsOpen = true;
        }

        private static void ChonGoiY(TextBox tb)
        {
            var popup = LayPopup(tb, false);
            if (popup == null) return;
            var list = LayDanhSach(popup);
            if (list == null || list.SelectedItem == null) return;

            dangCapNhat = true;
            tb.Text = list.SelectedItem.ToString();
            tb.CaretIndex = tb.Text.Length;
            dangCapNhat = false;

            popup.IsOpen = false;
            tb.Focus();
        }

        private static void DongPopup(TextBox tb)
        {
            var popup = tb.GetValue(PopupProperty) as Popup;
            if (popup != null) popup.IsOpen = false;
        }

        private static void Tb_LostFocus(object sender, RoutedEventArgs e)
        {
            var tb = (TextBox)sender;
            var popup = tb.GetValue(PopupProperty) as Popup;
            if (popup == null) return;
            // Hoãn đóng để cho phép click vào mục gợi ý.
            tb.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!popup.IsMouseOver) popup.IsOpen = false;
            }), DispatcherPriority.Background);
        }

        private static void Tb_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var tb = (TextBox)sender;
            var popup = tb.GetValue(PopupProperty) as Popup;
            if (popup == null || !popup.IsOpen) return;
            var list = LayDanhSach(popup);
            if (list == null || list.Items.Count == 0) return;

            if (e.Key == Key.Down)
            {
                list.SelectedIndex = Math.Min(list.Items.Count - 1, list.SelectedIndex + 1);
                list.ScrollIntoView(list.SelectedItem);
                e.Handled = true;
            }
            else if (e.Key == Key.Up)
            {
                list.SelectedIndex = Math.Max(0, list.SelectedIndex - 1);
                list.ScrollIntoView(list.SelectedItem);
                e.Handled = true;
            }
            else if (e.Key == Key.Enter && list.SelectedItem != null)
            {
                ChonGoiY(tb);
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                popup.IsOpen = false;
            }
        }
    }
}
