#r "Newtonsoft.Json"

using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using Newtonsoft.Json;

public static async Task<object> Run(HttpRequestMessage request, TraceWriter log)
{
    log.Info($"Webhook was triggered!");

    string requestContentAsString = await request.Content.ReadAsStringAsync();
    dynamic requestContentAsJObject = JsonConvert.DeserializeObject(requestContentAsString);

    if (requestContentAsJObject.authyId == null || requestContentAsJObject.email == null)
    {
        return request.CreateResponse(HttpStatusCode.BadRequest);
    }

    using (var client = new HttpClient())
    {
        var postRequestUri = $"https://api.authy.com/onetouch/json/users/{(string)requestContentAsJObject.authyId}/approval_requests?api_key={ConfigurationManager.AppSettings["AuthyApiKey"]}";
        var postRequestForm = new Dictionary<string, string>();
        postRequestForm.Add("message", "Sign in with your WingTip account");
        postRequestForm.Add("details[username]", (string)requestContentAsJObject.email);
        postRequestForm.Add("logos[][res]", "default");
        postRequestForm.Add("logos[][url]", "https://wingtipb2ctmpls.blob.core.windows.net/wingtiptoys/images/logo.png");
        postRequestForm.Add("seconds_to_expire", "30");
        var postRequestContent = new FormUrlEncodedContent(postRequestForm);
        var postResponse = await client.PostAsync(postRequestUri, postRequestContent);
        postResponse.EnsureSuccessStatusCode();
        dynamic postResponseContent = JsonConvert.DeserializeObject(await postResponse.Content.ReadAsStringAsync());
        string approvalRequestId = "";

        if ((bool)postResponseContent.success)
        {
            approvalRequestId = (string)postResponseContent.approval_request.uuid;
        }
        else
        {
            return request.CreateResponse<ResponseContent>(
                HttpStatusCode.Conflict,
                new ResponseContent
                {
                    version = "1.0.0",
                    status = (int)HttpStatusCode.Conflict,
                    userMessage = "An unexpected error occurred creating the Authy approval request."
                },
                new JsonMediaTypeFormatter(),
                "application/json");
        }

        var isApprovalRequestApproved = false;
        var approvalRequestQueryCount = 0;

        while (!isApprovalRequestApproved && approvalRequestQueryCount < 10)
        {
            var getRequestUri = $"https://api.authy.com/onetouch/json/approval_requests/{approvalRequestId}?api_key={ConfigurationManager.AppSettings["AuthyApiKey"]}";
            var getResponse = await client.GetAsync(getRequestUri);
            getResponse.EnsureSuccessStatusCode();
            dynamic getResponseContent = JsonConvert.DeserializeObject(await getResponse.Content.ReadAsStringAsync());

            if ((bool)getResponseContent.success)
            {
                if (((string)getResponseContent.approval_request.status).Equals("approved"))
                {
                    isApprovalRequestApproved = true;
                }
                else if (((string)getResponseContent.approval_request.status).Equals("denied"))
                {
                    return request.CreateResponse<ResponseContent>(
                        HttpStatusCode.Conflict,
                        new ResponseContent
                        {
                            version = "1.0.0",
                            status = (int)HttpStatusCode.Conflict,
                            userMessage = "The Authy approval request was denied."
                        },
                        new JsonMediaTypeFormatter(),
                        "application/json");
                }
            }
            else
            {
                return request.CreateResponse<ResponseContent>(
                    HttpStatusCode.Conflict,
                    new ResponseContent
                    {
                        version = "1.0.0",
                        status = (int)HttpStatusCode.Conflict,
                        userMessage = "An unexpected error occurred polling the Authy approval request."
                    },
                    new JsonMediaTypeFormatter(),
                    "application/json");
            }

            Thread.Sleep(3000);
            approvalRequestQueryCount++;
        }

        if (!isApprovalRequestApproved)
        {
            return request.CreateResponse<ResponseContent>(
                HttpStatusCode.Conflict,
                new ResponseContent
                {
                    version = "1.0.0",
                    status = (int)HttpStatusCode.Conflict,
                    userMessage = "The Authy approval request has timed out."
                },
                new JsonMediaTypeFormatter(),
                "application/json");
        }

        return request.CreateResponse(HttpStatusCode.OK);
    }
}

public class ResponseContent
{
    public string version { get; set; }

    public int status { get; set; }

    public string userMessage { get; set; }
}
