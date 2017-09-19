using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WingTipUserJourneyPlayerWebApplication.Api.Controllers
{
    [Route("api/[controller]")]
    public class StreamController : Controller
    {
        private const string DefaultTimeRange = "PT1H";

        private readonly string _applicationId;
        private readonly string _apiKey;

        public StreamController(AzureApplicationInsightsCredential applicationInsightsCredential)
        {
            if (applicationInsightsCredential == null)
            {
                throw new ArgumentNullException(nameof(applicationInsightsCredential));
            }

            _applicationId = applicationInsightsCredential.ApplicationId;
            _apiKey = applicationInsightsCredential.ApiKey;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Get(string timeRange = DefaultTimeRange)
        {
            using (var client = new HttpClient())
            {
                var requestUri = $"https://api.applicationinsights.io/beta/apps/{_applicationId}/events/traces?$filter=timestamp%20gt%20now()%20sub%20duration'{timeRange}'%20and%20customDimensions%2FEventName%20eq%20'Journey%20Recorder%20Event%20v1.0.0'&$orderby=timestamp";
                var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
                request.Headers.Add("x-api-key", _apiKey);
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var responseContentAsString = await response.Content.ReadAsStringAsync();
                Response.ContentType = "application/json";
                return Ok(responseContentAsString);
            }
        }
    }
}
