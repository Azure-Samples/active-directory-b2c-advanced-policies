namespace WingTipCommon.Services
{
    public interface ISmtpService
    {
        void SendActivationEmail(string toAddress, string displayName, string redeemUrl);

        void SendInvitationEmail(string toAddress, string redeemUrl);
    }
}
