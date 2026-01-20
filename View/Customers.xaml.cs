using Page_Navigation_App.Database; // Chứa class DBConnection của bạn
using Page_Navigation_App.Model;
using Page_Navigation_App.Utilities; // Chứa RelayCommand
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            // Chỉ cho phép các con số từ 0-9
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void txtDonGia_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Cho phép các phím điều khiển hệ thống hoạt động bình thường
            if (e.Key == Key.Back || e.Key == Key.Delete || e.Key == Key.Left ||
                e.Key == Key.Right || e.Key == Key.Tab || e.Key == Key.Enter)
            {
                e.Handled = false;
            }
            // Chặn phím Space (dấu cách) vì Regex đôi khi không chặn được phím này
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }
        private void txtDonGia_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox == null) return;

            // Lưu vị trí con trỏ hiện tại
            int selectionStart = textBox.SelectionStart;
            int oldLength = textBox.Text.Length;

            // Ép TextBox cập nhật định dạng từ ViewModel (thêm dấu phẩy)
            // Sau khi ViewModel cập nhật, Text có thể thay đổi độ dài (ví dụ từ 999 lên 1,000)

            int newLength = textBox.Text.Length;

            // Điều chỉnh lại vị trí con trỏ để không bị nhảy
            int newSelectionStart = selectionStart + (newLength - oldLength);

            if (newSelectionStart >= 0)
                textBox.SelectionStart = newSelectionStart;
        }

        private void cbMaLSP_Load(object sender, RoutedEventArgs e)
        {
            cbMaLSP.Items.Clear();
            string sql = "SELECT MaLSP FROM LoaiSP";
            DataTable dt = DBConnection.ExecuteQuery(sql);
            foreach (DataRow row in dt.Rows)
            {
                cbMaLSP.Items.Add(row["MaLSP"].ToString());
            }
        }

        private void txtSL_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}

