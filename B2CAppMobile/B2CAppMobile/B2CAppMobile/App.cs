using Microsoft.Identity.Client;
using Xamarin.Forms;

namespace B2CAppMobile
{
    public class App : Application
    {
        public static string Authority = "https://login.microsoftonline.com/b2ccharm.onmicrosoft.com";
        public static string ClientId = "7393f3b7-7c95-490e-b854-5cae8b7d160a";
        public static ExternalLoginInfo[] ExternalLogins;
        public static PublicClientApplication PublicClientApplication;
        public static string[] Scope = { ClientId };

        public App()
        {
            PublicClientApplication = new PublicClientApplication(Authority, ClientId);
            InitializeExternalLogins();
            var mainPage = new MainPage();
            MainPage = new NavigationPage(mainPage);
        }

        private static ExternalLoginInfo CreatePolicy(string authenticationType, string caption)
        {
            return new ExternalLoginInfo(
                Authority,
                authenticationType,
                caption,
                Scope);
        }

        private static void InitializeExternalLogins()
        {
            ExternalLogins = new[]
            {
                CreatePolicy("B2C_1_wingtiptoyssignup", "A basic sign-up or sign-in policy for local, Facebook, Google, and Microsoft accounts"),
                CreatePolicy("B2C_1A_SignupWithRest", "An advanced sign-up or sign-in policy that integrates with a REST API"),
                CreatePolicy("B2C_1A_SignupWithRestAndIdp", "An advanced sign-up or sign-in policy that integrates with a custom identity provider")
            };
        }
    }
}
