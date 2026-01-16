
using Page_Navigation_App.Model;
using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace Page_Navigation_App.View
{
    public partial class PageThemNhapHang : UserControl
    {
        ObservableCollection<ChiTietNhapHangModel> danhSachCT;
        string connStr = @"Server=(localdb)\MSSQLLocalDB;Database=YOUR_DB_NAME;Integrated Security=True;";

        public PageThemNhapHang()
        {
            InitializeComponent();
            danhSachCT = new ObservableCollection<ChiTietNhapHangModel>();
            dgChiTiet.ItemsSource = danhSachCT;
        }

        private void BtnLuu_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();

                try
                {
                    bool tonTai = KiemTraMaNH(conn, tran);

                    if (tonTai)
                    {
                        if (MessageBox.Show("Mã nhập hàng đã tồn tại. Bạn có muốn cập nhật không?",
                            "Xác nhận", MessageBoxButton.YesNo) == MessageBoxResult.No)
                        {
                            tran.Rollback();
                            return;
                        }

                        XoaChiTietCu(conn, tran);
                    }
                    else
                    {
                        ThemNhapHang(conn, tran);
                    }

                    ThemChiTiet(conn, tran);
                    tran.Commit();

                    MessageBox.Show("Đã thêm thành công");
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    MessageBox.Show(ex.Message);
                }
            }
        }

        bool KiemTraMaNH(SqlConnection conn, SqlTransaction tran)
        {
            SqlCommand cmd = new SqlCommand(
                "SELECT COUNT(*) FROM NHAPHANG WHERE MaNH=@MaNH",
                conn, tran);
            cmd.Parameters.AddWithValue("@MaNH", txtMaNH.Text);
            return (int)cmd.ExecuteScalar() > 0;
        }

        void ThemNhapHang(SqlConnection conn, SqlTransaction tran)
        {
            SqlCommand cmd = new SqlCommand(
                @"INSERT INTO NHAPHANG(MaNH, MaNCC, NgGiao, HSD)
                  VALUES(@MaNH,@MaNCC,@NgayNhap,@HSD)",
                conn, tran);

            cmd.Parameters.AddWithValue("@MaNH", txtMaNH.Text);
            cmd.Parameters.AddWithValue("@MaNCC", txtMaNCC.Text);
            cmd.Parameters.AddWithValue("@NgayNhap", dpNgayNhap.SelectedDate);
            cmd.Parameters.AddWithValue("@HSD", dpHSD.SelectedDate);
            cmd.ExecuteNonQuery();
        }

        void XoaChiTietCu(SqlConnection conn, SqlTransaction tran)
        {
            SqlCommand cmd = new SqlCommand(
                "DELETE FROM CHITIETNHAPHANG WHERE MaNH=@MaNH",
                conn, tran);
            cmd.Parameters.AddWithValue("@MaNH", txtMaNH.Text);
            cmd.ExecuteNonQuery();
        }

        void ThemChiTiet(SqlConnection conn, SqlTransaction tran)
        {
            foreach (var ct in danhSachCT)
            {
                SqlCommand cmd = new SqlCommand(
                    @"INSERT INTO CHITIETNHAPHANG
                      VALUES(@MaNH,@MaSP,@SoLuong,@GiaNhap)",
                    conn, tran);

                cmd.Parameters.AddWithValue("@MaNH", txtMaNH.Text);
                cmd.Parameters.AddWithValue("@MaSP", ct.MaSP);
                cmd.Parameters.AddWithValue("@SoLuong", ct.SoLuongNhapHang);
                cmd.Parameters.AddWithValue("@GiaNhap", ct.GiaNhap);
                cmd.ExecuteNonQuery();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var main = (MainWindow)Application.Current.MainWindow;
            main.HideOverlay();
        }
    }
}
