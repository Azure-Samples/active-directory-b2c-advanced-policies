namespace B2CAppMobile
{
    public class ExternalLoginInfo
    {
        public ExternalLoginInfo(
            string authority,
            string authenticationType,
            string caption,
            string[] scope)
        {
            Authority = authority;
            AuthenticationType = authenticationType;
            Caption = caption;
            Scope = scope;
        }

        public string AuthenticationType { get; }

        public string Authority { get; }

        public string Caption { get; }

        public string[] Scope { get; }
    }
}
