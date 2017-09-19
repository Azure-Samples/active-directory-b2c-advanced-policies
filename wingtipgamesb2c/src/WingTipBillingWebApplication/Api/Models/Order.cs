using System;

namespace WingTipBillingWebApplication.Api.Models
{
    public class Order
    {
        public string Id { get; set; }

        public DateTime Date { get; set; }

        public string Description { get; set; }

        public string Status { get; set; }

        public decimal TotalPrice { get; set; }
    }
}
