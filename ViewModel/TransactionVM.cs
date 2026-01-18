using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using Page_Navigation_App.Model;

namespace Page_Navigation_App.ViewModel
{
    public class TransactionVM : Utilities.ViewModelBase
    {
        private readonly string _connectionString =
            @"Server=(localdb)\MSSQLLocalDB;Database=QLBH;Integrated Security=True;";
        
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
