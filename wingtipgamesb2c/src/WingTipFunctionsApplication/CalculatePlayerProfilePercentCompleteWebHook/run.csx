#r "Newtonsoft.Json"

using System;
using System.Net;
using Newtonsoft.Json;

public static async Task<object> Run(HttpRequestMessage request, TraceWriter log)
{
    log.Info($"Webhook was triggered!");

    string requestContentAsString = await request.Content.ReadAsStringAsync();
    dynamic requestContentAsJObject = JsonConvert.DeserializeObject(requestContentAsString);
    var playerProfilePercentComplete = 0;

    if (!string.IsNullOrEmpty((string) requestContentAsJObject.playerTag))
    {
        playerProfilePercentComplete += 25;
    }

    if (!string.IsNullOrEmpty((string) requestContentAsJObject.playerZone))
    {
        playerProfilePercentComplete += 25;
    }

    if (!string.IsNullOrEmpty((string) requestContentAsJObject.playerMotto))
    {
        playerProfilePercentComplete += 25;
    }

    if (!string.IsNullOrEmpty((string) requestContentAsJObject.playerBio))
    {
        playerProfilePercentComplete += 25;
    }

    return request.CreateResponse(
        HttpStatusCode.OK,
        new {
            playerProfilePercentComplete
        });
}
