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
    class CustomerVM : ViewModelBase
    {
        private ObservableCollection<SanPham> _products;
        public ObservableCollection<SanPham> Products
        {
            get => _products;
            set { _products = value; OnPropertyChanged(); }
        }

        private string _maSP;
        public string MaSP
        {
            get => _maSP;
            set { _maSP = value; OnPropertyChanged(); }
        }

        private string _tenSP;
        public string TenSP
        {
            get => _tenSP;
            set { _tenSP = value; OnPropertyChanged(); }
        }

        private string _maLSP;
        public string MaLSP
        {
            get => _maLSP;
            set { _maLSP = value; OnPropertyChanged(); }
        }

        private int _soLuongCon;
        public int SoLuongCon
        {
            get => _soLuongCon;
            private set
            {
                _soLuongCon = value;
                OnPropertyChanged();
            }
        }

        private string _donViTinh;
        public string DonViTinh
        {
            get => _donViTinh;
            set { _donViTinh = value; OnPropertyChanged(); }
        }

        private decimal _giaBan;
        public decimal GiaBan
        {
            get => _giaBan;
            set { _giaBan = value; OnPropertyChanged(); }
        }

        public string GiaBanString
        {
            get => _giaBan.ToString("N0");
            set
            {
                string clean = value?.Replace(",", "").Replace(".", "");
                if (decimal.TryParse(clean, out decimal result))
                {
                    _giaBan = result;
                    OnPropertyChanged(nameof(GiaBan));
                    OnPropertyChanged();
                }
            }
        }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set { _searchText = value; OnPropertyChanged(); }
        }

        private int _searchTypeIndex = 1;
        public int SearchTypeIndex
        {
            get => _searchTypeIndex;
            set { _searchTypeIndex = value; OnPropertyChanged(); }
        }

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

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand ViewAllCommand { get; }
        public ICommand ClearCommand { get; }

        public CustomerVM()
        {
            Products = new ObservableCollection<SanPham>();

            AddCommand = new RelayCommand(_ => ExecuteAdd());
            EditCommand = new RelayCommand(_ => ExecuteEdit());
            DeleteCommand = new RelayCommand(_ => ExecuteDelete());
            SearchCommand = new RelayCommand(_ => ExecuteSearch());
            ViewAllCommand = new RelayCommand(_ => LoadData());
            ClearCommand = new RelayCommand(_ => ClearInputs());

            if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                LoadData();
            }
        }

        private void LoadData()
        {
            SearchText = "";
            FetchDataFromSql(
                "SELECT MaSP, TenSP, MaLSP, SoLuongCon, DonViTinh, GiaBan FROM SANPHAM"
            );
        }

        private void FetchDataFromSql(string sql, SqlParameter[] parameters = null)
        {
            try
            {
                DataTable dt = DBConnection.ExecuteQuery(sql, parameters);
                if (dt == null) return;

                Products.Clear();

                foreach (DataRow row in dt.Rows)
                {
                    Products.Add(new SanPham
                    {
                        MaSP = row["MaSP"]?.ToString().Trim(),
                        TenSP = row["TenSP"]?.ToString(),
                        MaLSP = row["MaLSP"]?.ToString(),
                        SoLuongCon = row["SoLuongCon"] != DBNull.Value
                                        ? Convert.ToInt32(row["SoLuongCon"])
                                        : 0,
                        DonViTinh = row["DonViTinh"]?.ToString(),
                        GiaBan = row["GiaBan"] != DBNull.Value
                                    ? Convert.ToDecimal(row["GiaBan"])
                                    : 0
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi xử lý dữ liệu: " + ex.Message);
            }
        }

        private void ExecuteSearch()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                LoadData();
                return;
            }

            string col = SearchTypeIndex switch
            {
                1 => "TenSP",
                2 => "MaLSP",
                _ => "MaSP"
            };

            FetchDataFromSql(
                $"SELECT * FROM SANPHAM WHERE {col} LIKE @search",
                new[] { new SqlParameter("@search", SearchText + "%") }
            );
        }

        private void ExecuteAdd()
        {
            if (IsInputInvalid()) return;

            if (Products.Any(p => p.MaSP.Equals(MaSP, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("Mã sản phẩm đã tồn tại!");
                return;
            }

            DBConnection.ExecuteNonQuery(
                @"INSERT INTO SANPHAM (MaSP, TenSP, MaLSP, SoLuongCon, DonViTinh, GiaBan)
                  VALUES (@ma, @ten, @malsp, 0, @dvt, @gia)",
                new[]
                {
                    new SqlParameter("@ma", MaSP),
                    new SqlParameter("@ten", TenSP),
                    new SqlParameter("@malsp", (object)MaLSP ?? DBNull.Value),
                    new SqlParameter("@dvt", (object)DonViTinh ?? DBNull.Value),
                    new SqlParameter("@gia", GiaBan)
                }
            );
            MessageBox.Show("Thêm sản phẩm thành công");

            LoadData();
            ClearInputs();
        }

        private void ExecuteEdit()
        {
            if (SelectedProduct == null) return;

            DBConnection.ExecuteNonQuery(
                @"UPDATE SANPHAM
                  SET TenSP=@ten, MaLSP=@malsp, DonViTinh=@dvt, GiaBan=@gia
                  WHERE MaSP=@ma",
                new[]
                {
                    new SqlParameter("@ten", TenSP),
                    new SqlParameter("@malsp", (object)MaLSP ?? DBNull.Value),
                    new SqlParameter("@dvt", (object)DonViTinh ?? DBNull.Value),
                    new SqlParameter("@gia", GiaBan),
                    new SqlParameter("@ma", MaSP)
                }
            );

            LoadData();
        }

        private void ExecuteDelete()
        {
            if (SelectedProduct == null) return;

            if (MessageBox.Show("Xóa sản phẩm này?", "Xác nhận",
                MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;

            DBConnection.ExecuteNonQuery(
                "DELETE FROM CHITIETNHAPHANG WHERE MaSP=@ma",
                new[] { new SqlParameter("@ma", MaSP) });

            DBConnection.ExecuteNonQuery(
                "DELETE FROM CTHD WHERE MaSP=@ma",
                new[] { new SqlParameter("@ma", MaSP) });

            DBConnection.ExecuteNonQuery(
                "DELETE FROM SANPHAM WHERE MaSP=@ma",
                new[] { new SqlParameter("@ma", MaSP) });

            LoadData();
            ClearInputs();
        }

        private bool IsInputInvalid()
        {
            if (string.IsNullOrWhiteSpace(MaSP) || string.IsNullOrWhiteSpace(TenSP))
            {
                MessageBox.Show("Dữ liệu không hợp lệ");
                return true;
            }
            return false;
        }

        private void ClearInputs()
        {
            MaSP = "";
            TenSP = "";
            MaLSP = null;
            DonViTinh = null;
            GiaBan = 0;
            GiaBanString = "0";
            SoLuongCon = 0;
            SelectedProduct = null;
        }
    }
}
