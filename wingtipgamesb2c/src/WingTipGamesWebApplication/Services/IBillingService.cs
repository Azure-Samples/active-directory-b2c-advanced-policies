using System.Collections.Generic;
using System.Threading.Tasks;
using WingTipGamesWebApplication.Models;

namespace WingTipGamesWebApplication.Services
{
    public interface IBillingService
    {
        Task<IEnumerable<Order>> GetOrdersAsync(string accessToken);
    }
}
