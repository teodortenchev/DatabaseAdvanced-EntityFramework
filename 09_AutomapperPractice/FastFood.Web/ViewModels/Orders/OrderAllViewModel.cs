using System;
using System.Globalization;

namespace FastFood.Web.ViewModels.Orders
{
    public class OrderAllViewModel
    {
        public int OrderId { get; set; }

        public string Customer { get; set; }

        public string Employee { get; set; }

        public string DateTime { get; set; }

        public string FormattedDate => System.DateTime.ParseExact(DateTime, "dd-MM-yyyy hh:mm", CultureInfo.InvariantCulture).ToString();
        
    }
}
