namespace WingTipIdentityWebApplication.Api.Models
{
    public class AccountResetPasswordRequest
    {
        public string UserName { get; set; }

        public string NewPassword { get; set; }
    }
}
