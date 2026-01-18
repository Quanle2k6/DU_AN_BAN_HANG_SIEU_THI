using Page_Navigation_App.ViewModel;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace Page_Navigation_App.View
{
    public partial class Orders : UserControl
    {
        private readonly string connectionString =
    ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;


        public Orders()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as OrderVM;
            if (vm == null || vm.OrderDetails == null || vm.OrderDetails.Count == 0)
            {
                MessageBox.Show("Chưa có sản phẩm trong hóa đơn!");
                return;
            }

            string tenKH = txtKH.Text.Trim();
            string diaChi = txtDiaChi.Text.Trim();
            string sdt = txtSDT.Text.Trim();

            if (string.IsNullOrWhiteSpace(tenKH))
            {
                MessageBox.Show("Vui lòng nhập tên khách hàng!");
                return;
            }

            string maKH;
            string maHD;

            using SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();
            SqlTransaction tran = conn.BeginTransaction();

            try
            {
               

                string sqlMaKH = @"
                    SELECT ISNULL(MAX(CAST(SUBSTRING(MaKH, 3, LEN(MaKH)) AS INT)), 0) + 1
                    FROM KHACHHANG";

                using (SqlCommand cmd = new SqlCommand(sqlMaKH, conn, tran))
                {
                    int nextKH = Convert.ToInt32(cmd.ExecuteScalar());
                    maKH = "KH" + nextKH.ToString("000");
                }

                string sqlKH = @"
                    INSERT INTO KHACHHANG(MaKH, TenKH, DiaChi, SDT)
                    VALUES(@MaKH, @TenKH, @DiaChi, @SDT)";

                using (SqlCommand cmd = new SqlCommand(sqlKH, conn, tran))
                {
                    cmd.Parameters.AddWithValue("@MaKH", maKH);
                    cmd.Parameters.AddWithValue("@TenKH", tenKH);
                    cmd.Parameters.AddWithValue("@DiaChi", diaChi);
                    cmd.Parameters.AddWithValue("@SDT", sdt);
                    cmd.ExecuteNonQuery();
                }

               

                string sqlMaHD = @"
                    SELECT ISNULL(MAX(CAST(SUBSTRING(MaHD, 3, LEN(MaHD)) AS INT)), 0) + 1
                    FROM HOADON";

                using (SqlCommand cmd = new SqlCommand(sqlMaHD, conn, tran))
                {
                    int nextHD = Convert.ToInt32(cmd.ExecuteScalar());
                    maHD = "HD" + nextHD.ToString("000");
                }

                string sqlHD = @"
                    INSERT INTO HOADON(MaHD, MaKH, MaNV, NgayLapHD, ThanhTien)
                    VALUES(@MaHD, @MaKH, @MaNV, @NgayLapHD, 0)";

                using (SqlCommand cmd = new SqlCommand(sqlHD, conn, tran))
                {
                    cmd.Parameters.AddWithValue("@MaHD", maHD);
                    cmd.Parameters.AddWithValue("@MaKH", maKH);
                    cmd.Parameters.AddWithValue("@MaNV", "NV001");
                    cmd.Parameters.AddWithValue("@NgayLapHD", DateTime.Now);
                    cmd.ExecuteNonQuery();
                }

               

                decimal tongTien = 0;

                foreach (var item in vm.OrderDetails)
                {
                   

                    if (string.IsNullOrWhiteSpace(item.TenSP) || item.SoLuong <= 0)
                        continue;

                    string maSP;
                    decimal donGia;

                    string sqlSP = @"
                        SELECT MaSP, GiaBan
                        FROM SANPHAM
                        WHERE TenSP = @TenSP";

                    using (SqlCommand cmd = new SqlCommand(sqlSP, conn, tran))
                    {
                        cmd.Parameters.AddWithValue("@TenSP", item.TenSP);

                        using SqlDataReader rd = cmd.ExecuteReader();
                        if (!rd.Read())
                            continue;

                        maSP = rd["MaSP"].ToString();
                        donGia = Convert.ToDecimal(rd["GiaBan"]);
                    }

                    string sqlCT = @"
                        INSERT INTO CTHD(MaHD, MaSP, SoLuongMua, DonGiaSP)
                        VALUES(@MaHD, @MaSP, @SoLuongMua, @DonGiaSP)";

                    using (SqlCommand cmd = new SqlCommand(sqlCT, conn, tran))
                    {
                        cmd.Parameters.AddWithValue("@MaHD", maHD);
                        cmd.Parameters.AddWithValue("@MaSP", maSP);
                        cmd.Parameters.AddWithValue("@SoLuongMua", item.SoLuong);
                        cmd.Parameters.AddWithValue("@DonGiaSP", donGia);
                        cmd.ExecuteNonQuery();
                    }

                    tongTien += donGia * item.SoLuong;
                }

             

                string sqlUpdate = @"
                    UPDATE HOADON
                    SET ThanhTien = @ThanhTien
                    WHERE MaHD = @MaHD";

                using (SqlCommand cmd = new SqlCommand(sqlUpdate, conn, tran))
                {
                    cmd.Parameters.AddWithValue("@ThanhTien", tongTien);
                    cmd.Parameters.AddWithValue("@MaHD", maHD);
                    cmd.ExecuteNonQuery();
                }

                tran.Commit();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                MessageBox.Show("Lỗi: " + ex.Message);
                return;
            }

           
            var main = (MainWindow)Application.Current.MainWindow;
            main.ShowOverlay(new PageCTHD(maHD));
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            if (DataContext is OrderVM vm)
            {
                if (vm.DeleteOrderCommand.CanExecute(null))
                {
                    vm.DeleteOrderCommand.Execute(null);
                }
            }
        }

    }
}
