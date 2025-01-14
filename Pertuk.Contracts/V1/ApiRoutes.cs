﻿namespace Pertuk.Contracts.V1
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

            public const string VerifyResetPasswordRecoveryCode = Base + "/verifyResetPasswordRecoveryCode";
        }

        public static class UserManager
        {
            public const string Base = Version + "/users";

            public const string SetStudentUser = Base + "/setStudent";

            public const string SetTeacherUser = Base + "/setTeacher";

            public const string GetUsers = Base + "/get";

            public const string GetCurrentDetails = Base + "/getCurrentUserDetails";
        }
    }
}
