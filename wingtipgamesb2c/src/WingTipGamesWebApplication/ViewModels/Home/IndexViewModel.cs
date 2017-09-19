using System.Collections.Generic;
using WingTipGamesWebApplication.Models;

namespace WingTipGamesWebApplication.ViewModels.Home
{
    public class IndexViewModel
    {
        public IEnumerable<Game> NewReleaseGames { get; set; }
    }
}
