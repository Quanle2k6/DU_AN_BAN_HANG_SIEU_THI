using Page_Navigation_App.Model;
using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace Page_Navigation_App.View
{
    public partial class PageThemNhapHang : UserControl
    {
        public event Action ReloadNhapHang;

        ObservableCollection<ChiTietNhapHangModel> danhSachCT;

        string connStr =
            ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;

        public PageThemNhapHang()
        {
            InitializeComponent();

            danhSachCT = new ObservableCollection<ChiTietNhapHangModel>();
            dgChiTiet.ItemsSource = danhSachCT;

            LoadNhaCungCap();
        }

     

        void LoadNhaCungCap()
        {
            cbNhaCungCap.Items.Clear();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(
                    "SELECT MaNCC, TenNCC, SoTaiKhoan FROM NHACUNGCAP", conn);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                cbNhaCungCap.ItemsSource = dt.DefaultView;
                cbNhaCungCap.DisplayMemberPath = "TenNCC";
                cbNhaCungCap.SelectedValuePath = "MaNCC";
            }
        }

    
        private void cbNhaCungCap_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbNhaCungCap.SelectedItem is DataRowView row)
            {
                txtSoTaiKhoan.Text = row["SoTaiKhoan"].ToString();
            }
        }


        private void BtnLuu_Click(object sender, RoutedEventArgs e)
        {
            if (cbNhaCungCap.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn nhà cung cấp");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();

                try
                {
                    bool tonTai = KiemTraMaNH(conn, tran);


                    if (tonTai)
                    {
                        TruTonKhoCu(conn, tran);   

                        XoaChiTietCu(conn, tran);
                        CapNhatNhapHang(conn, tran);
                        ThemChiTiet(conn, tran);
                        tran.Commit(); 
                        MessageBox.Show("Cập nhật thành công");
                        ReloadNhapHang?.Invoke();
                    }
                    else
                    {
                        ThemNhapHang(conn, tran);
                        ThemChiTiet(conn, tran);

                        tran.Commit();
                        MessageBox.Show("Thêm thành công");
                        ReloadNhapHang?.Invoke();
                    }
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
                "SELECT COUNT(*) FROM NHAPHANG WHERE MaNH = @MaNH",
                conn, tran);

            cmd.Parameters.AddWithValue("@MaNH", txtMaNH.Text);
            return (int)cmd.ExecuteScalar() > 0;
        }

        

        void ThemNhapHang(SqlConnection conn, SqlTransaction tran)
        {
            SqlCommand cmd = new SqlCommand(
                @"INSERT INTO NHAPHANG (MaNH, MaNCC, NgGiao, HSD)
                  VALUES (@MaNH, @MaNCC, @NgGiao, @HSD)",
                conn, tran);

            cmd.Parameters.AddWithValue("@MaNH", txtMaNH.Text);
            cmd.Parameters.AddWithValue("@MaNCC", cbNhaCungCap.SelectedValue.ToString());
            cmd.Parameters.AddWithValue("@NgGiao", dpNgayNhap.SelectedDate);
            cmd.Parameters.AddWithValue("@HSD", dpHSD.SelectedDate);

            cmd.ExecuteNonQuery();
        }

        

        void CapNhatNhapHang(SqlConnection conn, SqlTransaction tran)
        {
            SqlCommand cmd = new SqlCommand(
                @"UPDATE NHAPHANG
                  SET MaNCC = @MaNCC,
                      NgGiao = @NgGiao,
                      HSD = @HSD
                  WHERE MaNH = @MaNH",
                conn, tran);

            cmd.Parameters.AddWithValue("@MaNH", txtMaNH.Text);
            cmd.Parameters.AddWithValue("@MaNCC", cbNhaCungCap.SelectedValue.ToString());
            cmd.Parameters.AddWithValue("@NgGiao", dpNgayNhap.SelectedDate);
            cmd.Parameters.AddWithValue("@HSD", dpHSD.SelectedDate);

            cmd.ExecuteNonQuery();
        }

       

        void XoaChiTietCu(SqlConnection conn, SqlTransaction tran)
        {
            SqlCommand cmd = new SqlCommand(
                "DELETE FROM CHITIETNHAPHANG WHERE MaNH = @MaNH",
                conn, tran);

            cmd.Parameters.AddWithValue("@MaNH", txtMaNH.Text);
            cmd.ExecuteNonQuery();
        }


        void ThemChiTiet(SqlConnection conn, SqlTransaction tran)
        {
            foreach (var ct in danhSachCT)
            {
                

                SqlCommand cmdCT = new SqlCommand(
                    @"INSERT INTO CHITIETNHAPHANG
              (MaNH, MaSP, SoLuongNhapHang, GiaNhap)
              VALUES (@MaNH, @MaSP, @SoLuong, @GiaNhap)",
                    conn, tran);

                cmdCT.Parameters.AddWithValue("@MaNH", txtMaNH.Text);
                cmdCT.Parameters.AddWithValue("@MaSP", ct.MaSP);
                cmdCT.Parameters.AddWithValue("@SoLuong", ct.SoLuongNhapHang);
                cmdCT.Parameters.AddWithValue("@GiaNhap", ct.GiaNhap);
                cmdCT.ExecuteNonQuery();

                

                SqlCommand cmdUpdate = new SqlCommand(
                    @"UPDATE SANPHAM
              SET SoLuongCon = ISNULL(SoLuongCon, 0) + @SoLuong
              WHERE MaSP = @MaSP",
                    conn, tran);

                cmdUpdate.Parameters.AddWithValue("@SoLuong", ct.SoLuongNhapHang);
                cmdUpdate.Parameters.AddWithValue("@MaSP", ct.MaSP);
                cmdUpdate.ExecuteNonQuery();
            }
        }


        void TruTonKhoCu(SqlConnection conn, SqlTransaction tran)
        {
            SqlCommand cmd = new SqlCommand(
                @"SELECT MaSP, SoLuongNhapHang
          FROM CHITIETNHAPHANG
          WHERE MaNH = @MaNH",
                conn, tran);

            cmd.Parameters.AddWithValue("@MaNH", txtMaNH.Text);

            DataTable dt = new DataTable();
            new SqlDataAdapter(cmd).Fill(dt);

            foreach (DataRow row in dt.Rows)
            {
                SqlCommand cmdUpdate = new SqlCommand(
                    @"UPDATE SANPHAM
              SET SoLuongCon = SoLuongCon - @SoLuong
              WHERE MaSP = @MaSP",
                    conn, tran);

                cmdUpdate.Parameters.AddWithValue("@MaSP", row["MaSP"]);
                cmdUpdate.Parameters.AddWithValue("@SoLuong", row["SoLuongNhapHang"]);
                cmdUpdate.ExecuteNonQuery();
            }
        }


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var main = (MainWindow)Application.Current.MainWindow;
            main.HideOverlay();
        }
    }
}
