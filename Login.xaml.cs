using System;
using System.Windows;

namespace Page_Navigation_App
{
    public partial class Login : Window // Đã đổi sang Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            // Logic chuyển qua MainWindow
            MainWindow main = new MainWindow();
            main.DataContext = new Page_Navigation_App.ViewModel.NavigationVM();
            Application.Current.MainWindow = main;
            main.Show();

            this.Close(); // Đóng cửa sổ Login
        }

        private void CloseApp_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(); // Thoát toàn bộ ứng dụng
        }
    }
}