using System.Collections.ObjectModel;

namespace Page_Navigation_App.Model
{
    public class CTHD
    {
        public string TenSP { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }

        public decimal ThanhTien => SoLuong * DonGia;
    }
}
