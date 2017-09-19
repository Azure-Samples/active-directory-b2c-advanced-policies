using System.Collections.Generic;
using WingTipGamesWebApplication.Models;

namespace WingTipGamesWebApplication.Repositories
{
    public static class Games
    {
        static Games()
        {
            NewReleaseGames = new List<Game>
            {
                new Game
                {
                    DiscountPrice = 1.79M,
                    PlayerZones = new[]
                    {
                        Constants.PlayerZones.Recreation,
                        Constants.PlayerZones.Underground
                    },
                    ImageSource = "https://wingtipgamesb2c.azurewebsites.net/images/the-long-dark-game-preview.png",
                    StandardPrice = 1.99M,
                    Title = "The Long Dark"
                },
                new Game
                {
                    DiscountPrice = 3.14M,
                    PlayerZones = new[]
                    {
                        Constants.PlayerZones.Professional,
                        Constants.PlayerZones.Underground
                    },
                    ImageSource = "https://wingtipgamesb2c.azurewebsites.net/images/ark-survival-evolved-game-preview.png",
                    StandardPrice = 3.49M,
                    Title = "ARK: Survival Evolved"
                },
                new Game
                {
                    DiscountPrice = 1.34M,
                    PlayerZones = new[]
                    {
                        Constants.PlayerZones.Recreation,
                        Constants.PlayerZones.Underground
                    },
                    ImageSource = "https://wingtipgamesb2c.azurewebsites.net/images/the-solas-project-game-preview.png",
                    StandardPrice = 1.49M,
                    Title = "The Solus Project"
                },
                new Game
                {
                    PlayerZones = new[]
                    {
                        Constants.PlayerZones.Professional,
                        Constants.PlayerZones.Recreation,
                        Constants.PlayerZones.Underground
                    },
                    ImageSource = "https://wingtipgamesb2c.azurewebsites.net/images/gigantic.png",
                    StandardPrice = 0,
                    Title = "Gigantic"
                },
                new Game
                {
                    PlayerZones = new[]
                    {
                        Constants.PlayerZones.Professional,
                        Constants.PlayerZones.Recreation,
                        Constants.PlayerZones.Underground
                    },
                    ImageSource = "https://wingtipgamesb2c.azurewebsites.net/images/crackdown-3.png",
                    StandardPrice = 0,
                    Title = "Crackdown 3"
                },
                new Game
                {
                    DiscountPrice = 2.69M,
                    PlayerZones = new[]
                    {
                        Constants.PlayerZones.Professional,
                        Constants.PlayerZones.Underground
                    },
                    ImageSource = "https://wingtipgamesb2c.azurewebsites.net/images/prison-architect-xbox-one-edition-game-preview.png",
                    StandardPrice = 2.99M,
                    Title = "Prison Architect"
                },
                new Game
                {
                    PlayerZones = new[]
                    {
                        Constants.PlayerZones.Family
                    },
                    ImageSource = "https://wingtipgamesb2c.azurewebsites.net/images/dovetail-games-euro-fishing.png",
                    StandardPrice = 0,
                    Title = "Dovetail Games Euro Fishing"
                },
                new Game
                {
                    PlayerZones = new[]
                    {
                        Constants.PlayerZones.Professional,
                        Constants.PlayerZones.Recreation,
                        Constants.PlayerZones.Underground
                    },
                    ImageSource = "https://wingtipgamesb2c.azurewebsites.net/images/scalebound.png",
                    StandardPrice = 0,
                    Title = "Scalebound"
                },
                new Game
                {
                    PlayerZones = new[]
                    {
                        Constants.PlayerZones.Professional,
                        Constants.PlayerZones.Recreation,
                        Constants.PlayerZones.Underground
                    },
                    ImageSource = "https://wingtipgamesb2c.azurewebsites.net/images/deus-ex-mankind-divided.png",
                    StandardPrice = 0,
                    Title = "Deus Ex: Mankind Divided"
                },
                new Game
                {
                    PlayerZones = new[]
                    {
                        Constants.PlayerZones.Professional,
                        Constants.PlayerZones.Recreation,
                        Constants.PlayerZones.Underground
                    },
                    ImageSource = "https://wingtipgamesb2c.azurewebsites.net/images/recore.png",
                    StandardPrice = 0,
                    Title = "ReCore"
                }
            };
        }

        public static IEnumerable<Game> NewReleaseGames { get; }
    }
}
