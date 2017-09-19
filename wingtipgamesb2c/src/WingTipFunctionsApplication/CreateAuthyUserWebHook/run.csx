#r "Newtonsoft.Json"

using System;
using System.Configuration;
using System.Net;
using System.Net.Http.Formatting;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

public static async Task<object> Run(HttpRequestMessage request, TraceWriter log)
{
    log.Info($"Webhook was triggered!");

    string requestContentAsString = await request.Content.ReadAsStringAsync();
    dynamic requestContentAsJObject = JsonConvert.DeserializeObject(requestContentAsString);

    if (requestContentAsJObject.email == null || requestContentAsJObject.phoneNumber == null)
    {
        return request.CreateResponse(HttpStatusCode.BadRequest);
    }

    var phoneNumberMatch = Regex.Match((string)requestContentAsJObject.phoneNumber, @"\+(9[976]\d|8[987530]\d|6[987]\d|5[90]\d|42\d|3[875]\d|2[98654321]\d|9[8543210]|8[6421]|6[6543210]|5[87654321]|4[987654310]|3[9643210]|2[70]|7|1)(\d{1,14})$");

    if (!phoneNumberMatch.Success || phoneNumberMatch.Groups.Count != 3)
    {
        return request.CreateResponse(HttpStatusCode.BadRequest);
    }

    using (var client = new HttpClient())
    {
        var requestUri = $"https://api.authy.com/protected/json/users/new?api_key={ConfigurationManager.AppSettings["AuthyApiKey"]}";
        var requestForm = new Dictionary<string, string>();
        requestForm.Add("user[email]", (string)requestContentAsJObject.email);
        requestForm.Add("user[cellphone]", phoneNumberMatch.Groups[2].Value);
        requestForm.Add("user[country_code]", phoneNumberMatch.Groups[1].Value);
        var requestContent = new FormUrlEncodedContent(requestForm);
        var response = await client.PostAsync(requestUri, requestContent);
        response.EnsureSuccessStatusCode();
        dynamic responseContent = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());
        string authyId = "";

        if ((bool)responseContent.success)
        {
            authyId = (string)responseContent.user.id;
        }
        else
        {
            return request.CreateResponse<ResponseContent>(
                HttpStatusCode.Conflict,
                new ResponseContent
                {
                    version = "1.0.0",
                    status = (int)HttpStatusCode.Conflict,
                    userMessage = "An unexpected error occurred creating the Authy user."
                },
                new JsonMediaTypeFormatter(),
                "application/json");
        }

        return request.CreateResponse(HttpStatusCode.OK, new
        {
            authyId = authyId
        });
    }
}

public class ResponseContent
{
    public string version { get; set; }

    public int status { get; set; }

    public string userMessage { get; set; }
}
