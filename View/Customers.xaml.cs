using System;
using System.Collections.Generic;
using System.Globalization;
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
    }
}

