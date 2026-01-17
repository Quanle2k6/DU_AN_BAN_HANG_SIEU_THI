using System;

namespace Page_Navigation_App.Model
{
    public class PageModel
    {
        public int CustomerCount { get; set; }
        public string ProductStatus { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TransactionValue { get; set; }
        public TimeOnly ShipmentDelivery { get; set; }
        public bool LocationStatus { get; set; }
    }
}

