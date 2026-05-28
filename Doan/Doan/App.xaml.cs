using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Doan
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            // Bắt mọi lỗi chưa xử lý để hiển thị thông báo thay vì văng app.
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Exception loi = e.Exception;
            while (loi.InnerException != null)
            {
                loi = loi.InnerException;
            }

            MessageBox.Show("Đã xảy ra lỗi: " + loi.Message, "Lỗi",
                MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }
    }
}
