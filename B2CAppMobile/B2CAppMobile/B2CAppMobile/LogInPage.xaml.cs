using System;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using Xamarin.Forms;

namespace B2CAppMobile
{
    public partial class LogInPage : ContentPage, ILogInPage
    {
        public LogInPage()
        {
            InitializeComponent();
        }

        public IPlatformParameters PlatformParameters { get; set; }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            App.PublicClientApplication.PlatformParameters = PlatformParameters;
            BindingContext = new LogInPageModel(this);
        }

        public async Task RunPolicyAsync(
            string authority,
            string[] scope,
            string policy)
        {
            try
            {
                await App.PublicClientApplication.AcquireTokenAsync(
                    scope,
                    string.Empty,
                    UiOptions.SelectAccount,
                    string.Empty,
                    null,
                    authority,
                    policy);
                await Navigation.PopAsync();
            }
            catch (MsalException ex)
            {
                if (ex.ErrorCode != "authentication_canceled")
                {
                    await DisplayError(ex);
                }
            }
            catch (Exception ex)
            {
                await DisplayError(ex);
            }
        }

        private Task DisplayError(Exception exception)
        {
            return DisplayAlert("An unexpected error has occurred", exception.Message, "Dismiss");
        }
    }
}
