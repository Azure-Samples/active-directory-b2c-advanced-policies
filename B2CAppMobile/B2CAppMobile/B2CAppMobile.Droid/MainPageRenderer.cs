using Android.App;
using B2CAppMobile;
using B2CAppMobile.Droid;
using Microsoft.Identity.Client;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(MainPage), typeof(MainPageRenderer))]
namespace B2CAppMobile.Droid
{
    public class MainPageRenderer : PageRenderer
    {
        private MainPage _page;

        protected override void OnElementChanged(ElementChangedEventArgs<Page> e)
        {
            base.OnElementChanged(e);
            _page = e.NewElement as MainPage;
            var activity = Context as Activity;
            _page.PlatformParameters = new PlatformParameters(activity);
        }
    }
}
