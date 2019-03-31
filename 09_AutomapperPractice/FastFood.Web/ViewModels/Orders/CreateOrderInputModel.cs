using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;


namespace FastFood.Web.ViewModels.Orders
{
    public class CreateOrderInputModel
    {
        [Required]
        public string Customer { get; set; }

        public int ItemId { get; set; }

        public int EmployeeId { get; set; }

        
        [Range(1,1000)]
        public int Quantity { get; set; }

        public DateTime DateTime => DateTime.Now;
        
        [Required]
        public string Type { get; set; }
    }
}
