using Android.App;
using B2CAppMobile;
using B2CAppMobile.Droid;
using Microsoft.Identity.Client;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(LogInPage), typeof(LogInPageRenderer))]
namespace B2CAppMobile.Droid
{
    public class LogInPageRenderer : PageRenderer
    {
        private LogInPage _page;

        protected override void OnElementChanged(ElementChangedEventArgs<Page> e)
        {
            base.OnElementChanged(e);
            _page = e.NewElement as LogInPage;
            var activity = Context as Activity;
            _page.PlatformParameters = new PlatformParameters(activity);
        }
    }
}