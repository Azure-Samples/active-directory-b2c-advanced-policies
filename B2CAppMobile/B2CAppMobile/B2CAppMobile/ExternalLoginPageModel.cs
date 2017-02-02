using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace B2CAppMobile
{
    public class ExternalLoginPageModel
    {
        private readonly ILogInPage _page;

        public ExternalLoginPageModel(ILogInPage page)
        {
            if (page == null)
            {
                throw new ArgumentNullException(nameof(page));
            }

            _page = page;

            RunCommand = new Command(async () =>
            {
                await _page.RunPolicyAsync(Authority, Scope, AuthenticationType);
            });
        }

        public string AuthenticationType { get; set; }

        public string Authority { get; set; }

        public string Caption { get; set; }

        public ICommand RunCommand { get; }

        public string[] Scope { get; set; }
    }
}
