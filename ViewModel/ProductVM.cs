using Page_Navigation_App.Model;
using Page_Navigation_App.Utilities;
using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Input;

namespace Page_Navigation_App.ViewModel
{
    public class ProductVM : ViewModelBase
    {
        private static readonly string _connectionString =
            ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;

        

        public ObservableCollection<KhuyenMaiItem> StockItems { get; set; }

        private string _newMaKM;
        public string NewMaKM
        {
            get => _newMaKM;
            set { _newMaKM = value; OnPropertyChanged(); }
        }

        private string _newTenKM;
        public string NewTenKM
        {
            get => _newTenKM;
            set { _newTenKM = value; OnPropertyChanged(); }
        }

        private DateTime? _newNgayBD;
        public DateTime? NewNgayBD
        {
            get => _newNgayBD;
            set { _newNgayBD = value; OnPropertyChanged(); }
        }

        private DateTime? _newNgayKT;
        public DateTime? NewNgayKT
        {
            get => _newNgayKT;
            set { _newNgayKT = value; OnPropertyChanged(); }
        }

        private int _newPhanTramGiam;
        public int NewPhanTramGiam
        {
            get => _newPhanTramGiam;
            set { _newPhanTramGiam = value; OnPropertyChanged(); }
        }

        private int _newApDung;
        public int NewApDung
        {
            get => _newApDung;
            set { _newApDung = value; OnPropertyChanged(); }
        }

        public ProductVM()
        {
            StockItems = new ObservableCollection<KhuyenMaiItem>();
            AddKhuyenMaiCommand = new RelayCommand(AddKhuyenMai);
            DeleteKhuyenMaiCommand = new RelayCommand(DeleteKhuyenMai);
            LoadKhuyenMai();
        }
        public ICommand AddKhuyenMaiCommand { get; }

        private void LoadKhuyenMai()
        {
            StockItems.Clear();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string sql = @"
                    SELECT 
                        km.MaKM,
                        km.TenKM,
                        km.NgBD,
                        km.NgKT,
                        km.PhanTramGiam,
                        CASE 
                            WHEN EXISTS (
                                SELECT 1
                                FROM HOADON hd
                                WHERE hd.MaKM = km.MaKM
                                  AND hd.NgayLapHD BETWEEN km.NgBD AND km.NgKT
                            ) THEN 1
                            ELSE 0
                        END AS DaApDung
                    FROM KHUYENMAI km
                    ORDER BY km.NgBD DESC
                ";

                using SqlCommand cmd = new SqlCommand(sql, conn);
                using SqlDataReader rd = cmd.ExecuteReader();

                while (rd.Read())
                {
                    StockItems.Add(new KhuyenMaiItem
                    {
                        ProductId = rd["MaKM"].ToString(),
                        ProductName = rd["TenKM"].ToString(),
                        StartDate = Convert.ToDateTime(rd["NgBD"]),
                        EndDate = Convert.ToDateTime(rd["NgKT"]),
                        DiscountPercentage = Convert.ToInt32(rd["PhanTramGiam"]),
                        IsApplied = Convert.ToInt32(rd["DaApDung"]) == 1
                    });
                }
            }

            OnPropertyChanged(nameof(StockItems));
        }


        private void AddKhuyenMai(object obj)
        {
            if (string.IsNullOrWhiteSpace(NewMaKM)
                || string.IsNullOrWhiteSpace(NewTenKM)
                || NewNgayBD == null
                || NewNgayKT == null)
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin");
                return;
            }

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string sql = @"
            INSERT INTO KHUYENMAI
            (MaKM, TenKM, NgBD, NgKT, PhanTramGiam)
            VALUES
            (@MaKM, @TenKM, @NgBD, @NgKT, @PhanTramGiam)
        ";

                using SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MaKM", NewMaKM);
                cmd.Parameters.AddWithValue("@TenKM", NewTenKM);
                cmd.Parameters.AddWithValue("@NgBD", NewNgayBD.Value);
                cmd.Parameters.AddWithValue("@NgKT", NewNgayKT.Value);
                cmd.Parameters.AddWithValue("@PhanTramGiam", NewPhanTramGiam);

                cmd.ExecuteNonQuery();
            }

            ClearInput();
            LoadKhuyenMai();
        }

        private KhuyenMaiItem _selectedKhuyenMai;
        public KhuyenMaiItem SelectedKhuyenMai
        {
            get => _selectedKhuyenMai;
            set
            {
                _selectedKhuyenMai = value;
                OnPropertyChanged();
            }
        }

        public ICommand DeleteKhuyenMaiCommand { get; }



        private void DeleteKhuyenMai(object obj)
        {
            if (SelectedKhuyenMai == null)
            {
                MessageBox.Show("Vui lòng chọn khuyến mãi cần xóa");
                return;
            }

            var result = MessageBox.Show(
                $"Bạn có chắc muốn xóa khuyến mãi [{SelectedKhuyenMai.ProductName}]?",
                "Xác nhận xóa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string sql = "DELETE FROM KHUYENMAI WHERE MaKM = @MaKM";

                using SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MaKM", SelectedKhuyenMai.ProductId);
                cmd.ExecuteNonQuery();
            }

            LoadKhuyenMai();
        }


        private void ClearInput()
        {
            NewMaKM = string.Empty;
            NewTenKM = string.Empty;
            NewNgayBD = null;
            NewNgayKT = null;
            NewPhanTramGiam = 0;
            NewApDung = 0;
        }
    }
}
