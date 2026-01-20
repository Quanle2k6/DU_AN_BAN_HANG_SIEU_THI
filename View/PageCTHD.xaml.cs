using Page_Navigation_App.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Configuration;


namespace Page_Navigation_App.View
{
    public partial class PageCTHD : UserControl, INotifyPropertyChanged
    {
        private readonly string connectionString =
      ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;


        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string prop)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        private string _maHD;
        public string MaHD
        {
            get => _maHD;
            set { _maHD = value; OnPropertyChanged(nameof(MaHD)); }
        }

        private string _tenKH;
        public string TenKH
        {
            get => _tenKH;
            set { _tenKH = value; OnPropertyChanged(nameof(TenKH)); }
        }

        private string _sdt;
        public string SDT
        {
            get => _sdt;
            set { _sdt = value; OnPropertyChanged(nameof(SDT)); }
        }

        private DateTime _ngayLap;
        public DateTime NgayLap
        {
            get => _ngayLap;
            set { _ngayLap = value; OnPropertyChanged(nameof(NgayLap)); }
        }

        private decimal _tongTien;
        public decimal TongTien
        {
            get => _tongTien;
            set { _tongTien = value; OnPropertyChanged(nameof(TongTien)); }
        }

   

        public ObservableCollection<CTHD> ChiTietHoaDon { get; set; }

        

        public PageCTHD(string maHD)
        {
            InitializeComponent();
            ChiTietHoaDon = new ObservableCollection<CTHD>();
            DataContext = this;
            MaHD = maHD;
            LoadHoaDon();
        }




        private void LoadHoaDon()
        {
            using SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();

           

            string sqlHD = @"
                SELECT kh.TenKH, kh.SDT, hd.NgayLapHD, hd.ThanhTien
                FROM HOADON hd
                JOIN KHACHHANG kh ON hd.MaKH = kh.MaKH
                WHERE hd.MaHD = @MaHD";

            using SqlCommand cmdHD = new SqlCommand(sqlHD, conn);
            cmdHD.Parameters.AddWithValue("@MaHD", MaHD);

            using SqlDataReader rd = cmdHD.ExecuteReader();
            if (rd.Read())
            {
                TenKH = rd["TenKH"].ToString();
                SDT = rd["SDT"].ToString();
                NgayLap = Convert.ToDateTime(rd["NgayLapHD"]);
                TongTien = Convert.ToDecimal(rd["ThanhTien"]);
            }
            rd.Close();


            ChiTietHoaDon.Clear(); 


            string sqlCT = @"
                SELECT sp.TenSP, ct.SoLuongMua, ct.DonGiaSP
                FROM CTHD ct
                JOIN SANPHAM sp ON ct.MaSP = sp.MaSP
                WHERE ct.MaHD = @MaHD";

            using SqlCommand cmdCT = new SqlCommand(sqlCT, conn);
            cmdCT.Parameters.AddWithValue("@MaHD", MaHD);

            using SqlDataReader rdCT = cmdCT.ExecuteReader();
            while (rdCT.Read())
            {
                ChiTietHoaDon.Add(new CTHD
                {
                    TenSP = rdCT["TenSP"].ToString(),
                    SoLuong = Convert.ToInt32(rdCT["SoLuongMua"]),
                    DonGia = Convert.ToDecimal(rdCT["DonGiaSP"])
                });
            }
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var main = (MainWindow)Application.Current.MainWindow;
            main.HideOverlay();
        }
    }
}
