using Page_Navigation_App.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Page_Navigation_App.View
{
    public partial class Transactions : UserControl
    {
        public Transactions()
        {
            InitializeComponent();
        }

        private void btnThem_Click(object sender, RoutedEventArgs e)
        {
            var main = (MainWindow)Application.Current.MainWindow;

            var page = new PageThemNhapHang();
            page.ReloadNhapHang += ReloadDanhSachNhapHang;

            main.ShowOverlay(page);
        }

        private void btnChinhSua_Click(object sender, RoutedEventArgs e)
        {
            var main = (MainWindow)Application.Current.MainWindow;

            var page = new PageThemNhapHang();
            page.ReloadNhapHang += ReloadDanhSachNhapHang;

            main.ShowOverlay(page);
        }

        private void ReloadDanhSachNhapHang()
        {
            if (DataContext is TransactionVM vm)
            {
                vm.GetType()
                  .GetMethod("LoadDanhSachNhapHang",
                      System.Reflection.BindingFlags.NonPublic |
                      System.Reflection.BindingFlags.Instance)
                  ?.Invoke(vm, null);
            }
        }

        private void btnXoa_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is TransactionVM vm)
            {
                vm.XoaNhapHang();
            }
        }
    }
}
