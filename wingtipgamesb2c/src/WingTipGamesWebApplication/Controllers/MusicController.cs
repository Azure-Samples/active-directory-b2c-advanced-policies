using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WingTipGamesWebApplication.Services;
using WingTipGamesWebApplication.ViewModels.Music;

namespace WingTipGamesWebApplication.Controllers
{
    public class MusicController : Controller
    {
        private readonly IMusicService _albumsService;

        public MusicController(IMusicService albumsService)
        {
            _albumsService = albumsService ?? throw new ArgumentNullException(nameof(albumsService));
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var accessToken = await GetAccessTokenAsync();
            var newReleaseAlbums = await _albumsService.GetNewReleaseAlbumsAsync(accessToken);

            var viewModel = new IndexViewModel
            {
                NewReleaseAlbums = newReleaseAlbums
            };

            return View(viewModel);
        }

        private async Task<string> GetAccessTokenAsync()
        {
            var authenticateInfo = await HttpContext.Authentication.GetAuthenticateInfoAsync(Constants.AuthenticationSchemes.ApplicationCookie);
            return authenticateInfo.Properties.Items[".Token.access_token"];
        }
    }
}
