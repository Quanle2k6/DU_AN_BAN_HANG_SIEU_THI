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
            // 1. Kiểm tra để trống trường dữ liệu
            if (string.IsNullOrWhiteSpace(NewMaKM)
                || string.IsNullOrWhiteSpace(NewTenKM)
                || NewNgayBD == null
                || NewNgayKT == null)
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin bắt buộc!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 2. Kiểm tra logic ngày tháng
            if (NewNgayKT < NewNgayBD)
            {
                MessageBox.Show("Ngày kết thúc không thể trước ngày bắt đầu!", "Lỗi logic", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 3. Kiểm tra giá trị phần trăm giảm giá
            if (NewPhanTramGiam <= 0 || NewPhanTramGiam > 100)
            {
                MessageBox.Show("Phần trăm giảm giá phải nằm trong khoảng từ 1 đến 100!", "Dữ liệu không hợp lệ", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    // 4. Kiểm tra xem MaKM đã tồn tại chưa (Tránh lỗi Primary Key)
                    string checkSql = "SELECT COUNT(*) FROM KHUYENMAI WHERE MaKM = @MaKM";
                    using (SqlCommand checkCmd = new SqlCommand(checkSql, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@MaKM", NewMaKM);
                        int count = (int)checkCmd.ExecuteScalar();
                        if (count > 0)
                        {
                            MessageBox.Show($"Mã khuyến mãi '{NewMaKM}' đã tồn tại trong hệ thống!", "Trùng mã", MessageBoxButton.OK, MessageBoxImage.Stop);
                            return;
                        }
                    }

                    // 5. Thực hiện thêm mới nếu tất cả điều kiện thỏa mãn
                    string sql = @"
                INSERT INTO KHUYENMAI (MaKM, TenKM, NgBD, NgKT, PhanTramGiam)
                VALUES (@MaKM, @TenKM, @NgBD, @NgKT, @PhanTramGiam)";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@MaKM", NewMaKM);
                        cmd.Parameters.AddWithValue("@TenKM", NewTenKM);
                        cmd.Parameters.AddWithValue("@NgBD", NewNgayBD.Value);
                        cmd.Parameters.AddWithValue("@NgKT", NewNgayKT.Value);
                        cmd.Parameters.AddWithValue("@PhanTramGiam", NewPhanTramGiam);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Thêm chương trình khuyến mãi thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }

                ClearInput();
                LoadKhuyenMai();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi hệ thống: " + ex.Message, "Lỗi kết nối", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
