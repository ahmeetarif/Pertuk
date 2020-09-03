namespace Pertuk.Common.Infrastructure
{
    public static class BaseErrorResponseMessages
    {
        public static class Mail
        {
            public static string MailSentFail = "An error occurred while sending an email!";
            public static string MailSentSuccess = "Mail successfully sent to your Email address, Please check your Email inbox!";
        }

        public static class Email
        {
            public static string EmailExist = "Email address already exist!";
            public static string EmailNotFound = "Email address not found!";
            public static string EmailAlreadyConfirmed = "Email address already confirmed!";
            public static string ConfirmationSuccess = "Email address confirmed successfully!";
            public static string ConfirmationFail = "An error occurred while confirming your Email address";
        }

        public static class Password
        {
            public static string Invalid = "Invalid Password!";
            public static string PasswordResetSuccess = "Password successfully changed!";
        }

        public static class User
        {
            public static string EnterUserDetail = "Please enter user detail!";
            public static string UserNotFound = "User not found!";
            public static string UserCreatedSuccess = "User created successfully!";
            public static string LogedInSuccess = "User successfully logged in!";
        }

        public static class Username
        {
            public static string UsernameExist = "Username already exist!";
            public static string UsernameNotFound = "Username not found!";
        }

        public static class Defaults
        {
            public static string EnterDetail = "Please enter detail!";
            public static string InvalidDetails = "Invalid details!";
            public static string InvalidDigitCode = "Invalid Digit Code!";
        }
    }
}