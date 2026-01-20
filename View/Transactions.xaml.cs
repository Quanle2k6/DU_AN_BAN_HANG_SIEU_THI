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
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not TransactionVM vm)
                return;

            if (dpTuNgay.SelectedDate == null || dpDenNgay.SelectedDate == null)
            {
                MessageBox.Show("Vui lòng chọn đầy đủ từ ngày và đến ngày",
                                "Thông báo",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            DateTime tuNgay = dpTuNgay.SelectedDate.Value.Date;
            DateTime denNgay = dpDenNgay.SelectedDate.Value.Date;

            if (tuNgay > denNgay)
            {
                MessageBox.Show("Từ ngày không được lớn hơn đến ngày",
                                "Thông báo",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            var ketQua = vm.GetNhapHangTheoNgay(tuNgay, denNgay);

            if (ketQua.Count == 0)
            {
                MessageBox.Show(
                    $"Trong khoảng thời gian từ {tuNgay:dd/MM/yyyy} đến {denNgay:dd/MM/yyyy} không có nhập hàng nào",
                    "Thông báo",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                return; 

            }
            vm.DanhSachNhapHang.Clear();
            foreach (var item in ketQua)
                vm.DanhSachNhapHang.Add(item);
        }

        private void btnXemTatCa_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not TransactionVM vm)
                return;
            vm.GetType()
              .GetMethod("LoadDanhSachNhapHang",
                  System.Reflection.BindingFlags.NonPublic |
                  System.Reflection.BindingFlags.Instance)
              ?.Invoke(vm, null);
        }
    }
}
