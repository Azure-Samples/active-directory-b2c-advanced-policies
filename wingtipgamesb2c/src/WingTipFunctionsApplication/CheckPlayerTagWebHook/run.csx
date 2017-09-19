#r "Newtonsoft.Json"

using System;
using System.Net;
using System.Net.Http.Formatting;
using Newtonsoft.Json;

public static async Task<object> Run(HttpRequestMessage request, TraceWriter log)
{
    log.Info($"Webhook was triggered!");

    string requestContentAsString = await request.Content.ReadAsStringAsync();
    dynamic requestContentAsJObject = JsonConvert.DeserializeObject(requestContentAsString);

    if (requestContentAsJObject.playerTag == null)
    {
        return request.CreateResponse(HttpStatusCode.BadRequest);
    }

    var playerTag = ((string) requestContentAsJObject.playerTag).ToLower();

    if (playerTag == "mcvinny" || playerTag == "msgates123" || playerTag == "revcottonmarcus")
    {
        return request.CreateResponse<ResponseContent>(
            HttpStatusCode.Conflict,
            new ResponseContent
            {
                version = "1.0.0",
                status = (int) HttpStatusCode.Conflict,
                userMessage = $"The player tag '{requestContentAsJObject.playerTag}' is already used."
            },
            new JsonMediaTypeFormatter(),
            "application/json");
    }

    return request.CreateResponse(HttpStatusCode.OK);
}

public class ResponseContent
{
    public string version { get; set; }

    public int status { get; set; }

    public string userMessage { get; set; }
}
