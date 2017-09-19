using System.Collections.Generic;
using System.Threading.Tasks;
using WingTipMusicWebApplication.Models;

namespace WingTipMusicWebApplication.Repositories
{
    public interface IAlbumRepository
    {
        Task<IEnumerable<Album>> GetNewReleaseAlbumsAsync();
    }
}
