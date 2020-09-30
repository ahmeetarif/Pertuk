using FluentValidation;
using Pertuk.Common.Infrastructure;
using System.Text.RegularExpressions;

namespace Pertuk.Business.Extensions.ValidationExt
{
    public static class RuleBuilderExtensions
    {
        public static IRuleBuilder<T, string> Password<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            var options = ruleBuilder
                .NotEmpty().WithMessage("Please enter your Password!")
                .MinimumLength(6).WithMessage("Your Password must be 6 characters!")
                .Matches("[A-Z]").WithMessage("Your Password must contain at least one Uppercased letter!")
                .Matches("[a-z]").WithMessage("Your Password must contain at least one Lowercased letter!")
                .Matches("[0-9]").WithMessage("Your Password must contain at least one Number!");
            return options;
        }

        public static IRuleBuilder<T, string> Email<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            var options = ruleBuilder
                .NotEmpty().WithMessage("Please enter your Email Address!")
                .Must(IsEmailValid).WithMessage("Please enter valid Email Address!")
                .MaximumLength(256).WithMessage("Email Address should not be more than 256 characters!");
            return options;
        }

        public static IRuleBuilder<T, string> Username<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            var options = ruleBuilder
                .MaximumLength(256).WithMessage("Username should not be more than 256 characters!");
            return options;
        }

        public static IRuleBuilder<T, string> Fullname<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            var options = ruleBuilder
                .MaximumLength(100).WithMessage("Your Name must be string with length of 100!")
                .Must(IsValidName).WithMessage("Please enter valid Name!");
            return options;
        }

        public static IRuleBuilder<T, string> Department<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            var options = ruleBuilder
                .MaximumLength(100).WithMessage("Department should not be more than 100 characters!");
            return options;
        }

        public static IRuleBuilder<T, string> UserId<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            var options = ruleBuilder.NotEmpty().WithMessage("Please enter UserId");

            return options;
        }

        public static IRuleBuilder<T, string> UniversityName<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            var options = ruleBuilder
                .MaximumLength(256).WithMessage("University Name should not be more than 256 characters!");

            return options;
        }

        public static IRuleBuilder<T, string> AcademicOf<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            var options = ruleBuilder
                .MaximumLength(150).WithMessage("Academic Name should not be more than 150 characters!");

            return options;
        }

        public static IRuleBuilder<T, string> DepartmetOf<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            var options = ruleBuilder
                .MaximumLength(150).WithMessage("Department Name should not be more than 150 characters!");

            return options;
        }

        public static IRuleBuilder<T, string> Subject<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            var options = ruleBuilder
                .MaximumLength(100).WithMessage("Subject Name should not be more than 100 characters!");

            return options;
        }

        public static IRuleBuilder<T, string> Title<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            var options = ruleBuilder
                .MaximumLength(100).WithMessage("Title should not be more than 100 characters!");

            return options;
        }

        public static IRuleBuilder<T, string> Description<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            var options = ruleBuilder
                .MaximumLength(1000).WithMessage("Description should not be more than 1000 characters!");

            return options;
        }

        public static IRuleBuilder<T, string> DigitCode<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            var options = ruleBuilder
                .NotEmpty().WithMessage("Please provide a digit code!")
                .MaximumLength(6).WithMessage("The digit code must be Number with maximum 6 of length!");

            return options;
        }

        #region Private Functions

        private static bool IsEmailValid(string email)
        {
            var allowedEmailCharacters = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");

            if (allowedEmailCharacters.IsMatch(email))
            {
                return true;
            }

            return false;
        }

        private static bool IsValidName(string name)
        {
            var allowedNameCharacters = new Regex(@"^[\p{L} \.'\-]+$");

            if (name == null || string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name))
            {
                return true;
            }

            if (allowedNameCharacters.IsMatch(name))
            {
                return true;
            }

            return false;
        }

        #endregion
    }
}
