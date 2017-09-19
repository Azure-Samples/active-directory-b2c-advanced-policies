using System.Threading.Tasks;

namespace WingTipGamesWebApplication.Services
{
    public interface IGeolocationService
    {
        Task<Location> GetLocationAsync(string ipAddress);
    }
}
