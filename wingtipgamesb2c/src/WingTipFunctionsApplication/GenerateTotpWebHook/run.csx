#r "Newtonsoft.Json"

using System;
using System.Configuration;
using System.Net;
using System.Net.Http.Formatting;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using OtpSharp;
using QRCoder;

public static async Task<object> Run(HttpRequestMessage request, TraceWriter log)
{
    log.Info($"Webhook was triggered!");

    string requestContentAsString = await request.Content.ReadAsStringAsync();
    dynamic requestContentAsJObject = JsonConvert.DeserializeObject(requestContentAsString);

    if (requestContentAsJObject.userName == null)
    {
        return request.CreateResponse(HttpStatusCode.BadRequest);
    }

    var secretKey = Guid.NewGuid().ToString();
    var totpUrl = $"{KeyUrl.GetTotpUrl(Encoding.UTF8.GetBytes(secretKey), $"WingTip:{(string)requestContentAsJObject.userName}")}&issuer=WingTip";
    var qrCodeGenerator = new QRCodeGenerator();
    var qrCodeData = qrCodeGenerator.CreateQrCode(totpUrl, QRCodeGenerator.ECCLevel.L);
    var qrCode = new BitmapByteQRCode(qrCodeData);
    var qrCodeBitmap = qrCode.GetGraphic(4);

    using (var deriveBytes = new Rfc2898DeriveBytes(Convert.FromBase64String(ConfigurationManager.AppSettings["CryptoKeyPassword"]), Convert.FromBase64String(ConfigurationManager.AppSettings["CryptoKeySalt"]), int.Parse(ConfigurationManager.AppSettings["CryptoKeyIterations"])))
    {
        using (var symmetricAlgorithm = TripleDES.Create())
        {
            symmetricAlgorithm.Key = deriveBytes.GetBytes(16);
            symmetricAlgorithm.IV = Convert.FromBase64String(ConfigurationManager.AppSettings["CryptoIV"]);
            byte[] encryptedSecretKey;

            using (var encryptionStream = new MemoryStream())
            {
                using (var encryptorStream = new CryptoStream(encryptionStream, symmetricAlgorithm.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    var encodedSecretKey = new UTF8Encoding(false).GetBytes(secretKey);
                    encryptorStream.Write(encodedSecretKey, 0, encodedSecretKey.Length);
                    encryptorStream.FlushFinalBlock();
                    encryptorStream.Close();
                }

                encryptedSecretKey = encryptionStream.ToArray();
                deriveBytes.Reset();
            }

            return request.CreateResponse(HttpStatusCode.OK, new
            {
                qrCodeBitmap = Convert.ToBase64String(qrCodeBitmap),
                secretKey = Convert.ToBase64String(encryptedSecretKey)
            });
        }
    }
}
