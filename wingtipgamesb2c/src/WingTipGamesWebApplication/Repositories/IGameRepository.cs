using System.Collections.Generic;
using System.Threading.Tasks;
using WingTipGamesWebApplication.Models;

namespace WingTipGamesWebApplication.Repositories
{
    public interface IGameRepository
    {
        Task<IEnumerable<Game>> GetNewReleaseGamesAsync();
    }
}
