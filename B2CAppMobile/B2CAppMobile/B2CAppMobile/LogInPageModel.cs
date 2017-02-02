using System.Collections.Generic;
using System.Linq;

namespace B2CAppMobile
{
    public class LogInPageModel
    {
        public LogInPageModel(ILogInPage page)
        {
            InitializePolicyListItems(page);
        }

        public IEnumerable<ExternalLoginPageModel> ExternalLogins { get; set; }

        private void InitializePolicyListItems(ILogInPage page)
        {
            ExternalLogins = App.ExternalLogins.Select(policy => new ExternalLoginPageModel(page)
            {
                Authority = policy.Authority,
                AuthenticationType = policy.AuthenticationType,
                Caption = policy.Caption,
                Scope = policy.Scope
            }).ToArray();
        }
    }
}
