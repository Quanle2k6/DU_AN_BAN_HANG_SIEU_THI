using Page_Navigation_App.ViewModel;
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
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : UserControl
    {
        private SettingVM VM => DataContext as SettingVM;

        public Settings()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            VM?.SaveNhanVien();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string newPass = Microsoft.VisualBasic.Interaction.InputBox(
        "Nhập mật khẩu mới", "Đổi mật khẩu");

            VM?.ChangePassword(newPass);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            VM?.Logout();
        }
    }
}
