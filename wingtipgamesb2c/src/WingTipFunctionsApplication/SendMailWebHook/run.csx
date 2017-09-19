#r "Newtonsoft.Json"

using System;
using System.Net;
using System.Net.Mail;
using Newtonsoft.Json;

public static async Task<object> Run(HttpRequestMessage request, TraceWriter log)
{
    log.Verbose($"Webhook was triggered!");

    var requestContentAsString = await request.Content.ReadAsStringAsync();
    dynamic requestContentAsJObject = JsonConvert.DeserializeObject(requestContentAsString);
    string fromAddress = requestContentAsJObject.fromAddress;
    string toAddress = requestContentAsJObject.toAddress;
    var smtpHost = "smtp.sendgrid.net";
    var smtpPort = 587;
    var smtpEnableSsl = true;
    var smtpUser = GetEnvironmentVariable("SmtpUserName");
    var smtpPass = GetEnvironmentVariable("SmtpPassword");
    var brand = requestContentAsJObject.brand;    
    var displayName = requestContentAsJObject.displayName;
        
    using (var client = new SmtpClient(smtpHost, smtpPort))
    {
        client.UseDefaultCredentials = false;
        client.Credentials = new System.Net.NetworkCredential(smtpUser, smtpPass);
        client.DeliveryMethod = SmtpDeliveryMethod.Network;
        client.EnableSsl = smtpEnableSsl;
        MailMessage message = new MailMessage(fromAddress, toAddress);
        message.Subject = "Welcome to WingTip Toys";
        message.IsBodyHtml = true;
        string imageSource;

        if (brand == "WingTip Games")
        {
            imageSource = "https://wingtipgamesb2c.azurewebsites.net/images/logo.png";
        }
        else if (brand == "WingTip Music")
        {
            imageSource = "https://wingtipmusicb2c.azurewebsites.net/images/logo.png";
        }
        else
        {
            imageSource = "https://wingtipb2ctmpls.blob.core.windows.net/wingtiptoys/images/logo.png";
        } 

        message.Body = $"<p>Hi {displayName}</p><p>Thank you for joining WingTip Toys by registering yourself through {brand}! We hope you enjoy our online services.</p><p>Regards,</p><p>The WingTip Toys Team</p><p><img src=\"{imageSource}\"></p>";

        try
        {
            client.Send(message);
            log.Verbose("Success.");

            return request.CreateResponse(
                HttpStatusCode.OK,
                new
                {
                    status = true,
                    message = string.Empty
                });
        }
        catch (Exception ex)
        {
            log.Verbose("Failure: " + ex.ToString());
            
            return request.CreateResponse(
                HttpStatusCode.InternalServerError,
                new
                {
                    status = false,
                    message = "Check Azure Function Logs for more information."
                });
        }
    }
}

public static string GetEnvironmentVariable(string name)
{
    return Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
}
