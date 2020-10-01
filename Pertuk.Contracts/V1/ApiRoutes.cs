namespace Pertuk.Contracts.V1
{
    public static class ApiRoutes
    {
        public const string Version = "v1";

        public static class Auth
        {
            public const string Base = Version + "/auth";

            public const string Register = Base + "/register";

            public const string Login = Base + "/login";

            public const string ConfirmEmail = Base + "/confirmemail";

            public const string SendEmailConfirmation = Base + "/sendemailconfirmation";

            public const string SendResetPassword = Base + "/sendresetpassword";

            public const string ResetPassword = Base + "/resetpassword";

            public const string FacebookAuthentication = Base + "/facebookauthentication";

        }
    }
}
