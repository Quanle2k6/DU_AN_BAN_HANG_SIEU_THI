using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Data.SqlClient;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Page_Navigation_App.Database; // Chứa class DBConnection của bạn
using Page_Navigation_App.Utilities; // Chứa RelayCommand
using Page_Navigation_App.Model;
using System.Data;


namespace Page_Navigation_App.View
{
    /// <summary>
    /// Interaction logic for Customers.xaml
    /// </summary>
    public partial class Customers : UserControl
    {
        public Customers()
        {
            InitializeComponent();
        }


        private void ProductsDataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            // Kiểm tra nếu dòng có dữ liệu (không phải dòng trống)
            if (e.Row.Item != null)
            {
                // Gán số thứ tự vào Header (dù Header đang ẩn nhưng dữ liệu vẫn nằm ở đó)
                e.Row.Header = (e.Row.GetIndex() + 1).ToString();
            }
        }

        private void Xem_Click(object sender, RoutedEventArgs e)
        {
            string searchTerm = txt_Search.Text.Trim();
            // Tạo pattern để tìm kiếm bắt đầu bằng từ khóa
            string searchPattern = searchTerm + "%";

            // 1. Xác định câu lệnh SQL (Sửa lại đúng chính tả @searchPattern)
            string sql = "";
            switch (cbSearchType.SelectedIndex)
            {
                case 0: sql = "SELECT * FROM SANPHAM WHERE MaSP LIKE @searchPattern"; break;
                case 1: sql = "SELECT * FROM SANPHAM WHERE TenSP LIKE @searchPattern"; break;
                default: sql = "SELECT * FROM SANPHAM WHERE MaLSP LIKE @searchPattern"; break;
            }

            // Pass a SqlParameter[] as required by ExecuteQuery
            var parameters = new SqlParameter[] { new SqlParameter("@searchPattern", searchPattern) };

            DataTable dt = DBConnection.ExecuteQuery(sql, parameters);

            // 3. Cập nhật giao diện
            ProductsDataGrid.ItemsSource = dt?.DefaultView;
        }

        private void txt_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null)
            {
                // Nếu nội dung bắt đầu bằng "Hãy" thì xóa đi và đổi màu chữ sang đen
                if (textBox.Text.StartsWith("Hãy"))
                {
                    textBox.Text = "";
                    textBox.Foreground = Brushes.Black; // Đổi sang màu chữ nhập liệu
                }
            }
        }

        private void txt_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null && string.IsNullOrWhiteSpace(textBox.Text))
            {
                // Nếu người dùng bỏ trống, hiện lại hướng dẫn và đổi màu xám
                if (textBox.Name == "txtMaSP") textBox.Text = "Hãy nhập mã gồm 5 chữ số";
                // ... (Bạn có thể thêm các điều kiện cho các TextBox khác ở đây)

                textBox.Foreground = Brushes.Gray;
            }
        }

        private void txt1_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null && string.IsNullOrWhiteSpace(textBox.Text))
            {
                // Nếu người dùng bỏ trống, hiện lại hướng dẫn và đổi màu xám
                if (textBox.Name == "txtDonGia") textBox.Text = "Hãy nhập đơn giá sản phẩm";
                // ... (Bạn có thể thêm các điều kiện cho các TextBox khác ở đây)
                textBox.Foreground = Brushes.Gray;
            }
        }

        private void txt2_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null && string.IsNullOrWhiteSpace(textBox.Text))
            {
                // Nếu người dùng bỏ trống, hiện lại hướng dẫn và đổi màu xám
                if (textBox.Name == "txtTenSP") textBox.Text = "Hãy nhập tên sản phẩm";
                // ... (Bạn có thể thêm các điều kiện cho các TextBox khác ở đây)
                textBox.Foreground = Brushes.Gray;
            }
        }

        private void txt3_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null && string.IsNullOrWhiteSpace(textBox.Text))
            {
                // Nếu người dùng bỏ trống, hiện lại hướng dẫn và đổi màu xám
                if (textBox.Name == "txtMaLSP") textBox.Text = "Hãy nhập mã loại sản phẩm gồm 5 kí tự";
                // ... (Bạn có thể thêm các điều kiện cho các TextBox khác ở đây)
                textBox.Foreground = Brushes.Gray;
            }
        }

        private void XemTatCa_Click(object sender, RoutedEventArgs e)
        {
            string sql = "SELECT * FROM SANPHAM";
                
            DataTable dt = DBConnection.ExecuteQuery(sql);
            ProductsDataGrid.ItemsSource = dt.DefaultView;
            cbSearchType.SelectedIndex = 0;
            txt_Search.Text = "";
        }

        private void btnThem_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaSP.Text) || string.IsNullOrWhiteSpace(txtTenSP.Text) ||
                string.IsNullOrWhiteSpace(txtMaLSP.Text) || string.IsNullOrWhiteSpace(txtDonGia.Text))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin sản phẩm.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            List<string> dsMaSP = new List<string>();
            string checkSql = "SELECT MaSP FROM SANPHAM";
            DataTable dt = DBConnection.ExecuteQuery(checkSql);
            foreach (DataRow row in dt.Rows)
            {
                dsMaSP.Add(row["MaSP"].ToString());
            }
            if (dsMaSP.Contains(txtMaSP.Text))
            {
                MessageBox.Show("Mã sản phẩm đã tồn tại. Vui lòng sử dụng mã khác.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            string insertSql = "INSERT INTO SANPHAM (MaSP, TenSP, MaLSP, GiaBan) VALUES (@MaSP, @TenSP, @MaLSP, @GiaBan)";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@MaSP", txtMaSP.Text),
                new SqlParameter("@TenSP", txtTenSP.Text),
                new SqlParameter("@MaLSP", txtMaLSP.Text),
                new SqlParameter("@GiaBan", decimal.Parse(txtDonGia.Text))
            };
            int rowsAffected = DBConnection.ExecuteNonQuery(insertSql, parameters);
            if (rowsAffected > 0)
            {
                MessageBox.Show("Thêm sản phẩm thành công.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                // Cập nhật lại DataGrid
                string refreshSql = "SELECT * FROM SANPHAM";
                DataTable refreshedDt = DBConnection.ExecuteQuery(refreshSql);
                ProductsDataGrid.ItemsSource = refreshedDt.DefaultView;
            }
            else
            {
                MessageBox.Show("Thêm sản phẩm thất bại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnXoa_Click(object sender, RoutedEventArgs e)
        {
            string maSP = txtMaSP.Text.Trim();

            // 1. Kiểm tra tồn tại (Nên dùng COUNT thay vì tải cả danh sách về để tiết kiệm RAM)
            string checkSql = "SELECT COUNT(*) FROM SANPHAM WHERE MaSP = @MaSP";
            var checkParam = new SqlParameter[] { new SqlParameter("@MaSP", maSP) };
            int count = (int)DBConnection.ExecuteQuery(checkSql, checkParam).Rows[0][0];
            if (count == 0)
            {
                MessageBox.Show("Mã sản phẩm không tồn tại. Vui lòng kiểm tra lại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 2. Hỏi xác nhận vì đây là thao tác xóa hàng loạt liên quan
            var result = MessageBox.Show("Xóa sản phẩm này sẽ xóa tất cả lịch sử Nhập hàng và Chi tiết hóa đơn liên quan. Bạn chắc chắn chứ?",
                                         "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // Xóa ở bảng con trước
                    string delCTNH = "DELETE FROM CHITIETNHAPHANG WHERE MaSP = @MaSP";
                    string delCTHD = "DELETE FROM CTHD WHERE MaSP = @MaSP";
                    // Xóa ở bảng cha sau cùng
                    string delSP = "DELETE FROM SANPHAM WHERE MaSP = @MaSP";

                    // Create a SqlParameter[] as ExecuteNonQuery expects an array
                    var param = new SqlParameter[] { new SqlParameter("@MaSP", maSP) };

                    // Nên thực hiện trong một Transaction để đảm bảo an toàn dữ liệu
                    DBConnection.ExecuteNonQuery(delCTNH, param);
                    DBConnection.ExecuteNonQuery(delCTHD, param);
                    int rowsAffected = DBConnection.ExecuteNonQuery(delSP, param);

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Xóa thành công!");
                        // Refresh DataGrid
                        string refreshSql = "SELECT * FROM SANPHAM";
                        DataTable refreshedDt = DBConnection.ExecuteQuery(refreshSql);
                        ProductsDataGrid.ItemsSource = refreshedDt.DefaultView;
                        ProductsDataGrid.Columns.Clear();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi xóa: " + ex.Message);
                }
            }
        }

        private void ProductDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (ProductsDataGrid.SelectedItem is DataRowView row)
            {
                txtMaSP.Text = row["MaSP"].ToString();
                txtTenSP.Text = row["TenSP"].ToString();
                txtMaLSP.Text = row["MaLSP"].ToString();
                txtDonGia.Text = row["GiaBan"].ToString();
            }
        }

        private void btnSua_Click(object sender, RoutedEventArgs e)
        {
            List<string> dsMaSP = new List<string>();
            string checkSql = "SELECT MaSP FROM SANPHAM";
            DataTable dt = DBConnection.ExecuteQuery(checkSql);
            foreach (DataRow row in dt.Rows)
            {
                dsMaSP.Add(row["MaSP"].ToString());
            }
            if (!dsMaSP.Contains(txtMaSP.Text))
            {
                MessageBox.Show("Mã sản phẩm không tồn tại. Vui lòng kiểm tra lại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            string updateSql = "UPDATE SANPHAM SET TenSP = @TenSP, MaLSP = @MaLSP, GiaBan = @GiaBan WHERE MaSP = @MaSP";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@TenSP", txtTenSP.Text),
                new SqlParameter("@MaLSP", txtMaLSP.Text),
                new SqlParameter("@GiaBan", decimal.Parse(txtDonGia.Text)),
                new SqlParameter("@MaSP", txtMaSP.Text)
            };
            int rowsAffected = DBConnection.ExecuteNonQuery(updateSql, parameters);
            if (rowsAffected > 0)
            {
                MessageBox.Show("Cập nhật sản phẩm thành công.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                // Cập nhật lại DataGrid
                string refreshSql = "SELECT * FROM SANPHAM";
                DataTable refreshedDt = DBConnection.ExecuteQuery(refreshSql);
                ProductsDataGrid.ItemsSource = refreshedDt.DefaultView;
            }
            else
            {
                MessageBox.Show("Cập nhật sản phẩm thất bại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

