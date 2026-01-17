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
using System.Data.SqlClient;
using Page_Navigation_App.Database;
using Page_Navigation_App.Model;
using System.Data;

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
                if (navVM.ShipmentsCommand != null && navVM.ShipmentsCommand.CanExecute(null))
                {
                    navVM.ShipmentsCommand.Execute(null);
                }
            }
        }

        private void txtDoanhSo_Load(object sender, RoutedEventArgs e)
        {
            TextBlock txtDoanhSo = sender as TextBlock;
            if (txtDoanhSo == null) return;
            try
            {
                // 1. Lấy ngày hiện tại dưới dạng tham số để tránh lỗi định dạng SQL
                DateTime homNay = DateTime.Now.Date;

                // 2. Viết câu lệnh SQL (Dùng SUM để cộng tổng doanh số)
                // Lưu ý: CAST hoặc CONVERT tùy vào kiểu dữ liệu ngày trong DB của bạn
                string sql = "SELECT SUM(ThanhTien) FROM HoaDon WHERE CAST(NgayLapHD AS DATE) = @Ngay";

                // 3. Tạo tham số tránh SQL Injection
                SqlParameter param = new SqlParameter("@Ngay", homNay);

                // 4. Thực thi lấy một giá trị duy nhất (ExecuteScalar)
                object result = DBConnection.ExecuteScalar(sql, param);

                // 5. Kiểm tra kết quả và hiển thị
                if (result != DBNull.Value && result != null)
                {
                    decimal doanhSo = Convert.ToDecimal(result);
                    // Hiển thị định dạng tiền tệ (N0 = 1,000,000)
                    txtDoanhSo.Text = doanhSo.ToString("N0");
                }
                else
                {
                    txtDoanhSo.Text = "0 VNĐ";
                }
            }
            catch (Exception ex)
            {
                txtDoanhSo.Text = "Lỗi!";
                MessageBox.Show("Lỗi cập nhật doanh số: " + ex.Message);
            }
        }

        private void txtSanPham_Load(object sender, RoutedEventArgs e)
        {
            TextBlock txtSanPham = sender as TextBlock;
            if (txtSanPham == null) return;
            try
            {
                string sql = "SELECT COUNT(*) FROM SanPham";
                object result = DBConnection.ExecuteScalar(sql);
                if (result != DBNull.Value && result != null)
                {
                    int soLuongSP = Convert.ToInt32(result);
                    txtSanPham.Text = soLuongSP.ToString();
                }
                else
                {
                    txtSanPham.Text = "0";
                }
            }
            catch (Exception ex)
            {
                txtSanPham.Text = "Lỗi!";
                MessageBox.Show("Lỗi cập nhật số sản phẩm: " + ex.Message);
            }
        }

        private void txtHangTon_Load(object sender, RoutedEventArgs e)
        {
            TextBlock txtHangTon = sender as TextBlock;
            if (txtHangTon == null) return;
            try
            {
                string sql = "SELECT SUM(SoLuongCon) FROM SanPham";
                object result = DBConnection.ExecuteScalar(sql);
                if (result != DBNull.Value && result != null)
                {
                    int soLuongHangTon = Convert.ToInt32(result);
                    txtHangTon.Text = soLuongHangTon.ToString();
                }
                else
                {
                    txtHangTon.Text = "0";
                }
            }
            catch (Exception ex)
            {
                txtHangTon.Text = "Lỗi!";
                MessageBox.Show("Lỗi cập nhật số hàng tồn: " + ex.Message);
            }
        }

        private void txtSanPhamThietYeu_Load(object sender, RoutedEventArgs e)
        {
            TextBlock txtSanPhamThietYeu = sender as TextBlock;
            if (txtSanPhamThietYeu == null) return;
            try
            {
                string sql = "SELECT COUNT(*) FROM SanPham WHERE MaLSP IN ('L0001', 'L0004', 'L0005')";
                object result = DBConnection.ExecuteScalar(sql);
                if (result != DBNull.Value && result != null)
                {
                    int soLuongSPThietYeu = Convert.ToInt32(result);
                    txtSanPhamThietYeu.Text = soLuongSPThietYeu.ToString();
                }
                else
                {
                    txtSanPhamThietYeu.Text = "0";
                }
            }
            catch (Exception ex)
            {
                txtSanPhamThietYeu.Text = "Lỗi!";
                MessageBox.Show("Lỗi cập nhật số sản phẩm thiết yếu: " + ex.Message);
            }

        }
    }
}
