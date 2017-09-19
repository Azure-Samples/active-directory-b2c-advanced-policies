using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json.Linq;

namespace WingTipCommon.Services
{
    public class GraphService : IGraphService
    {
        private const string GraphResource = "https://graph.windows.net";

        private readonly IAuthenticationService _authenticationService;
        private readonly ClientCredential _clientCredential;
        private readonly Uri _redirectUri;
        private readonly string _tenantId;

        public GraphService(
            IAuthenticationService authenticationService,
            string tenantId,
            string clientId,
            string clientSecret)
            : this(authenticationService, tenantId, clientId, clientSecret, null)
        {
        }

        public GraphService(
            IAuthenticationService authenticationService,
            string tenantId,
            string clientId,
            string clientSecret,
            Uri redirectUri)
        {
            if (string.IsNullOrEmpty(tenantId))
            {
                throw new ArgumentNullException(nameof(tenantId));
            }

            if (string.IsNullOrEmpty(clientId))
            {
                throw new ArgumentNullException(nameof(clientId));
            }

            if (string.IsNullOrEmpty(clientSecret))
            {
                throw new ArgumentNullException(nameof(clientSecret));
            }

            _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
            _tenantId = tenantId;
            _clientCredential = new ClientCredential(clientId, clientSecret);
            _redirectUri = redirectUri;
        }

        public async Task<JObject> CreateUserAsync(
            string userName,
            string password,
            string displayName,
            string activationStatus,
            string playerTag,
            string termsOfServiceConsented)
        {
            if (string.IsNullOrEmpty(displayName))
            {
                displayName = "Unknown";
            }

            var requestContentAsString = $@"{{
    ""accountEnabled"": true,
    ""signInNames"": [
        {{
            ""type"": ""emailAddress"",
            ""value"": ""{userName}""
        }}
    ],
    ""creationType"": ""LocalAccount"",
    ""displayName"": ""{displayName}"",
    ""mailNickname"": ""Unknown"",
    ""passwordProfile"": {{
        ""password"": ""{password}"",
        ""forceChangePasswordNextLogin"": false
    }},
    ""passwordPolicies"": ""DisablePasswordExpiration"",
    ""extension_7f15ff45527446eb986b7622c13461b0_ActivationStatus"": ""{activationStatus}"",
    ""extension_7f15ff45527446eb986b7622c13461b0_PlayerTag"": ""{playerTag}"",
    ""extension_7f15ff45527446eb986b7622c13461b0_TermsOfServiceConsented"": ""{termsOfServiceConsented}""
}}";

            var responseContentAsString = await SendGraphPostRequestAsync(
                AcquireTokenForApplicationAsync,
                "users",
                "1.6",
                null,
                requestContentAsString);

            if (responseContentAsString != null)
            {
                return JObject.Parse(responseContentAsString);
            }

            return null;
        }

        public async Task DeleteUserAsync(string userName)
        {
            var userObject = await GetUserBySignInNameAsync(userName);

            if (userObject != null && ((JArray)userObject["value"]).Count > 0)
            {
                await SendGraphDeleteRequestAsync(AcquireTokenForApplicationAsync, $"users/{((JArray)userObject["value"])[0]["objectId"]}", "1.6");
            }
        }

        public async Task<JObject> GetAuditAsync(string activityType, int top, string userId)
        {
            var responseContentAsString = await SendGraphGetRequestAsync(
                () => AcquireTokenForUserAsync(userId),
                "activities/audit",
                "beta",
                $"$filter=activityType eq '{activityType}'&$top={top}");
            return JObject.Parse(responseContentAsString);
        }

        public async Task<JObject> GetReportAsync(string reportType, string userId)
        {
            var responseContentAsString = await SendGraphGetRequestAsync(
                () => AcquireTokenForUserAsync(userId),
                $"reports/{reportType}",
                "beta",
                null);
            return JObject.Parse(responseContentAsString);
        }

        public async Task ResetAsync(string userName)
        {
            var userObject = await GetUserBySignInNameAsync(userName);

            if (userObject != null && ((JArray)userObject["value"]).Count > 0)
            {
                var requestContentAsString = $@"{{
    ""extension_7f15ff45527446eb986b7622c13461b0_AuthyId"": """",
    ""extension_7f15ff45527446eb986b7622c13461b0_StrongAuthenticationAppSecretKey"": """",
    ""extension_7f15ff45527446eb986b7622c13461b0_StrongAuthenticationAppTimeStepMatched"": """"
}}";

                await SendGraphPatchRequestAsync(
                    AcquireTokenForApplicationAsync,
                    $"users/{((JArray)userObject["value"])[0]["objectId"]}",
                    "1.6",
                    null,
                    requestContentAsString);
            }
        }

        public async Task SetUserActivationStatusAsync(string userName, string activationStatus)
        {
            var userObject = await GetUserBySignInNameAsync(userName);

            if (userObject != null && ((JArray)userObject["value"]).Count > 0)
            {
                var requestContentAsString = $@"{{
    ""extension_7f15ff45527446eb986b7622c13461b0_ActivationStatus"": ""{activationStatus}""
}}";

                await SendGraphPatchRequestAsync(
                    AcquireTokenForApplicationAsync,
                    $"users/{((JArray)userObject["value"])[0]["objectId"]}",
                    "1.6",
                    null,
                    requestContentAsString);
            }
        }

        public async Task SetUserPasswordAsync(string userName, string password)
        {
            var userObject = await GetUserBySignInNameAsync(userName);

            if (userObject != null && ((JArray)userObject["value"]).Count > 0)
            {
                var requestContentAsString = $@"{{
    ""passwordProfile"": {{
        ""password"": ""{password}"",
        ""forceChangePasswordNextLogin"": false
    }},
    ""passwordPolicies"": ""DisablePasswordExpiration""
}}";

                await SendGraphPatchRequestAsync(
                    AcquireTokenForApplicationAsync,
                    $"users/{((JArray)userObject["value"])[0]["objectId"]}",
                    "1.6",
                    null,
                    requestContentAsString);
            }
        }

        private Task<AuthenticationResult> AcquireTokenForApplicationAsync()
        {
            return _authenticationService.AcquireTokenAsync(GraphResource, _clientCredential);
        }

        private Task<AuthenticationResult> AcquireTokenForUserAsync(string userId)
        {
            return _authenticationService.AcquireTokenAsync(
                GraphResource,
                _clientCredential.ClientId,
                _redirectUri,
                userId);
        }

        private async Task<JObject> GetUserBySignInNameAsync(string signInName)
        {
            var responseContentAsString = await SendGraphGetRequestAsync(
                AcquireTokenForApplicationAsync,
                "users",
                "1.6",
                $"$filter=signInNames/any(x:x/value eq '{signInName}')");

            if (responseContentAsString != null)
            {
                return JObject.Parse(responseContentAsString);
            }

            return null;
        }

        private Task SendGraphDeleteRequestAsync(Func<Task<AuthenticationResult>> acquireTokenAsync, string requestPath, string apiVersion)
        {
            return SendGraphRequestAsync(
                acquireTokenAsync,
                requestUrl => new HttpRequestMessage(HttpMethod.Delete, requestUrl),
                requestPath,
                apiVersion,
                null);
        }

        private Task<string> SendGraphGetRequestAsync(
            Func<Task<AuthenticationResult>> acquireTokenAsync,
            string requestPath,
            string apiVersion,
            string requestQuery)
        {
            return SendGraphRequestAsync(
                acquireTokenAsync,
                requestUrl => new HttpRequestMessage(HttpMethod.Get, requestUrl),
                requestPath,
                apiVersion,
                requestQuery);
        }

        private Task<string> SendGraphPatchRequestAsync(
            Func<Task<AuthenticationResult>> acquireTokenAsync,
            string requestPath,
            string apiVersion,
            string requestQuery,
            string requestContentAsString)
        {
            return SendGraphRequestAsync(
                acquireTokenAsync,
                requestUrl =>
                {
                    var request = new HttpRequestMessage(new HttpMethod("PATCH"), requestUrl)
                    {
                        Content = new StringContent(requestContentAsString, Encoding.UTF8, "application/json")
                    };

                    return request;
                },
                requestPath,
                apiVersion,
                requestQuery);
        }

        private Task<string> SendGraphPostRequestAsync(
            Func<Task<AuthenticationResult>> acquireTokenAsync,
            string requestPath,
            string apiVersion,
            string requestQuery,
            string requestContentAsString)
        {
            return SendGraphRequestAsync(
                acquireTokenAsync,
                requestUrl =>
                {
                    var request = new HttpRequestMessage(HttpMethod.Post, requestUrl)
                    {
                        Content = new StringContent(requestContentAsString, Encoding.UTF8, "application/json")
                    };

                    return request;
                },
                requestPath,
                apiVersion,
                requestQuery);
        }

        private async Task<string> SendGraphRequestAsync(
            Func<Task<AuthenticationResult>> acquireTokenAsync,
            Func<string, HttpRequestMessage> createRequest,
            string requestPath,
            string apiVersion,
            string requestQuery)
        {
            var authenticationResult = await acquireTokenAsync();

            using (var client = new HttpClient())
            {
                var requestUrl = $"https://graph.windows.net/{_tenantId}/{requestPath}?api-version={apiVersion}";

                if (!string.IsNullOrEmpty(requestQuery))
                {
                    requestUrl = requestUrl + $"&{requestQuery}";
                }

                var request = createRequest(requestUrl);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authenticationResult.AccessToken);
                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }

                throw new WebException();
            }
        }
    }
}
