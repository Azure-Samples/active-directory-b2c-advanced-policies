using System.Collections.Generic;
using WingTipMusicWebApplication.Models;

namespace WingTipMusicWebApplication.ViewModels.Home
{
    public class IndexViewModel
    {
        public IEnumerable<Album> NewReleaseAlbums { get; set; }
    }
}
