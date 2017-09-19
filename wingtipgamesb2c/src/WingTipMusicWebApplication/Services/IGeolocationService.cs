using System.Threading.Tasks;

namespace WingTipMusicWebApplication.Services
{
    public interface IGeolocationService
    {
        Task<Location> GetLocationAsync(string ipAddress);
    }
}
