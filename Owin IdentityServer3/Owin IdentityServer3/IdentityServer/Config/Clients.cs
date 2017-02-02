using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentityServer3.Core.Models;

namespace IdentityServer.Config
{
    public static class Clients
    {
        public static IEnumerable<Client> Get()
        {
            return new[]
             {
                new Client
                {
                     ClientId = "https://op.certification.openid.net:60591/authz_cb",
                     ClientName = "Openid connect certification",
                     Flow = Flows.AuthorizationCode,
                     AllowAccessToAllScopes = true,

                    // redirect = URI of the MVC application callback
                    RedirectUris = new List<string>
                    {
                        "https://op.certification.openid.net:60591/authz_cb"
                    }
                }
            };
        }
    }
}
