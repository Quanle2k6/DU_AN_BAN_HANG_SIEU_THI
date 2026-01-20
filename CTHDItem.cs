using System.ComponentModel;

namespace Page_Navigation_App.Model
{
    public class CTHDItem : INotifyPropertyChanged
    {
        public int STT { get; set; }

        private string _tenSP;
        public string TenSP   

        {
            get => _tenSP;
            set
            {
                if (_tenSP != value)
                {
                    _tenSP = value;
                    OnPropertyChanged(nameof(TenSP));
                }
            }
        }

        private int _soLuong = 1;
        public int SoLuong
        {
            get => _soLuong;
            set
            {
                if (_soLuong != value)
                {
                    _soLuong = value;
                    OnPropertyChanged(nameof(SoLuong));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
