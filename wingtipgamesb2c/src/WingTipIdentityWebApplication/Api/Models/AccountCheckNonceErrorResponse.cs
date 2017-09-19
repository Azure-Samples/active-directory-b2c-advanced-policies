namespace WingTipIdentityWebApplication.Api.Models
{
    public class AccountCheckNonceErrorResponse
    {
        public string version { get; set; }

        public int status { get; set; }

        public string userMessage { get; set; }
    }
}
