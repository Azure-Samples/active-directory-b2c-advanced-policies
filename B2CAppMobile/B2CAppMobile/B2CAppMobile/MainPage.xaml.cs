using System.Text;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;

namespace B2CAppMobile
{
    public partial class MainPage : ContentPage, IMainPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        public IPlatformParameters PlatformParameters { get; set; }

        private MainPageModel Model => (MainPageModel)BindingContext;

        public Task EditProfileAsync()
        {
            return Task.FromResult(0);
        }

        public async Task LogInAsync()
        {
            var logInPage = new LogInPage();
            await Navigation.PushAsync(logInPage);
        }

        public Task LogOutAsync()
        {
            App.PublicClientApplication.UserTokenCache.Clear(App.ClientId);
            Model.SetUser(false, null, null);
            return Task.FromResult(0);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            App.PublicClientApplication.PlatformParameters = PlatformParameters;
            BindingContext = new MainPageModel(this);

            try
            {
                var authenticationResult = await App.PublicClientApplication.AcquireTokenSilentAsync(App.Scope);
                var claims = ReadToken(authenticationResult.IdToken);
                var gamerTagClaim = claims["extension_GamerTag"];
                var gamerZoneClaim = claims["extension_GamerZone"];
                Model.SetUser(true, gamerTagClaim != null ? $"@{gamerTagClaim.Value<string>()}" : null, gamerZoneClaim?.Value<string>());
            }
            catch
            {
                Model.SetUser(false, null, null);
            }
        }

        private static JObject ReadToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return null;
            }

            var tokenParts = token.Split('.');

            if (tokenParts.Length != 3)
            {
                return null;
            }

            var tokenPayload = Base64Url.Decode(tokenParts[1]);
            return JObject.Parse(Encoding.UTF8.GetString(tokenPayload, 0, tokenPayload.Length));
        }
    }
}
