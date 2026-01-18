using System;

namespace Page_Navigation_App.Model
{
    public class NhanVienModel
    {
        public string MaNV { get; set; }
        public string TenNV { get; set; }
        public string GioiTinh { get; set; }
        public DateTime? NgSinh { get; set; }
        public DateTime? NgBDLamViec { get; set; }
        public decimal Luong { get; set; }
    }
}
