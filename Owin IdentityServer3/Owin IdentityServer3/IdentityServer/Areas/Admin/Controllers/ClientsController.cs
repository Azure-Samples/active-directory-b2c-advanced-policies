using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using IdentityServer.Areas.Admin.ViewModels;
using IdentityServer.EntityFramework;
using IdentityServer3.Core.Models;
using IdentityServer3.EntityFramework.Entities;
using Client = IdentityServer3.EntityFramework.Entities.Client;

namespace IdentityServer.Areas.Admin.Controllers
{
    public class ClientsController : Controller
    {
        private CpimClientConfigurationDbContext _dbContext;

        public ClientsController()
        {
            _dbContext = new CpimClientConfigurationDbContext();
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateClientViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var client = new Client
                {
                    AbsoluteRefreshTokenLifetime = 2592000,
                    AccessTokenLifetime = 3600,
                    AllowAccessToAllScopes = true,
                    AllowAccessTokensViaBrowser = true,
                    AllowRememberConsent = true,
                    AuthorizationCodeLifetime = 300,
                    ClientId = $"https://login.microsoftonline.com/te/{viewModel.TenantName}",
                    ClientName = $"{viewModel.TenantName} (Authorization Code)",
                    Enabled = true,
                    EnableLocalLogin = true,
                    Flow = Flows.AuthorizationCode,
                    IdentityTokenLifetime = 300,
                    LogoutSessionRequired = true,
                    PrefixClientClaims = true,
                    RefreshTokenExpiration = TokenExpiration.Absolute,
                    RefreshTokenUsage = TokenUsage.OneTimeOnly,
                    SlidingRefreshTokenLifetime = 1296000,
                    ClientSecrets = new List<ClientSecret>
                    {
                        new ClientSecret
                        {
                            Value = "abcdef".Sha256()
                        }
                    },
                    RedirectUris = new List<ClientRedirectUri>
                    {
                        new ClientRedirectUri
                        {
                            Uri = $"https://login.microsoftonline.com/te/{viewModel.TenantName}/b2c_1a_base_extensions/oauth2/authresp"
                        }
                    }
                };

                _dbContext.Clients.Add(client);
                await _dbContext.SaveChangesAsync();
                return RedirectToAction("Index", "Clients");
            }

            return View(viewModel);
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var viewModel = (await _dbContext.Clients.OrderBy(c => c.ClientName).ToListAsync()).Select(CreateClientDisplayViewModel);
            return View(viewModel);
        }

        /// <summary>
        /// Releases unmanaged resources and optionally releases managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (_dbContext != null)
            {
                _dbContext.Dispose();
                _dbContext = null;
            }
        }

        private static ClientDisplayViewModel CreateClientDisplayViewModel(Client client)
        {
            if (client != null)
            {
                var clientDisplayViewModel = new ClientDisplayViewModel
                {
                    ClientId = client.ClientId,
                    ClientName = client.ClientName
                };

                if (client.RedirectUris != null && client.RedirectUris.Any())
                {
                    clientDisplayViewModel.RedirectUri = client.RedirectUris.First().Uri;
                }

                return clientDisplayViewModel;
            }

            return null;
        }
    }
}
