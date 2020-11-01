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

            public const string ConfirmEmail = Base + "/confirmEmail";

            public const string SendEmailConfirmation = Base + "/sendEmailConfirmation";

            public const string SendResetPassword = Base + "/sendResetPassword";

            public const string ResetPassword = Base + "/resetPassword";

            public const string FacebookAuthentication = Base + "/facebookAuthentication";

            public const string RefreshToken = Base + "/refreshToken";

        }

        public static class UserManager
        {
            public const string Base = Version + "/usermanager";
            public const string SetStudentUser = Base + "/setStudentUser";
            public const string SetTeacherUser = Base + "/setTeacherUser";
        }
    }
}
