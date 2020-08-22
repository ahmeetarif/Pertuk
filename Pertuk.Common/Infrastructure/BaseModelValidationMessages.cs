namespace Pertuk.Common.Infrastructure
{
    public static class BaseModelValidationMessages
    {
        public static class Email
        {
            public static string Valid = "Please enter valid Email address!";
            public static string Empty = "Please enter Email address!";
        }

        public static class Username
        {
            public static string Valid = "Please enter valid Username!";
            public static string Empty = "Please enter Username!";
            public static string Min = string.Format($"Minimum Username length : {BaseModelLength.MinUsername}");
            public static string Max = string.Format($"Maximum Username length : {BaseModelLength.MaxUsername}");
        }

        public static class Password
        {
            public static string Valid = "Please enter valid Password!";
            public static string Empty = "Please enter your Password!";
            public static string Min = string.Format($"Minimum Password length : {BaseModelLength.MinPassword}");
            public static string Max = string.Format($"Maximum Passowrd length : {BaseModelLength.MaxPassword}");
            public static string LowercaseLetter = "Password should contain at least one Lowercased letter!";
            public static string UppercaseLetter = "Passowrd should contain at least one Uppercased letter!";
            public static string Number = "Password should contain at least one Number!";
        }

        public static class Fullname
        {
            public static string Valid = "Please enter valid Name!";
            public static string Empty = "Please enter Name!";
            public static string Max = string.Format($"Maximum Firstname length : {BaseModelLength.MaxFullname}");
        }

        public static class Department
        {
            public static string Max = string.Format($"Maximum Department length : {BaseModelLength.MaxDepartment}");
        }
    }
}