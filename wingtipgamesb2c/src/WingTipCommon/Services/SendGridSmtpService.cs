using System.Net.Mail;

namespace WingTipCommon.Services
{
    public class SendGridSmtpService : ISmtpService
    {
        private readonly bool _enableSsl;
        private readonly string _host;
        private readonly string _pass;
        private readonly int _port;
        private readonly string _user;

        public SendGridSmtpService(
            bool enableSsl,
            string host,
            int port,
            string user,
            string pass)
        {
            _enableSsl = enableSsl;
            _host = host;
            _port = port;
            _user = user;
            _pass = pass;
        }

        public void SendActivationEmail(string toAddress, string displayName, string redeemUrl)
        {
            SendEmail(
                toAddress,
                "Activate your WingTip account",
                $"<p>Hi {displayName}</p><p>Click the following link to activate your WingTip account.</p><p><a href=\"{redeemUrl}\">{redeemUrl}</a></p><p>We hope you enjoy our online services.</p><p>Regards,</p><p>The WingTip Toys Team</p><p><img src=\"https://wingtipgamesb2c.azurewebsites.net/images/logo.png\"></p>");
        }

        public void SendInvitationEmail(string toAddress, string redeemUrl)
        {
            SendEmail(
                toAddress,
                "Claim your free game rental",
                $"<p>Hi</p><p>Click the following link to claim your free game rental.</p><p><a href=\"{redeemUrl}\">{redeemUrl}</a></p><p>We hope you enjoy your free game rental.</p><p>Regards,</p><p>The WingTip Toys Team</p><p><img src=\"https://wingtipgamesb2c.azurewebsites.net/images/logo.png\"></p>");
        }

        private void SendEmail(string toAddress, string subject, string body)
        {
            using (var client = new SmtpClient(_host, _port))
            {
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential(_user, _pass);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.EnableSsl = _enableSsl;

                var message = new MailMessage("no-reply@wingtiptoys.net", toAddress)
                {
                    Subject = subject,
                    IsBodyHtml = true,
                    Body = body
                };

                client.Send(message);
            }
        }
    }
}
