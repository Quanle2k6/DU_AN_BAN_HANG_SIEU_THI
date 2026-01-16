using System;

namespace Page_Navigation_App.Model
{
    public class ChiTietNhapHangModel
    {
        public string MaSP { get; set; }

        public int SoLuongNhapHang { get; set; }

        public decimal GiaNhap { get; set; }

        public decimal ThanhTien
        {
            get { return SoLuongNhapHang * GiaNhap; }
        }
    }
}
