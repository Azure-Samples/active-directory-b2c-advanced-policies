using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using B2CAppMvc.Models.Home;
using B2CAppMvc.Properties;

namespace B2CAppMvc.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = Settings.Default.AppName;
            var viewModel = CreateIndexViewModel();

            if (!string.IsNullOrEmpty(User.Identity.GetGamerZone()))
            {
                var gamerZone = User.Identity.GetGamerZone();
                viewModel.NewReleaseGames = viewModel.NewReleaseGames.Where(g => g.GamerZones.Contains(gamerZone)).ToList();
            }

            return View(viewModel);
        }

        private static IndexViewModel CreateIndexViewModel()
        {
            return new IndexViewModel
            {
                NewReleaseGames = new List<RentableGameViewModel>
                {
                    new RentableGameViewModel
                    {
                        DiscountPrice = 1.79M,
                        GamerZones = new[]
                        {
                            Constants.GamerZones.Recreation,
                            Constants.GamerZones.Underground
                        },
                        ImageSource = "/images/the-long-dark-game-preview.png",
                        StandardPrice = 1.99M,
                        Title = "The Long Dark"
                    },
                    new RentableGameViewModel
                    {
                        DiscountPrice = 3.14M,
                        GamerZones = new[]
                        {
                            Constants.GamerZones.Professional,
                            Constants.GamerZones.Underground
                        },
                        ImageSource = "/images/ark-survival-evolved-game-preview.png",
                        StandardPrice = 3.49M,
                        Title = "ARK: Survival Evolved"
                    },
                    new RentableGameViewModel
                    {
                        DiscountPrice = 1.34M,
                        GamerZones = new[]
                        {
                            Constants.GamerZones.Recreation,
                            Constants.GamerZones.Underground
                        },
                        ImageSource = "/images/the-solas-project-game-preview.png",
                        StandardPrice = 1.49M,
                        Title = "The Solus Project"
                    },
                    new RentableGameViewModel
                    {
                        GamerZones = new[]
                        {
                            Constants.GamerZones.Professional,
                            Constants.GamerZones.Recreation,
                            Constants.GamerZones.Underground
                        },
                        ImageSource = "/images/gigantic.png",
                        StandardPrice = 0,
                        Title = "Gigantic"
                    },
                    new RentableGameViewModel
                    {
                        GamerZones = new[]
                        {
                            Constants.GamerZones.Professional,
                            Constants.GamerZones.Recreation,
                            Constants.GamerZones.Underground
                        },
                        ImageSource = "/images/crackdown-3.png",
                        StandardPrice = 0,
                        Title = "Crackdown 3"
                    },
                    new RentableGameViewModel
                    {
                        DiscountPrice = 2.69M,
                        GamerZones = new[]
                        {
                            Constants.GamerZones.Professional,
                            Constants.GamerZones.Underground
                        },
                        ImageSource = "/images/prison-architect-xbox-one-edition-game-preview.png",
                        StandardPrice = 2.99M,
                        Title = "Prison Architect"
                    },
                    new RentableGameViewModel
                    {
                        GamerZones = new[]
                        {
                            Constants.GamerZones.Family
                        },
                        ImageSource = "/images/dovetail-games-euro-fishing.png",
                        StandardPrice = 0,
                        Title = "Dovetail Games Euro Fishing"
                    },
                    new RentableGameViewModel
                    {
                        GamerZones = new[]
                        {
                            Constants.GamerZones.Professional,
                            Constants.GamerZones.Recreation,
                            Constants.GamerZones.Underground
                        },
                        ImageSource = "/images/scalebound.png",
                        StandardPrice = 0,
                        Title = "Scalebound"
                    },
                    new RentableGameViewModel
                    {
                        GamerZones = new[]
                        {
                            Constants.GamerZones.Professional,
                            Constants.GamerZones.Recreation,
                            Constants.GamerZones.Underground
                        },
                        ImageSource = "/images/deus-ex-mankind-divided.png",
                        StandardPrice = 0,
                        Title = "Deus Ex: Mankind Divided"
                    },
                    new RentableGameViewModel
                    {
                        GamerZones = new[]
                        {
                            Constants.GamerZones.Professional,
                            Constants.GamerZones.Recreation,
                            Constants.GamerZones.Underground
                        },
                        ImageSource = "/images/recore.png",
                        StandardPrice = 0,
                        Title = "ReCore"
                    }
                }
            };
        }
    }
}
