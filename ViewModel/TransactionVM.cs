using Page_Navigation_App.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data.SqlClient;

namespace Page_Navigation_App.ViewModel
{
    public class TransactionVM : Utilities.ViewModelBase
    {
        private readonly string _connectionString =
    ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;


        public ObservableCollection<NhapHangModel> DanhSachNhapHang { get; set; }
        public ObservableCollection<ChiTietNhapHangModel> ChiTietNhapHangs { get; set; }
       
        private NhapHangModel _nhapHangDangChon;
        public NhapHangModel NhapHangDangChon
        {
            get => _nhapHangDangChon;
            set
            {
                _nhapHangDangChon = value;
                OnPropertyChanged();

                if (_nhapHangDangChon != null)
                    LoadChiTietNhapHang(_nhapHangDangChon.MaNH);
            }
        }
      
        public TransactionVM()
        {
            DanhSachNhapHang = new ObservableCollection<NhapHangModel>();
            ChiTietNhapHangs = new ObservableCollection<ChiTietNhapHangModel>();

            LoadDanhSachNhapHang();
        }

     
        private void LoadDanhSachNhapHang()
        {
            DanhSachNhapHang.Clear();

            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            string sql = @"
                SELECT 
                    nh.MaNH,
                    nh.MaNCC,
                    ncc.TenNCC,
                    ncc.SoTaiKhoan,
                    nh.NgGiao,
                    nh.HSD
                FROM NHAPHANG nh
                JOIN NHACUNGCAP ncc ON nh.MaNCC = ncc.MaNCC
                ORDER BY nh.NgGiao DESC";

            using var cmd = new SqlCommand(sql, conn);
            using var rd = cmd.ExecuteReader();

            while (rd.Read())
            {
                DanhSachNhapHang.Add(new NhapHangModel
                {
                    MaNH = rd["MaNH"].ToString(),
                    MaNCC = rd["MaNCC"].ToString(),
                    TenNCC = rd["TenNCC"].ToString(),
                    SoTaiKhoan = rd["SoTaiKhoan"].ToString(),
                    NgGiao = rd["NgGiao"] == DBNull.Value ? null : (DateTime?)rd["NgGiao"],
                    HSD = rd["HSD"] == DBNull.Value ? null : (DateTime?)rd["HSD"]
                });
            }
        }
        public void XoaNhapHang()
        {
            if (NhapHangDangChon == null)
            {
                System.Windows.MessageBox.Show("Vui lòng chọn phiếu nhập cần xóa");
                return;
            }

            if (System.Windows.MessageBox.Show(
                "Bạn có chắc muốn xóa phiếu nhập này?",
                "Xác nhận",
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Warning)
                != System.Windows.MessageBoxResult.Yes)
                return;

            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var tran = conn.BeginTransaction();

            try
            {
              

                var listCT = new List<(string MaSP, int SoLuong)>();

                var cmdGetCT = new SqlCommand(@"
            SELECT MaSP, SoLuongNhapHang
            FROM CHITIETNHAPHANG
            WHERE MaNH = @MaNH", conn, tran);

                cmdGetCT.Parameters.AddWithValue("@MaNH", NhapHangDangChon.MaNH);

                using (var rd = cmdGetCT.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        listCT.Add((
                            rd["MaSP"].ToString(),
                            Convert.ToInt32(rd["SoLuongNhapHang"])
                        ));
                    }
                }

              

                foreach (var ct in listCT)
                {
                    var cmdUpdateKho = new SqlCommand(@"
                UPDATE SANPHAM
                SET SoLuongCon = SoLuongCon - @SoLuong
                WHERE MaSP = @MaSP",
                        conn, tran);

                    cmdUpdateKho.Parameters.AddWithValue("@MaSP", ct.MaSP);
                    cmdUpdateKho.Parameters.AddWithValue("@SoLuong", ct.SoLuong);
                    cmdUpdateKho.ExecuteNonQuery();
                }

               

                var cmdCT = new SqlCommand(
                    "DELETE FROM CHITIETNHAPHANG WHERE MaNH = @MaNH",
                    conn, tran);
                cmdCT.Parameters.AddWithValue("@MaNH", NhapHangDangChon.MaNH);
                cmdCT.ExecuteNonQuery();


                var cmdNH = new SqlCommand(
                    "DELETE FROM NHAPHANG WHERE MaNH = @MaNH",
                    conn, tran);
                cmdNH.Parameters.AddWithValue("@MaNH", NhapHangDangChon.MaNH);
                cmdNH.ExecuteNonQuery();

                tran.Commit();

                LoadDanhSachNhapHang();
                ChiTietNhapHangs.Clear();

                System.Windows.MessageBox.Show("Xóa thành công");
            }
            catch (Exception ex)
            {
                tran.Rollback();
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        public ObservableCollection<NhapHangModel> GetNhapHangTheoNgay(
    DateTime tuNgay, DateTime denNgay)
        {
            var result = new ObservableCollection<NhapHangModel>();

            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            string sql = @"
        SELECT 
            nh.MaNH,
            nh.MaNCC,
            ncc.TenNCC,
            ncc.SoTaiKhoan,
            nh.NgGiao,
            nh.HSD
        FROM NHAPHANG nh
        JOIN NHACUNGCAP ncc ON nh.MaNCC = ncc.MaNCC
        WHERE nh.NgGiao >= @TuNgay 
          AND nh.NgGiao <= @DenNgay
        ORDER BY nh.NgGiao DESC";

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@TuNgay", tuNgay);
            cmd.Parameters.AddWithValue("@DenNgay", denNgay);

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                result.Add(new NhapHangModel
                {
                    MaNH = rd["MaNH"].ToString(),
                    MaNCC = rd["MaNCC"].ToString(),
                    TenNCC = rd["TenNCC"].ToString(),
                    SoTaiKhoan = rd["SoTaiKhoan"].ToString(),
                    NgGiao = rd["NgGiao"] == DBNull.Value ? null : (DateTime?)rd["NgGiao"],
                    HSD = rd["HSD"] == DBNull.Value ? null : (DateTime?)rd["HSD"]
                });
            }

            return result;
        }
      
        private void LoadChiTietNhapHang(string maNH)
        {
            ChiTietNhapHangs.Clear();

            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            string sql = @"
                SELECT MaSP, SoLuongNhapHang, GiaNhap
                FROM CHITIETNHAPHANG
                WHERE MaNH = @MaNH";

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@MaNH", maNH);

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                ChiTietNhapHangs.Add(new ChiTietNhapHangModel
                {
                    MaSP = rd["MaSP"].ToString(),
                    SoLuongNhapHang = Convert.ToInt32(rd["SoLuongNhapHang"]),
                    GiaNhap = Convert.ToDecimal(rd["GiaNhap"])
                });
            }
        }
    }
}
