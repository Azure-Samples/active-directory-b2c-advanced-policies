using System;
using ITfoxtec.Identity.Saml2;
using ITfoxtec.Identity.Saml2.MvcCore;
using ITfoxtec.Identity.Saml2.Schemas;
using ITfoxtec.Identity.Saml2.Schemas.Metadata;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WingTipBillingWebApplication.Controllers
{
    [AllowAnonymous]
    public class MetadataController : Controller
    {
        private readonly Saml2Configuration _configuration;

        public MetadataController(Saml2Configuration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public IActionResult Index()
        {
            //var configuration = new Saml2Configuration();

            var entityDescriptor = new EntityDescriptor(_configuration)
            {
                ValidUntil = 365,
                SPSsoDescriptor = new SPSsoDescriptor
                {
                    WantAssertionsSigned = true,
                    SingleLogoutServices = new[]
                    {
                        new SingleLogoutService
                        {
                            Binding = ProtocolBindings.HttpPost,
                            Location = new Uri($"{Request.Scheme}://{Request.Host}{Url.Action("SingleLogout", "Account")}"),
                            ResponseLocation = new Uri($"{Request.Scheme}://{Request.Host}{Url.Action("LoggedOut", "Account")}")
                        }
                    },
                    NameIDFormats = new[]
                    {
                        NameIdentifierFormats.Persistent
                    },
                    AssertionConsumerServices = new[]
                    {
                        new AssertionConsumerService
                        {
                            Binding = ProtocolBindings.HttpPost,
                            Location = new Uri($"{Request.Scheme}://{Request.Host}{Url.Action("AssertionConsumer", "Account")}")
                        }
                    }
                }
            };

            var metadata = new Saml2Metadata(entityDescriptor);

            return metadata.CreateMetadata()
                .ToActionResult();
        }
    }
}
