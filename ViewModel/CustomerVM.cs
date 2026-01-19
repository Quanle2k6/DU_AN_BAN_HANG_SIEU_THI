using Page_Navigation_App.Database;
using Page_Navigation_App.Model;
using Page_Navigation_App.Utilities;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Page_Navigation_App.ViewModel
{
    class CustomerVM : Utilities.ViewModelBase
    {
        // --- Properties ---
        private ObservableCollection<SanPham> _products;
        public ObservableCollection<SanPham> Products
        {
            get => _products;
            set { _products = value; OnPropertyChanged(); }
        }

        private string _maSP;
        public string MaSP { get => _maSP; set { _maSP = value; OnPropertyChanged(); } }

        private string _tenSP;
        public string TenSP { get => _tenSP; set { _tenSP = value; OnPropertyChanged(); } }

        private string _maLSP;
        public string MaLSP { get => _maLSP; set { _maLSP = value; OnPropertyChanged(); } }

        private int _soLuongCon;
        public int SoLuongCon { get => _soLuongCon; set { _soLuongCon = value; OnPropertyChanged(); } }
        private string _donViTinh;
        public string DonViTinh { get => _donViTinh; set { _donViTinh = value; OnPropertyChanged(); } }

        private decimal _giaBan;
        public decimal GiaBan
        {
            get => _giaBan;
            set { _giaBan = value; OnPropertyChanged(); }
        }

        public string GiaBanString
        {
            get => _giaBan.ToString("N0"); // Hiển thị số có dấu phẩy: 1,000,000
            set
            {
                // 1. Loại bỏ dấu phẩy để lấy số thuần túy
                string cleanString = value.Replace(",", "").Replace(".", "");

                if (decimal.TryParse(cleanString, out decimal result))
                {
                    _giaBan = result;
                    // 2. Thông báo cập nhật để UI vẽ lại dấu phẩy đúng vị trí
                    OnPropertyChanged();
                }
                else if (string.IsNullOrEmpty(cleanString))
                {
                    _giaBan = 0;
                    OnPropertyChanged();
                }
            }
        }

        private string _searchText;
        public string SearchText { get => _searchText; set { _searchText = value; OnPropertyChanged(); } }

        private int _searchTypeIndex = 1;
        public int SearchTypeIndex { get => _searchTypeIndex; set { _searchTypeIndex = value; OnPropertyChanged(); } }

        private SanPham _selectedProduct;
        public SanPham SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                _selectedProduct = value;
                OnPropertyChanged();

                if (_selectedProduct != null)
                {
                    MaSP = _selectedProduct.MaSP;
                    TenSP = _selectedProduct.TenSP;
                    MaLSP = _selectedProduct.MaLSP;
                    SoLuongCon = _selectedProduct.SoLuongCon;
                    DonViTinh = _selectedProduct.DonViTinh;
                    
                    GiaBan = _selectedProduct.GiaBan;
                    OnPropertyChanged(nameof(GiaBanString));
                }
            }
        }

        // --- Commands ---
        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand ViewAllCommand { get; set; }
        public ICommand ClearCommand { get; set; }

        public CustomerVM()
        {
            Products = new ObservableCollection<SanPham>();

            AddCommand = new RelayCommand(p => ExecuteAdd());
            EditCommand = new RelayCommand(p => ExecuteEdit());
            DeleteCommand = new RelayCommand(p => ExecuteDelete());
            SearchCommand = new RelayCommand(p => ExecuteSearch());
            ViewAllCommand = new RelayCommand(p => LoadData());
            ClearCommand = new RelayCommand(p => ClearInputs());

            if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                LoadData();
            }
        }

        private void LoadData()
        {
            SearchText = "";
            // Query lấy đầy đủ thông tin để tính toán
            FetchDataFromSql("SELECT MaSP, TenSP, MaLSP, SoLuongCon, DonViTinh, GiaBan FROM SANPHAM");
        }

        private void ExecuteSearch()
        {
            if (string.IsNullOrWhiteSpace(SearchText)) { LoadData(); return; }

            string filterCol = "MaSP";
            if (SearchTypeIndex == 1) filterCol = "TenSP";
            else if (SearchTypeIndex == 2) filterCol = "MaLSP";

            string sql = $"SELECT MaSP, TenSP, MaLSP, SoLuongCon, DonViTinh, GiaBan FROM SANPHAM WHERE {filterCol} LIKE @search";
            SqlParameter[] parameters = { new SqlParameter("@search", SearchText + "%") };

            FetchDataFromSql(sql, parameters);
        }

        private void ExecuteAdd()
        {
            if (IsInputInvalid()) return;

            if (Products.Any(p => p.MaSP.Trim().Equals(MaSP.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("Mã sản phẩm đã tồn tại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string sql = "INSERT INTO SANPHAM (MaSP, TenSP, MaLSP, SoLuongCon, DonViTinh, GiaBan) VALUES (@ma, @ten, @malsp, 0, @dvt, @gia)";
            SqlParameter[] parameters = {
                new SqlParameter("@ma", MaSP),
                new SqlParameter("@ten", TenSP),
                new SqlParameter("@malsp", MaLSP ?? (object)DBNull.Value),
                new SqlParameter("@dvt", DonViTinh ?? (object)DBNull.Value),
                new SqlParameter("@gia", GiaBan)
            };

            if (DBConnection.ExecuteNonQuery(sql, parameters) > 0)
            {
                MessageBox.Show("Thêm sản phẩm thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadData();
                ClearInputs();
            }
        }

        private void ExecuteEdit()
        {
            if (string.IsNullOrWhiteSpace(MaSP))
            {
                MessageBox.Show("Vui lòng chọn sản phẩm cần sửa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (IsInputInvalid()) return;

            string sql = "UPDATE SANPHAM SET TenSP = @ten, MaLSP = @malsp, DonViTinh = @dvt, GiaBan = @gia WHERE MaSP = @ma";
            SqlParameter[] parameters = {
                new SqlParameter("@ten", TenSP),
                new SqlParameter("@malsp", MaLSP ?? (object)DBNull.Value),
                new SqlParameter("@gia", GiaBan),
                new SqlParameter("@dvt", DonViTinh ?? (object)DBNull.Value),
                new SqlParameter("@ma", MaSP)
            };

            if (DBConnection.ExecuteNonQuery(sql, parameters) > 0)
            {
                MessageBox.Show("Cập nhật thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadData();
            }
        }

        private void ExecuteDelete()
        {
            if (string.IsNullOrWhiteSpace(MaSP))
            {
                MessageBox.Show("Vui lòng chọn sản phẩm cần xóa!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Bạn có chắc muốn xóa sản phẩm {MaSP}?",
                "Xác nhận",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes) return;

            // ❗ MỖI LỆNH → MỖI SqlParameter MỚI
            DBConnection.ExecuteNonQuery(
                "DELETE FROM CHITIETNHAPHANG WHERE MaSP = @ma",
                new SqlParameter[] { new SqlParameter("@ma", MaSP) });

            DBConnection.ExecuteNonQuery(
                "DELETE FROM CTHD WHERE MaSP = @ma",
                new SqlParameter[] { new SqlParameter("@ma", MaSP) });

            int rows = DBConnection.ExecuteNonQuery(
                "DELETE FROM SANPHAM WHERE MaSP = @ma",
                new SqlParameter[] { new SqlParameter("@ma", MaSP) });

            if (rows > 0)
            {
                MessageBox.Show("Xóa sản phẩm thành công!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                LoadData();
                ClearInputs();
            }
        }


        private void FetchDataFromSql(string sql, SqlParameter[] parameters = null)
        {
            try
            {
                DataTable dt = DBConnection.ExecuteQuery(sql, parameters);

                // Nếu dt null, nghĩa là lớp DBConnection đã ném ngoại lệ
                if (dt == null)
                {
                    MessageBox.Show("Không thể lấy dữ liệu. Vui lòng kiểm tra lại kết nối Database trong App.config.");
                    return;
                }

                Products.Clear();
                foreach (DataRow row in dt.Rows)
                {
                    Products.Add(new SanPham
                    {
                        MaSP = row["MaSP"]?.ToString().Trim(),
                        TenSP = row["TenSP"]?.ToString(),
                        MaLSP = row["MaLSP"]?.ToString(),
                        // Ép kiểu an toàn hơn
                        SoLuongCon = row["SoLuongCon"] != DBNull.Value ? Convert.ToInt32(row["SoLuongCon"]) : 0,
                        DonViTinh = row["DonViTinh"]?.ToString(),
                        GiaBan = row["GiaBan"] != DBNull.Value ? Convert.ToDecimal(row["GiaBan"]) : 0
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi xử lý dữ liệu: " + ex.Message);
            }
        }

        private bool IsInputInvalid()
        {
            if (string.IsNullOrWhiteSpace(MaSP) || MaSP.Length > 10) // Điều chỉnh độ dài theo DB của bạn
            {
                MessageBox.Show("Mã sản phẩm không hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return true;
            }
            if (string.IsNullOrWhiteSpace(TenSP))
            {
                MessageBox.Show("Tên sản phẩm trống!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return true;
            }
            return false;
        }

        private void ClearInputs()
        {
            MaSP = string.Empty;
            TenSP = string.Empty;
            MaLSP = null;
            SoLuongCon = 0;
            GiaBan = 0;
            SelectedProduct = null;
            DonViTinh = null;
            GiaBanString = "0";
        }
    }
}