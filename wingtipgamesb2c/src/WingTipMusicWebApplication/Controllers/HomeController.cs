using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using WingTipMusicWebApplication.ViewModels.Home;
using Microsoft.AspNetCore.Mvc;
using WingTipMusicWebApplication.Repositories;

namespace WingTipMusicWebApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAlbumRepository _albumRepository;

        public HomeController(IAlbumRepository albumRepository)
        {
            _albumRepository = albumRepository ?? throw new ArgumentNullException(nameof(albumRepository));
        }

        [HttpPost]
        public IActionResult ChangeCulture(string newCulture, [FromQuery] string returnUrl)
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
            var newReleaseAlbums = await _albumRepository.GetNewReleaseAlbumsAsync();
            IndexViewModel viewModel;

            if (User.Identity.IsAuthenticated)
            {
                var listenerGenre = User.Identity.GetListenerGenre();

                if (!string.IsNullOrEmpty(listenerGenre))
                {
                    newReleaseAlbums = newReleaseAlbums.Where(a => a.ListenerGenres.Contains(listenerGenre));
                }

                viewModel = new IndexViewModel
                {
                    NewReleaseAlbums = newReleaseAlbums
                };

                if (TempData.ContainsKey("IsNewUser"))
                {
                    ViewBag.IsNewUser = true;
                    TempData.Remove("IsNewUser");
                }
            }
            else
            {
                viewModel = new IndexViewModel
                {
                    NewReleaseAlbums = newReleaseAlbums
                };
            }

            return View(viewModel);
        }
    }
}
