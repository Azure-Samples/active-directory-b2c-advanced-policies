using System.Collections.Generic;
using System.Threading.Tasks;
using WingTipGamesWebApplication.Models;

namespace WingTipGamesWebApplication.Services
{
    public interface IMusicService
    {
        Task<IEnumerable<Album>> GetNewReleaseAlbumsAsync(string accessToken);
    }
}
