using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WingTipGamesWebApplication.Repositories;

namespace WingTipGamesWebApplication.Api.Controllers
{
    [Authorize(ActiveAuthenticationSchemes = Constants.AuthenticationSchemes.Bearer, Policy = Constants.AuthorizationPolicies.ReadGames)]
    [Route("api/[controller]")]
    public class GamesController : Controller
    {
        private readonly IGameRepository _gameRepository;

        public GamesController(IGameRepository gameRepository)
        {
            _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
        }

        [Route("newrelease")]
        public async Task<IActionResult> GetNewReleaseGames()
        {
            var newReleaseGames = await _gameRepository.GetNewReleaseGamesAsync();
            var playerZone = User.Identity.GetPlayerZone();

            if (!string.IsNullOrEmpty(playerZone))
            {
                newReleaseGames = newReleaseGames.Where(a => a.PlayerZones.Contains(playerZone));
            }

            return Ok(newReleaseGames);
        }
    }
}
