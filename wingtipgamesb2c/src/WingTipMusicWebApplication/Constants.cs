namespace WingTipMusicWebApplication
{
    public static class Constants
    {
        public static class AuthenticationPropertiesKeys
        {
            public const string DomainHint = "domain_hint";

            public const string UILocales = "ui_locales";
        }

        public static class AuthenticationSchemes
        {
            public const string ApplicationCookie = "ApplicationCookie";

            public const string Bearer = "Bearer";
        }

        public static class AuthorizationPolicies
        {
            public const string ReadMusic = "Music.Read";
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

        public static class ListenerGenres
        {
            public const string Classical = "Classical";

            public const string Country = "Country";

            public const string Electronic = "Electronic / Dance";

            public const string HipHop = "Hip Hop";

            public const string Jazz = "Jazz";

            public const string Pop = "Pop";

            public const string RNB = "R & B / Soul";

            public const string Rock = "Rock";

            public const string Soundtracks = "Soundtracks";
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

        public static class PolicyIds
        {
            public const string PasswordReset = "b2c_1a_password_reset";

            public const string ProfileUpdate = "b2c_1a_profile_update_music";

            public const string SignUpOrSignIn = "b2c_1a_sign_up_sign_in_music";
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
