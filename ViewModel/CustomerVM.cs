using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Input;
using Page_Navigation_App.Model;
using Page_Navigation_App.Database; // Chứa class DBConnection của bạn
using Page_Navigation_App.Utilities; // Chứa RelayCommand

namespace Page_Navigation_App.ViewModel
{
    class CustomerVM : Utilities.ViewModelBase
    {
        private readonly PageModel _pageModel;

        // 1. Danh sách hiển thị trên DataGrid
        private ObservableCollection<SanPham> _products;
        public ObservableCollection<SanPham> Products
        {
            get => _products;
            set { _products = value; OnPropertyChanged(); }
        }

        // 2. Thuộc tính để Binding với các TextBox (Ví dụ cho MaSP và TenSP)
        private string _maSP;
        public string MaSP { get => _maSP; set { _maSP = value; OnPropertyChanged(); } }

        private string _tenSP;
        public string TenSP { get => _tenSP; set { _tenSP = value; OnPropertyChanged(); } }

        // Làm tương tự cho MaLSP, SoLuongCon, GiaBan...

        // 3. Các Commands cho nút bấm
        public ICommand AddCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand LoadCommand { get; set; }

        public CustomerVM()
        {
            _pageModel = new PageModel();
            Products = new ObservableCollection<SanPham>();

            // Khởi tạo các Command
            LoadCommand = new RelayCommand(p => LoadData());
            AddCommand = new RelayCommand(p => ExecuteAdd());
            DeleteCommand = new RelayCommand(p => ExecuteDelete());

            // Tự động load dữ liệu khi khởi tạo
            LoadData();
            
        }

        // Hàm Load dữ liệu từ DB
        private void LoadData()
        {
            try
            {
                Products.Clear();
                string sql = "SELECT * FROM SANPHAM";
                DataTable dt = DBConnection.ExecuteQuery(sql);

                if (dt == null) return;

                foreach (DataRow row in dt.Rows)
                {
                    Products.Add(new SanPham
                    {
                        MaSP = row["MaSP"].ToString(),
                        TenSP = row["TenSP"].ToString(),
                        MaLSP = row["MaLSP"].ToString(),
                        SoLuongCon = row["SoLuongCon"] != DBNull.Value ? Convert.ToInt32(row["SoLuongCon"]) : 0,
                        GiaBan = row["GiaBan"] != DBNull.Value ? Convert.ToDecimal(row["GiaBan"]) : 0
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể tải dữ liệu sản phẩm: " + ex.Message, "Lỗi kết nối", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Hàm Thêm dữ liệu
        private void ExecuteAdd()
        {
            // Kiểm tra trống
            if (string.IsNullOrEmpty(MaSP) || string.IsNullOrEmpty(TenSP))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ mã và tên sản phẩm!");
                return;
            }

            try
            {
                string sql = "INSERT INTO SANPHAM (MaSP, TenSP, MaLSP, SoLuongCon, GiaBan) VALUES (@ma, @ten, @malsp, @sl, @gia)";
                SqlParameter[] parameters = {
            new SqlParameter("@ma", MaSP),
            new SqlParameter("@ten", TenSP),
            new SqlParameter("@malsp", "L0001"), // Lưu ý: Dùng mã LSP có thật trong bộ dữ liệu mẫu (L0001)
            new SqlParameter("@sl", 0),
            new SqlParameter("@gia", 50000)
        };

                if (DBConnection.ExecuteNonQuery(sql, parameters) > 0)
                {
                    MessageBox.Show("Thêm thành công!");
                    LoadData();
                    // Xóa sạch TextBox sau khi thêm
                    MaSP = string.Empty;
                    TenSP = string.Empty;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm: " + ex.Message);
            }
        }

        private void ExecuteDelete()
        {
            // Logic xóa dựa trên MaSP đang nhập hoặc được chọn
        }
    }
}