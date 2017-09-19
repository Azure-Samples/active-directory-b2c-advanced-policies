using System.Collections.Generic;
using System.Threading.Tasks;
using WingTipGamesWebApplication.Models;

namespace WingTipGamesWebApplication.Repositories
{
    public class GameRepository : IGameRepository
    {
        public Task<IEnumerable<Game>> GetNewReleaseGamesAsync()
        {
            return Task.FromResult(Games.NewReleaseGames);
        }
    }
}
