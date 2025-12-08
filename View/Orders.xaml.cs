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
    /// Interaction logic for Orders.xaml
    /// </summary>
    public partial class Orders : UserControl
    {
        public Orders()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void txtBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if(txtSearch.Text == "Tìm kiếm theo mã hoá đơn")
            {
                txtSearch.Text = "";
                txtSearch.Foreground = Brushes.Black;
            }
        }

        private void txtBox_LostFocus(object sender,  RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtSearch.Text))
            {
                txtSearch.Text = "Tìm kiếm hoá đơn";
                txtSearch.Foreground = Brushes.White;
            }
        }
    }
}
