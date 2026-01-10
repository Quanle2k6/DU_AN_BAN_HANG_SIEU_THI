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
            if (Application.Current.MainWindow.DataContext is Page_Navigation_App.ViewModel.NavigationVM navVM)
            {
                // Chuyển sang trang Quản lý nhập xuất
                if (navVM.TransactionsCommand?.CanExecute(null) == true)
                {
                    navVM.TransactionsCommand.Execute(null);
                    // Menu:Btn tương ứng trong MainWindow sẽ tự IsChecked nhờ Converter đã set ở trên
                }
            }
        }

        public void ChangeIntoProductPage(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow.DataContext is Page_Navigation_App.ViewModel.NavigationVM navVM)
            {
                // Chuyển sang trang Sản phẩm
                if (navVM.CustomersCommand?.CanExecute(null) == true)
                {
                    navVM.CustomersCommand.Execute(null);
                }
            }
        }
        public void ChangeIntoDoanhSoPage(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow.DataContext is Page_Navigation_App.ViewModel.NavigationVM navVM)
            {
                // Chuyển sang trang Doanh số
                if (navVM.OrdersCommand?.CanExecute(null) == true)
                {
                    navVM.ShipmentsCommand.Execute(null);
                }
            }
        }
    }
}
