using System.Collections.Generic;
using System.Threading.Tasks;
using WingTipMusicWebApplication.Models;

namespace WingTipMusicWebApplication.Repositories
{
    public class AlbumRepository : IAlbumRepository
    {
        public Task<IEnumerable<Album>> GetNewReleaseAlbumsAsync()
        {
            return Task.FromResult(Albums.NewReleaseAlbums);
        }
    }
}
