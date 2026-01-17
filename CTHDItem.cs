using System.ComponentModel;

namespace Page_Navigation_App.Model
{
    public class CTHDItem : INotifyPropertyChanged
    {
        public int STT { get; set; }
        public string MaSP { get; set; }
        public string TenSanPham { get; set; }

        private int _soLuong;
        public int SoLuong
        {
            get => _soLuong;
            set
            {
                if (_soLuong != value)
                {
                    _soLuong = value;
                    OnPropertyChanged(nameof(SoLuong));
                    OnPropertyChanged(nameof(ThanhTien));
                }
            }
        }

        private decimal _donGia;
        public decimal DonGia
        {
            get => _donGia;
            set
            {
                if (_donGia != value)
                {
                    _donGia = value;
                    OnPropertyChanged(nameof(DonGia));
                    OnPropertyChanged(nameof(ThanhTien));
                }
            }
        }

        public decimal ThanhTien => SoLuong * DonGia;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}


