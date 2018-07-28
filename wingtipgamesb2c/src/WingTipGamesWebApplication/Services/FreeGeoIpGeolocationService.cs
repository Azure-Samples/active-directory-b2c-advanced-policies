using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WingTipGamesWebApplication.Services
{
    public class FreeGeoIpGeolocationService : IGeolocationService
    {
        private readonly string _apiKey;
        private readonly string _baseUrl;
        private readonly string _format;

        public FreeGeoIpGeolocationService(string baseUrl, string apiKey, string format)
        {
            _baseUrl = baseUrl;
            _apiKey = apiKey;
            _format = format;
        }

        public async Task<Location> GetLocationAsync(string ipAddress)
        {
            if (string.IsNullOrEmpty(ipAddress))
            {
                throw new ArgumentNullException(nameof(ipAddress));
            }

            using (var client = new HttpClient())
            {
                var requestUri = $"{_baseUrl}/{ipAddress}?access_key={_apiKey}&format={_format}";
                var response = await client.GetAsync(requestUri);
                response.EnsureSuccessStatusCode();
                var responseContentAsString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Location>(responseContentAsString);
            }
        }
    }
}
