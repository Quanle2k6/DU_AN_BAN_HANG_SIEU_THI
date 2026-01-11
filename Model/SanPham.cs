using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Page_Navigation_App.Model
{
    public class SanPham
    {
        public string MaSP { get; set; }
        public string TenSP { get; set; }
        public string MaLSP { get; set; }
        public int SoLuongCon { get; set; }
        public decimal GiaBan { get; set; }

        public decimal ThanhTien
        {
            get { return SoLuongCon * GiaBan; }
        }
    }
}
