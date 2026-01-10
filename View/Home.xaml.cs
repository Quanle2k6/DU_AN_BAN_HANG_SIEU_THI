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

namespace Page_Navigation_App.View
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : UserControl
    {
        public Home()
        {
            InitializeComponent();
        }
        public void ChangeIntoHangTonPage(object sender, RoutedEventArgs e)
        {
            // Lấy ViewModel từ MainWindow
            if (Application.Current.MainWindow.DataContext is Page_Navigation_App.ViewModel.NavigationVM navVM)
            {
                // Kiểm tra đúng tên Command (thường là TransactionsCommand cho hàng tồn/giao dịch)
                if (navVM.TransactionsCommand != null && navVM.TransactionsCommand.CanExecute(null))
                {
                    navVM.TransactionsCommand.Execute(null);
                }
            }
        }

        public void ChangeIntoProductPage(object sender, RoutedEventArgs e)
        {
            // Tìm MainWindow của ứng dụng
            var mainWindow = Application.Current.MainWindow;

            // Kiểm tra DataContext của MainWindow
            if (mainWindow != null && mainWindow.DataContext != null)
            {
                // Sử dụng dynamic để bỏ qua kiểm tra kiểu dữ liệu khắt khe lúc biên dịch nếu bạn không chắc chắn Namespace
                dynamic navVM = mainWindow.DataContext;

                try
                {
                    // Thay "CustomersCommand" bằng tên chính xác trong NavigationVM của bạn
                    if (navVM.CustomersCommand != null)
                    {
                        navVM.CustomersCommand.Execute(null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: Không tìm thấy lệnh chuyển trang! " + ex.Message);
                }
            }
        }

        public void ChangeIntoDoanhSoPage(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow.DataContext is Page_Navigation_App.ViewModel.NavigationVM navVM)
            {
                // LỖI CŨ: Kiểm tra Orders nhưng Execute Shipments -> Đã sửa lại đồng bộ
                // Thay OrdersCommand bằng Command tương ứng với Doanh số trong VM của bạn
                if (navVM.OrdersCommand != null && navVM.OrdersCommand.CanExecute(null))
                {
                    navVM.OrdersCommand.Execute(null);
                }
            }
        }


    }
}
