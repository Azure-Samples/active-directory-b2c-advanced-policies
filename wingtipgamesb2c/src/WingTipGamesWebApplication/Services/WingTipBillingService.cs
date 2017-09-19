using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WingTipGamesWebApplication.Models;

namespace WingTipGamesWebApplication.Services
{
    public class WingTipBillingService : IBillingService, IDisposable
    {
        private HttpClient _client;

        public WingTipBillingService()
        {
            _client = new HttpClient();
        }

        public async Task<IEnumerable<Order>> GetOrdersAsync(string accessToken)
        {
            var requestUri = "https://wingtipbillingb2c.azurewebsites.net/api/orders";
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var responseContentAsString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IEnumerable<Order>>(responseContentAsString);
        }

        public void Dispose()
        {
            if (_client != null)
            {
                _client.Dispose();
                _client = null;
            }
        }
    }
}
