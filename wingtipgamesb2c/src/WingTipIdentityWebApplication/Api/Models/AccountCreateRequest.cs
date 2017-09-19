namespace WingTipIdentityWebApplication.Api.Models
{
    public class AccountCreateRequest
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public string DisplayName { get; set; }

        public string PlayerTag { get; set; }
    }
}
