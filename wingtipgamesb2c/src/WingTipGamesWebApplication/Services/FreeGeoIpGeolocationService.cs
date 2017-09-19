using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WingTipGamesWebApplication.Services
{
    public class FreeGeoIpGeolocationService : IGeolocationService
    {
        public async Task<Location> GetLocationAsync(string ipAddress)
        {
            if (string.IsNullOrEmpty(ipAddress))
            {
                throw new ArgumentNullException(nameof(ipAddress));
            }

            using (var client = new HttpClient())
            {
                var requestUri = $"http://freegeoip.net/json/{ipAddress}";
                var response = await client.GetAsync(requestUri);
                response.EnsureSuccessStatusCode();
                var responseContentAsString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Location>(responseContentAsString);
            }
        }
    }
}
