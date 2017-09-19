namespace WingTipIdentityWebApplication.Api.Models
{
    public class AccountCheckNonceRequest
    {
        public string UserName { get; set; }

        public string Nonce { get; set; }
    }
}
