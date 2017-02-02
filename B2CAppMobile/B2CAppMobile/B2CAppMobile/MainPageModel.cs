using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xamarin.Forms;

namespace B2CAppMobile
{
    public class MainPageModel : INotifyPropertyChanged
    {
        private readonly IMainPage _page;

        private bool _isUserAuthenticated;
        private IEnumerable<RentableGamePageModel> _newReleaseGames;
        private string _userGamerTag;
        private string _userGamerZone;

        public MainPageModel(IMainPage page)
        {
            InitializeNewGameRentals();
            _page = page;
            EditProfileCommand = new Command(async () => await _page.EditProfileAsync());
            LogInCommand = new Command(async () => await _page.LogInAsync());
            LogOutCommand = new Command(async () => await _page.LogOutAsync());
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand EditProfileCommand { get; }

        public bool IsLoggedIn => _isUserAuthenticated;

        public bool IsLoggedOut => !_isUserAuthenticated;

        public ICommand LogInCommand { get; }

        public ICommand LogOutCommand { get; }

        public IEnumerable<RentableGamePageModel> NewReleaseGames
        {
            get
            {
                if (!string.IsNullOrEmpty(_userGamerZone))
                {
                    return _newReleaseGames.Where(g => g.GamerZones.Contains(_userGamerZone));
                }

                return _newReleaseGames;
            }
        }

        public string UserGamerTag
        {
            get { return _userGamerTag; }
            set
            {
                if (value != _userGamerTag)
                {
                    _userGamerTag = value;
                    OnPropertyChanged();
                }
            }
        }

        public void SetUser(
            bool isAuthenticated,
            string gamerTag,
            string gamerZone)
        {
            _isUserAuthenticated = isAuthenticated;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsLoggedIn"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsLoggedOut"));
            _userGamerTag = gamerTag;
            _userGamerZone = gamerZone;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NewReleaseGames"));
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void InitializeNewGameRentals()
        {
            _newReleaseGames = new List<RentableGamePageModel>
            {
                new RentableGamePageModel
                {
                    DiscountPrice = 1.79M,
                    GamerZones = new[]
                    {
                        Constants.GamerZones.Recreation,
                        Constants.GamerZones.Underground
                    },
                    ImageSource = ImageSource.FromFile("the_long_dark_game_preview.png"),
                    StandardPrice = 1.99M,
                    Title = "The Long Dark"
                },
                new RentableGamePageModel
                {
                    DiscountPrice = 3.14M,
                    GamerZones = new[]
                    {
                        Constants.GamerZones.Professional,
                        Constants.GamerZones.Underground
                    },
                    ImageSource = ImageSource.FromFile("ark_survival_evolved_game_preview.png"),
                    StandardPrice = 3.49M,
                    Title = "ARK: Survival Evolved"
                },
                new RentableGamePageModel
                {
                    DiscountPrice = 1.34M,
                    GamerZones = new[]
                    {
                        Constants.GamerZones.Recreation,
                        Constants.GamerZones.Underground
                    },
                    ImageSource = ImageSource.FromFile("the_solas_project_game_preview.png"),
                    StandardPrice = 1.49M,
                    Title = "The Solus Project"
                },
                new RentableGamePageModel
                {
                    GamerZones = new[]
                    {
                        Constants.GamerZones.Professional,
                        Constants.GamerZones.Recreation,
                        Constants.GamerZones.Underground
                    },
                    ImageSource = ImageSource.FromFile("gigantic.jpg"),
                    StandardPrice = 0,
                    Title = "Gigantic"
                },
                new RentableGamePageModel
                {
                    GamerZones = new[]
                    {
                        Constants.GamerZones.Professional,
                        Constants.GamerZones.Recreation,
                        Constants.GamerZones.Underground
                    },
                    ImageSource = ImageSource.FromFile("crackdown_3.png"),
                    StandardPrice = 0,
                    Title = "Crackdown 3"
                },
                new RentableGamePageModel
                {
                    DiscountPrice = 2.69M,
                    GamerZones = new[]
                    {
                        Constants.GamerZones.Professional,
                        Constants.GamerZones.Underground
                    },
                    ImageSource = ImageSource.FromFile("prison_architect_xbox_one_edition_game_preview.png"),
                    StandardPrice = 2.99M,
                    Title = "Prison Architect"
                },
                new RentableGamePageModel
                {
                    GamerZones = new[]
                    {
                        Constants.GamerZones.Family
                    },
                    ImageSource = ImageSource.FromFile("dovetail_games_euro_fishing.png"),
                    StandardPrice = 0,
                    Title = "Dovetail Games Euro Fishing"
                },
                new RentableGamePageModel
                {
                    GamerZones = new[]
                    {
                        Constants.GamerZones.Professional,
                        Constants.GamerZones.Recreation,
                        Constants.GamerZones.Underground
                    },
                    ImageSource = ImageSource.FromFile("scalebound.png"),
                    StandardPrice = 0,
                    Title = "Scalebound"
                },
                new RentableGamePageModel
                {
                    GamerZones = new[]
                    {
                        Constants.GamerZones.Professional,
                        Constants.GamerZones.Recreation,
                        Constants.GamerZones.Underground
                    },
                    ImageSource = ImageSource.FromFile("deus_ex_mankind_divided.png"),
                    StandardPrice = 0,
                    Title = "Deus Ex: Mankind Divided"
                },
                new RentableGamePageModel
                {
                    GamerZones = new[]
                    {
                        Constants.GamerZones.Professional,
                        Constants.GamerZones.Recreation,
                        Constants.GamerZones.Underground
                    },
                    ImageSource = ImageSource.FromFile("recore.png"),
                    StandardPrice = 0,
                    Title = "ReCore"
                }
            };
        }
    }
}
