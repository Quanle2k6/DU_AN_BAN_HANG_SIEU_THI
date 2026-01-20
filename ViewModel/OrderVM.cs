using Page_Navigation_App.Model;
using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Input;

namespace Page_Navigation_App.ViewModel
{

    public class SanPhamItem
    {
        public string TenSP { get; set; }
        public int SoLuong { get; set; }
        public decimal GiaBan { get; set; }
        public decimal ThanhTien => SoLuong * GiaBan;
    }

    public class HoaDonView
    {
        public int STT { get; set; }
        public string MaHD { get; set; }
        public string TenKH { get; set; }
        public string DiaChi { get; set; }
        public string SDT { get; set; }
        public ObservableCollection<SanPhamItem> MatHang { get; set; }
        public decimal TongTien => MatHang.Sum(x => x.ThanhTien);
    }



    public class OrderVM : Utilities.ViewModelBase
    {
        private string connStr =
    ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;


        public ObservableCollection<HoaDonView> Orders { get; set; }



        public ObservableCollection<SanPhamThem> OrderDetails { get; set; }


        public ObservableCollection<string> TenSanPhamList { get; set; }

        public ICommand CreateOrderCommand { get; }

        public OrderVM()
        {
            LoadHoaDon();
            LoadTenSanPham();

            OrderDetails = new ObservableCollection<SanPhamThem>
            {
                new SanPhamThem()
            };

            CreateOrderCommand = new Utilities.RelayCommand(CreateOrder);
            DeleteOrderCommand = new Utilities.RelayCommand(DeleteOrder);
            SearchOrderCommand = new Utilities.RelayCommand(SearchOrder);

        }
     


        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
            }
        }

        public ICommand SearchOrderCommand { get; }
        private void SearchOrder(object obj)
        {
            Orders.Clear();

            if (string.IsNullOrWhiteSpace(SearchText))
            {
                LoadHoaDon();
                return;
            }

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string sql = @"
        SELECT hd.MaHD, kh.TenKH, kh.DiaChi, kh.SDT,
               sp.TenSP, sp.GiaBan, ct.SoLuongMua
        FROM HOADON hd
        JOIN KHACHHANG kh ON hd.MaKH = kh.MaKH
        JOIN CTHD ct ON hd.MaHD = ct.MaHD
        JOIN SANPHAM sp ON ct.MaSP = sp.MaSP
        WHERE hd.MaHD LIKE @MaHD
        ORDER BY hd.MaHD";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MaHD", "%" + SearchText + "%");

                SqlDataReader rd = cmd.ExecuteReader();

                while (rd.Read())
                {
                    string maHD = rd["MaHD"].ToString();

                    var hd = Orders.FirstOrDefault(x => x.MaHD == maHD);

                    if (hd == null)
                    {
                        hd = new HoaDonView
                        {
                            STT = Orders.Count + 1,
                            MaHD = maHD,
                            TenKH = rd["TenKH"].ToString(),
                            DiaChi = rd["DiaChi"].ToString(),
                            SDT = rd["SDT"].ToString(),
                            MatHang = new ObservableCollection<SanPhamItem>()
                        };
                        Orders.Add(hd);
                    }

                    hd.MatHang.Add(new SanPhamItem
                    {
                        TenSP = rd["TenSP"].ToString(),
                        SoLuong = Convert.ToInt32(rd["SoLuongMua"]),
                        GiaBan = Convert.ToDecimal(rd["GiaBan"])
                    });
                }
            }

            if (Orders.Count == 0)
            {
                Orders.Add(new HoaDonView
                {
                    STT = 1,
                    MaHD = "Không tìm thấy",
                    TenKH = "",
                    DiaChi = "",
                    SDT = "",
                    MatHang = new ObservableCollection<SanPhamItem>()
                });
            }

            OnPropertyChanged(nameof(Orders));
        }


        private void LoadHoaDon()
        {
            Orders = new ObservableCollection<HoaDonView>();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string sql = @"
                SELECT hd.MaHD, kh.TenKH, kh.DiaChi, kh.SDT,
                       sp.TenSP, sp.GiaBan, ct.SoLuongMua
                FROM HOADON hd
                JOIN KHACHHANG kh ON hd.MaKH = kh.MaKH
                JOIN CTHD ct ON hd.MaHD = ct.MaHD
                JOIN SANPHAM sp ON ct.MaSP = sp.MaSP
                ORDER BY hd.MaHD";

                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader rd = cmd.ExecuteReader();

                while (rd.Read())
                {
                    string maHD = rd["MaHD"].ToString();

                    var hd = Orders.FirstOrDefault(x => x.MaHD == maHD);

                    if (hd == null)
                    {
                        hd = new HoaDonView
                        {
                            STT = Orders.Count + 1,
                            MaHD = maHD,
                            TenKH = rd["TenKH"].ToString(),
                            DiaChi = rd["DiaChi"].ToString(),
                            SDT = rd["SDT"].ToString(),
                            MatHang = new ObservableCollection<SanPhamItem>()
                        };
                        Orders.Add(hd);
                    }

                    hd.MatHang.Add(new SanPhamItem
                    {
                        TenSP = rd["TenSP"].ToString(),
                        SoLuong = Convert.ToInt32(rd["SoLuongMua"]),
                        GiaBan = Convert.ToDecimal(rd["GiaBan"])
                    });
                }
            }

            OnPropertyChanged(nameof(Orders));
        }

     

        private void LoadTenSanPham()
        {
            TenSanPhamList = new ObservableCollection<string>();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string sql = "SELECT TenSP FROM SANPHAM";

                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader rd = cmd.ExecuteReader();

                while (rd.Read())
                {
                    TenSanPhamList.Add(rd["TenSP"].ToString());
                }
            }

            OnPropertyChanged(nameof(TenSanPhamList));
        }

        public ObservableCollection<int> DanhSachSoLuong { get; } =
      new ObservableCollection<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

        private void CreateOrder(object obj)
        {
            if (!OrderDetails.Any(x => !string.IsNullOrEmpty(x.TenSP) && x.SoLuong > 0))
                return;

            var newHD = new HoaDonView
            {
                STT = Orders.Count + 1,
                MaHD = "HD" + DateTime.Now.Ticks,
                TenKH = "Khách mới",
                DiaChi = "",
                SDT = "",
                MatHang = new ObservableCollection<SanPhamItem>()
            };

            foreach (var sp in OrderDetails.Where(x => !string.IsNullOrEmpty(x.TenSP)))
            {
                newHD.MatHang.Add(new SanPhamItem
                {
                    TenSP = sp.TenSP,
                    SoLuong = sp.SoLuong,
                    GiaBan = 0
                });
            }

            Orders.Add(newHD);

            OrderDetails.Clear();
            OrderDetails.Add(new SanPhamThem());
        }

        private HoaDonView _selectedOrder;
        public HoaDonView SelectedOrder
        {
            get => _selectedOrder;
            set
            {
                _selectedOrder = value;
                OnPropertyChanged();
            }
        }
        public ICommand DeleteOrderCommand { get; }

        private void DeleteOrder(object obj)
        {
            if (SelectedOrder == null)
            {
                System.Windows.MessageBox.Show(
                    "Vui lòng chọn hóa đơn cần xóa",
                    "Thông báo",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Warning);
                return;
            }

            var result = System.Windows.MessageBox.Show(
                $"Bạn có chắc muốn xóa hóa đơn {SelectedOrder.MaHD} ?",
                "Xác nhận",
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Question);

            if (result != System.Windows.MessageBoxResult.Yes)
                return;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();

                try
                {


                    SqlCommand cmdCT = new SqlCommand(
                        "DELETE FROM CTHD WHERE MaHD = @MaHD",
                        conn, tran);
                    cmdCT.Parameters.AddWithValue("@MaHD", SelectedOrder.MaHD);
                    cmdCT.ExecuteNonQuery();



                    SqlCommand cmdHD = new SqlCommand(
                        "DELETE FROM HOADON WHERE MaHD = @MaHD",
                        conn, tran);
                    cmdHD.Parameters.AddWithValue("@MaHD", SelectedOrder.MaHD);
                    cmdHD.ExecuteNonQuery();

                    tran.Commit();
                }
                catch
                {
                    tran.Rollback();
                    throw;
                }
            }



            LoadHoaDon();

            System.Windows.MessageBox.Show(
                "Xóa hóa đơn thành công",
                "Thành công",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Information);
        }

    }
}
