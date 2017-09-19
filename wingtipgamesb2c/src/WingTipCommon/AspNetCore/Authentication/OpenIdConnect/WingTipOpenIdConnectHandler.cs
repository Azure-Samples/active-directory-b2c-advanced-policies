using System.Net.Http;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http.Authentication;

namespace WingTipCommon.AspNetCore.Authentication.OpenIdConnect
{
    public class WingTipOpenIdConnectHandler : OpenIdConnectHandler
    {
        public WingTipOpenIdConnectHandler(HttpClient backchannel, HtmlEncoder htmlEncoder)
            : base(backchannel, htmlEncoder)
        {
        }

        protected override void GenerateCorrelationId(AuthenticationProperties authenticationProperties)
        {
            var skipCorrelation = SkipCorrelation(authenticationProperties);

            if (skipCorrelation)
            {
                return;
            }

            base.GenerateCorrelationId(authenticationProperties);
        }

        protected override bool ValidateCorrelationId(AuthenticationProperties authenticationProperties)
        {
            var skipCorrelation = SkipCorrelation(authenticationProperties);

            if (skipCorrelation)
            {
                return true;
            }

            return base.ValidateCorrelationId(authenticationProperties);
        }

        private static bool SkipCorrelation(AuthenticationProperties authenticationProperties)
        {
            if (authenticationProperties == null)
            {
                return false;
            }

            if (!authenticationProperties.Items.ContainsKey("skip_correlation"))
            {
                return false;
            }

            bool skipCorrelation;

            if (!bool.TryParse(authenticationProperties.Items["skip_correlation"], out skipCorrelation))
            {
                return false;
            }

            return skipCorrelation;
        }
    }
}
