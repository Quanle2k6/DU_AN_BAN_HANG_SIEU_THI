using Page_Navigation_App.Model;
using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;

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
        private readonly PageModel _pageModel;

        public DateTime DisplayOrderDate
        {
            get => _pageModel.OrderDate;
            set
            {
                _pageModel.OrderDate = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<HoaDonView> Orders { get; set; }

        private string connStr =
            @"Server=HP_DEVICE;Database=QLBH;Integrated Security=True;";

        public OrderVM()
        {
            _pageModel = new PageModel
            {
                OrderDate = DateTime.Now
            };

            LoadHoaDon();
        }

        private void LoadHoaDon()
        {
            Orders = new ObservableCollection<HoaDonView>();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string sql = @"
                SELECT
                    hd.MaHD,
                    kh.TenKH,
                    kh.DiaChi,
                    kh.SDT,
                    sp.TenSP,
                    sp.GiaBan,
                    ct.SoLuongMua
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
    }
}
