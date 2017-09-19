namespace WingTipBillingWebApplication
{
    public static class Constants
    {
        public static class AuthenticationSchemes
        {
            public const string Bearer = "Bearer";
        }

        public static class AuthorizationPolicies
        {
            public const string ReadBilling = "ReadBilling";
        }

        public static class PolicyIds
        {
            public const string Link = "b2c_1a_link";

            public const string PasswordReset = "b2c_1a_password_reset";

            public const string ProfileUpdate = "b2c_1a_profile_update_games";

            public const string SignUpOrSignIn = "b2c_1a_sign_up_sign_in_games";

            public const string StepUp = "b2c_1a_step_up";
        }
    }
}
