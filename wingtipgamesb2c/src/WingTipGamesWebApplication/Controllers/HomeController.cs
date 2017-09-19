using System;
using System.Linq;
using System.Threading.Tasks;
using WingTipGamesWebApplication.ViewModels.Home;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WingTipGamesWebApplication.Repositories;

namespace WingTipGamesWebApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly IGameRepository _gameRepository;

        public HomeController(IGameRepository gameRepository)
        {
            _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
        }

        [HttpPost]
        public IActionResult ChangeCulture(string newCulture)
        {
            if (!string.IsNullOrEmpty(newCulture))
            {
                Response.Cookies.Append(
                    Constants.CookieNames.CurrentLocale,
                    newCulture,
                    new CookieOptions
                    {
                        HttpOnly = true,
                        Path = "/",
                        Secure = false
                    });
            }
            else
            {
                Response.Cookies.Delete(Constants.CookieNames.CurrentLocale);
            }

            return Redirect("/");
        }

        [HttpPost]
        public IActionResult ChangeLocation(string newLocation)
        {
            if (!string.IsNullOrEmpty(newLocation))
            {
                Response.Cookies.Append(
                    Constants.CookieNames.CurrentLocation,
                    newLocation,
                    new CookieOptions
                    {
                        HttpOnly = true,
                        Path = "/",
                        Secure = false
                    });
            }
            else
            {
                Response.Cookies.Delete(Constants.CookieNames.CurrentLocation);
            }

            return Redirect("/");
        }

        public IActionResult Error()
        {
            return View();
        }

        public async Task<IActionResult> Index()
        {
            var newReleaseGames = await _gameRepository.GetNewReleaseGamesAsync();
            IndexViewModel viewModel;

            if (User.Identity.IsAuthenticated)
            {
                var playerZone = User.Identity.GetPlayerZone();

                if (!string.IsNullOrEmpty(playerZone))
                {
                    newReleaseGames = newReleaseGames.Where(g => g.PlayerZones.Contains(playerZone));
                }

                viewModel = new IndexViewModel
                {
                    NewReleaseGames = newReleaseGames
                };

                if (TempData.ContainsKey(Constants.TempDataKeys.IsNewUser))
                {
                    ViewBag.IsNewUser = true;
                    TempData.Remove(Constants.TempDataKeys.IsNewUser);
                }
            }
            else
            {
                viewModel = new IndexViewModel
                {
                    NewReleaseGames = newReleaseGames
                };
            }

            return View(viewModel);
        }
    }
}
