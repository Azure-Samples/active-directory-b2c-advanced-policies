using System.Collections.Generic;
using WingTipMusicWebApplication.Models;

namespace WingTipMusicWebApplication.Repositories
{
    public static class Albums
    {
        static Albums()
        {
            NewReleaseAlbums = new List<Album>
            {
                new Album
                {
                    Artist = new Artist
                    {
                        Name = "Ed Sheran"
                    },
                    DiscountPrice = 1.79M,
                    ListenerGenres = new[]
                    {
                        Constants.ListenerGenres.Pop
                    },
                    ImageSource = "/images/ed-sheran_divide.jpg",
                    StandardPrice = 1.99M,
                    Title = "Divide"
                },
                new Album
                {
                    Artist = new Artist
                    {
                        Name = "Lorde"
                    },
                    DiscountPrice = 3.14M,
                    ListenerGenres = new[]
                    {
                        Constants.ListenerGenres.Pop
                    },
                    ImageSource = "/images/lorde_green-light.jpg",
                    StandardPrice = 3.49M,
                    Title = "Green Light"
                },
                new Album
                {
                    Artist = new Artist
                    {
                        Name = "Holy Holy"
                    },
                    DiscountPrice = 1.34M,
                    ListenerGenres = new[]
                    {
                        Constants.ListenerGenres.Rock
                    },
                    ImageSource = "/images/holy-holy_paint.jpg",
                    StandardPrice = 1.49M,
                    Title = "Paint"
                },
                new Album
                {
                    Artist = new Artist
                    {
                        Name = "Horrorshow"
                    },
                    ListenerGenres = new[]
                    {
                        Constants.ListenerGenres.HipHop
                    },
                    ImageSource = "/images/horrorshow_bardo-state.jpg",
                    StandardPrice = 0,
                    Title = "Bardo State"
                },
                new Album
                {
                    Artist = new Artist
                    {
                        Name = "Khalid"
                    },
                    ListenerGenres = new[]
                    {
                        Constants.ListenerGenres.RNB
                    },
                    ImageSource = "/images/khalid_american-teen.jpg",
                    StandardPrice = 0,
                    Title = "American Teen"
                },
                new Album
                {
                    Artist = new Artist
                    {
                        Name = "Stormzy"
                    },
                    DiscountPrice = 2.69M,
                    ListenerGenres = new[]
                    {
                        Constants.ListenerGenres.HipHop
                    },
                    ImageSource = "/images/stormzy_gang-signs-&-prayer.jpg",
                    StandardPrice = 2.99M,
                    Title = "Gang Signs & Prayer"
                },
                new Album
                {
                    Artist = new Artist
                    {
                        Name = "Flume"
                    },
                    ListenerGenres = new[]
                    {
                        Constants.ListenerGenres.Electronic
                    },
                    ImageSource = "/images/flume_skin-companion-ep-ii.jpg",
                    StandardPrice = 0,
                    Title = "Skin Companion EP II"
                },
                new Album
                {
                    Artist = new Artist
                    {
                        Name = "Gordy"
                    },
                    ListenerGenres = new[]
                    {
                        Constants.ListenerGenres.Rock
                    },
                    ImageSource = "/images/gordy-haab_halo-wars-2.jpg",
                    StandardPrice = 0,
                    Title = "Halo Wars 2"
                },
                new Album
                {
                    Artist = new Artist
                    {
                        Name = "Electric Guest"
                    },
                    ListenerGenres = new[]
                    {
                        Constants.ListenerGenres.Rock
                    },
                    ImageSource = "/images/electric-guest_plural.jpg",
                    StandardPrice = 0,
                    Title = "Plural"
                },
                new Album
                {
                    Artist = new Artist
                    {
                        Name = "Jess & Matt"
                    },
                    ListenerGenres = new[]
                    {
                        Constants.ListenerGenres.Pop
                    },
                    ImageSource = "/images/jess-&-matt_belmont-street.jpg",
                    StandardPrice = 0,
                    Title = "Belmont Street"
                }
            }; ;
        }

        public static IEnumerable<Album> NewReleaseAlbums { get; }
    }
}
