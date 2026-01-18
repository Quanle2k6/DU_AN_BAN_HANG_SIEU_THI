using System.ComponentModel;

namespace Page_Navigation_App.Model
{
    public class ChiTietNhapHangModel : INotifyPropertyChanged
    {
        private int _soLuong;
        private decimal _giaNhap;

        public string MaSP { get; set; }

        public int SoLuongNhapHang
        {
            get => _soLuong;
            set
            {
                _soLuong = value;
                OnPropertyChanged(nameof(SoLuongNhapHang));
                OnPropertyChanged(nameof(ThanhTien));
            }
        }

        public decimal GiaNhap
        {
            get => _giaNhap;
            set
            {
                _giaNhap = value;
                OnPropertyChanged(nameof(GiaNhap));
                OnPropertyChanged(nameof(ThanhTien));
            }
        }

        public decimal ThanhTien => SoLuongNhapHang * GiaNhap;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

