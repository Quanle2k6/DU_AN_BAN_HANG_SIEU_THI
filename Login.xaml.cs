using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using Page_Navigation_App.Database; // Đảm bảo using đúng namespace của file DBConnection

namespace Page_Navigation_App
{
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            // 1. Lấy thông tin từ giao diện
            string username = txtUser.Text.Trim();
            string password = (chkShowPass.IsChecked == true) ? txtPassVisible.Text : txtPass.Password;

            // 2. Kiểm tra đầu vào cơ bản
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ tài khoản và mật khẩu!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 3. Chuẩn bị truy vấn và tham số
            string sql = "SELECT * FROM DANGNHAP WHERE TenDN = @user AND MatKhau = @pass";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@user", SqlDbType.NVarChar) { Value = username },
                new SqlParameter("@pass", SqlDbType.NVarChar) { Value = password }
            };

            // 4. Gọi DBConnection để thực thi
            DataTable dt = DBConnection.ExecuteQuery(sql, parameters);

            // 5. Kiểm tra kết quả
            if (dt != null && dt.Rows.Count > 0)
            {
                // Đăng nhập thành công
                MessageBox.Show($"Chào mừng {username} quay trở lại!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                MainWindow main = new MainWindow();
                main.DataContext = new Page_Navigation_App.ViewModel.NavigationVM();
                Application.Current.MainWindow = main;
                main.Show();

                this.Close(); // Đóng cửa sổ Login
            }
            else
            {
                // Đăng nhập thất bại
                MessageBox.Show("Tên đăng nhập hoặc mật khẩu không chính xác.", "Lỗi đăng nhập", MessageBoxButton.OK, MessageBoxImage.Error);

                // Reset form
                txtPass.Clear();
                txtPassVisible.Clear();
                txtUser.Focus();
            }
        }

        // --- Giữ nguyên các hàm giao diện bên dưới ---

        private void CloseApp_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void chkShowPass_Checked(object sender, RoutedEventArgs e)
        {
            txtPassVisible.Text = txtPass.Password;
            txtPassVisible.Visibility = Visibility.Visible;
            txtPass.Visibility = Visibility.Collapsed;
            txtPassVisible.Focus();
            txtPassVisible.SelectionStart = txtPassVisible.Text.Length;
        }

        private void chkShowPass_Unchecked(object sender, RoutedEventArgs e)
        {
            txtPass.Password = txtPassVisible.Text;
            txtPass.Visibility = Visibility.Visible;
            txtPassVisible.Visibility = Visibility.Collapsed;
            txtPass.Focus();

            // Focus vào cuối PasswordBox bằng Reflection
            txtPass.GetType().GetMethod("Select", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                ?.Invoke(txtPass, new object[] { txtPass.Password.Length, 0 });
        }
    }
}