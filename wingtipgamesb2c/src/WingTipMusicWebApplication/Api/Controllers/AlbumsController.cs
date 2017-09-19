using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WingTipMusicWebApplication.Repositories;

namespace WingTipMusicWebApplication.Api.Controllers
{
    [Authorize(ActiveAuthenticationSchemes = Constants.AuthenticationSchemes.Bearer, Policy = Constants.AuthorizationPolicies.ReadMusic)]
    [Route("api/[controller]")]
    public class AlbumsController : Controller
    {
        private readonly IAlbumRepository _albumRepository;

        public AlbumsController(IAlbumRepository albumRepository)
        {
            _albumRepository = albumRepository ?? throw new ArgumentNullException(nameof(albumRepository));
        }

        [Route("newrelease")]
        public async Task<IActionResult> GetNewReleaseAlbums()
        {
            var newReleaseAlbums = await _albumRepository.GetNewReleaseAlbumsAsync();
            var listenerGenre = User.Identity.GetListenerGenre();

            if (!string.IsNullOrEmpty(listenerGenre))
            {
                newReleaseAlbums = newReleaseAlbums.Where(a => a.ListenerGenres.Contains(listenerGenre));
            }

            return Ok(newReleaseAlbums);
        }
    }
}
