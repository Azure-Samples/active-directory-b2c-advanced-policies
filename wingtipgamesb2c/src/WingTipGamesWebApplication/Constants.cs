namespace WingTipGamesWebApplication
{
    public static class Constants
    {
        public static class AuthenticationPropertiesKeys
        {
            public const string DomainHint = "domain_hint";

            public const string Nonce = "nonce";

            public const string PlayerProfileRegistrationMode = "player_profile_registration_mode";

            public const string PolicyTokenLifetime = "policy_token_lifetime";

            public const string SkipCorrelation = "skip_correlation";

            public const string UILocales = "ui_locales";

            public const string VerifiedEmail = "verified_email";
        }

        public static class AuthenticationSchemes
        {
            public const string ApplicationCookie = "ApplicationCookie";

            public const string Bearer = "Bearer";
        }

        public static class AuthorizationPolicies
        {
            public const string ReadGames = "ReadGames";
        }

        public static class CookieNames
        {
            public const string CurrentLocale = ".WingTipToys.CurrentLocale";

            public const string CurrentLocation = ".WingTipToys.CurrentLocation";
        }

        public static class Cultures
        {
            public static class Keys
            {
                public const string Auto = "";

                public const string English = "en";

                public const string French = "fr";
            }

            public static class Values
            {
                public const string Auto = "(Auto)";

                public const string English = "English";

                public const string French = "French";
            }
        }

        public static class Domains
        {
            public const string Amazon = "amazon.com";

            public const string Facebook = "facebook.com";
        }

        public static class Locations
        {
            public static class Keys
            {
                public const string Auto = "(Auto)";

                public const string Canada = "CA";

                public const string Manual = "(Manual)";

                public const string UnitedStates = "US";
            }

            public static class Values
            {
                public const string Auto = "(Auto)";

                public const string Canada = "Canada";

                public const string Manual = "(Manual)";

                public const string UnitedStates = "United States";
            }
        }

        public static class PlayerZones
        {
            public const string Family = "Family";

            public const string Professional = "Professional";

            public const string Recreation = "Recreation";

            public const string Underground = "Underground";
        }

        public static class PolicyIds
        {
            public const string Activation = "b2c_1a_activation";

            public const string Invitation = "b2c_1a_invitation";

            public const string InvitationLink = "b2c_1a_invitation_link";

            public const string InvitationRedirect = "b2c_1a_invitation_redirect";

            public const string Link = "b2c_1a_link";

            public const string PasswordReset = "b2c_1a_password_reset";

            public const string ProfileUpdate = "b2c_1a_profile_update_games";

            public const string SignInUsingAppCode = "b2c_1a_sign_in_games_app_code";

            public const string SignInUsingAuthyCode = "b2c_1a_sign_in_games_authy_code";

            public const string SignInUsingEmailCode = "b2c_1a_sign_in_games_email_code";

            public const string SignInUsingPhoneCode = "b2c_1a_sign_in_games_phone_code";

            public const string SignUpOrSignIn = "b2c_1a_sign_up_sign_in_games";

            public const string SignUpUsingAppCode = "b2c_1a_sign_up_games_app_code";

            public const string SignUpUsingAuthyCode = "b2c_1a_sign_up_games_authy_code";

            public const string StepUp = "b2c_1a_step_up";
        }

        public static class TempDataKeys
        {
            public const string IsNewUser = "IsNewUser";
        }

        public static class ViewDataKeys
        {
            public const string CurrentCulture = "CurrentCulture";

            public const string CurrentLocation = "CurrentLocation";
        }
    }
}
