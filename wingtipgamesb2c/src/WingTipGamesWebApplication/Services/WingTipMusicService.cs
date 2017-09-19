using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WingTipGamesWebApplication.Models;

namespace WingTipGamesWebApplication.Services
{
    public class WingTipMusicService : IMusicService, IDisposable
    {
        private HttpClient _client;

        public WingTipMusicService()
        {
            _client = new HttpClient();
        }

        public async Task<IEnumerable<Album>> GetNewReleaseAlbumsAsync(string accessToken)
        {
            var requestUri = "https://wingtipmusicb2c.azurewebsites.net/api/albums/newrelease";
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IEnumerable<Album>>(responseContent);
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
