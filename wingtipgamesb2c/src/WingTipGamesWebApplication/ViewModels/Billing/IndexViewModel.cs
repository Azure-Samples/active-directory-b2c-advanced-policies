using System.Collections.Generic;
using WingTipGamesWebApplication.Models;

namespace WingTipGamesWebApplication.ViewModels.Billing
{
    public class IndexViewModel
    {
        public IEnumerable<Order> Orders { get; set; }
    }
}
