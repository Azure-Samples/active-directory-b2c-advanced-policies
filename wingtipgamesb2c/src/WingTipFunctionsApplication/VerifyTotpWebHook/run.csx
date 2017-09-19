#r "Newtonsoft.Json"

using System;
using System.Configuration;
using System.Net;
using System.Net.Http.Formatting;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using OtpSharp;

public static async Task<object> Run(HttpRequestMessage request, TraceWriter log)
{
    log.Info($"Webhook was triggered!");

    string requestContentAsString = await request.Content.ReadAsStringAsync();
    dynamic requestContentAsJObject = JsonConvert.DeserializeObject(requestContentAsString);

    if (requestContentAsJObject.secretKey == null || requestContentAsJObject.totpCode == null)
    {
        return request.CreateResponse(HttpStatusCode.BadRequest);
    }

    log.Info((string)requestContentAsJObject.timeStepMatched);

    using (var deriveBytes = new Rfc2898DeriveBytes(Convert.FromBase64String(ConfigurationManager.AppSettings["CryptoKeyPassword"]), Convert.FromBase64String(ConfigurationManager.AppSettings["CryptoKeySalt"]), int.Parse(ConfigurationManager.AppSettings["CryptoKeyIterations"])))
    {
        using (var symmetricAlgorithm = TripleDES.Create())
        {
            symmetricAlgorithm.Key = deriveBytes.GetBytes(16);
            symmetricAlgorithm.IV = Convert.FromBase64String(ConfigurationManager.AppSettings["CryptoIV"]);
            byte[] secretKey;

            using (var decryptionStream = new MemoryStream())
            {
                using (var decryptorStream = new CryptoStream(decryptionStream, symmetricAlgorithm.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    var encryptedSecretKey = Convert.FromBase64String((string)requestContentAsJObject.secretKey);
                    decryptorStream.Write(encryptedSecretKey, 0, encryptedSecretKey.Length);
                    decryptorStream.Flush();
                    decryptorStream.Close();
                }

                secretKey = decryptionStream.ToArray();
                deriveBytes.Reset();
            }

            var totp = new Totp(secretKey);
            long timeStepMatched;
            var verificationResult = totp.VerifyTotp((string)requestContentAsJObject.totpCode, out timeStepMatched, VerificationWindow.RfcSpecifiedNetworkDelay);

            if (!verificationResult)
            {
                return request.CreateResponse<ResponseContent>(
                    HttpStatusCode.Conflict,
                    new ResponseContent
                    {
                        version = "1.0.0",
                        status = (int)HttpStatusCode.Conflict,
                        userMessage = "The verification code is invalid."
                    },
                    new JsonMediaTypeFormatter(),
                    "application/json");
            }

            if (requestContentAsJObject.timeStepMatched != null && ((string)requestContentAsJObject.timeStepMatched).Equals(timeStepMatched.ToString()))
            {
                return request.CreateResponse<ResponseContent>(
                    HttpStatusCode.Conflict,
                    new ResponseContent
                    {
                        version = "1.0.0",
                        status = (int)HttpStatusCode.Conflict,
                        userMessage = "The verification code has already been used."
                    },
                    new JsonMediaTypeFormatter(),
                    "application/json");
            }

            return request.CreateResponse(HttpStatusCode.OK, new
            {
                timeStepMatched = timeStepMatched.ToString()
            });
        }
    }
}

public class ResponseContent
{
    public string version { get; set; }

    public int status { get; set; }

    public string userMessage { get; set; }
}
