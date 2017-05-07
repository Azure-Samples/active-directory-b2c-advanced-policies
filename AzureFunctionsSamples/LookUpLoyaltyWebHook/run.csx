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

    if (requestContentAsJObject.email == null)
    {
        return request.CreateResponse(HttpStatusCode.BadRequest);
    }

    var email = ((string) requestContentAsJObject.email).ToLower();


     return request.CreateResponse<ResponseContent>(
            HttpStatusCode.OK,
            new ResponseContent
            {
                version = "1.0.0",
                status = (int) HttpStatusCode.OK,
                userMessage = "User Found",
                city = "Redmond"
            },
            new JsonMediaTypeFormatter(),
            "application/json");
}

public class ResponseContent
{
    public string version { get; set; }

    public int status { get; set; }

    public string userMessage { get; set; }

    public string city {get; set; }
}
