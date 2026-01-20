using System;
using System.Windows.Media;

namespace Page_Navigation_App.Model
{
    public class KhuyenMaiItem
    {
        public string ProductId { get; set; }       

        public string ProductName { get; set; }      

        public DateTime StartDate { get; set; }      

        public DateTime EndDate { get; set; }        

        public int DiscountPercentage { get; set; }  


        public bool IsApplied { get; set; }

        public string Status => IsApplied ? "Đã áp dụng" : "Chưa áp dụng";

        public Brush StatusColor =>
            IsApplied ? Brushes.Green : Brushes.Gray;

        public Brush DiscountColor =>
            DiscountPercentage <= 5 ? Brushes.DodgerBlue : Brushes.OrangeRed;
    }
}
