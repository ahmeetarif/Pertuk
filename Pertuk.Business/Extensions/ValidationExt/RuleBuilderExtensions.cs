using FluentValidation;
using Microsoft.AspNetCore.Routing.Tree;
using Pertuk.Common.Infrastructure;
using System.Resources;
using System.Text.RegularExpressions;

namespace Pertuk.Business.Extensions.ValidationExt
{
    public static class RuleBuilderExtensions
    {
        public static IRuleBuilder<T, string> Password<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            var options = ruleBuilder
                .NotEmpty().WithMessage(BaseModelValidationMessages.Password.Empty)
                .MinimumLength(BaseModelLength.MinPassword).WithMessage(BaseModelValidationMessages.Password.Min)
                .MaximumLength(BaseModelLength.MaxPassword).WithMessage(BaseModelValidationMessages.Password.Max)
                .Matches("[A-Z]").WithMessage(BaseModelValidationMessages.Password.UppercaseLetter)
                .Matches("[a-z]").WithMessage(BaseModelValidationMessages.Password.LowercaseLetter)
                .Matches("[0-9]").WithMessage(BaseModelValidationMessages.Password.Number);
            return options;
        }

        public static IRuleBuilder<T, string> Email<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            var options = ruleBuilder
                .NotEmpty().WithMessage(BaseModelValidationMessages.Email.Empty)
                .Must(IsEmailValid).WithMessage(BaseModelValidationMessages.Email.Valid);
            return options;
        }

        public static IRuleBuilder<T, string> Username<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            var options = ruleBuilder
                .Must(IsUsernameValid).WithMessage(BaseModelValidationMessages.Username.Valid)
                .MaximumLength(BaseModelLength.MaxUsername).WithMessage(BaseModelValidationMessages.Username.Max);
            return options;
        }

        public static IRuleBuilder<T, string> Fullname<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            var options = ruleBuilder
                .NotEmpty().WithMessage(BaseModelValidationMessages.Fullname.Empty)
                .MaximumLength(BaseModelLength.MaxFullname).WithMessage(BaseModelValidationMessages.Fullname.Max)
                .Must(IsValidName).WithMessage(BaseModelValidationMessages.Fullname.Valid);
            return options;
        }

        public static IRuleBuilder<T, string> Department<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            var options = ruleBuilder
                .MaximumLength(BaseModelLength.MaxDepartment).WithMessage(BaseModelValidationMessages.Department.Max);
            return options;
        }

        public static IRuleBuilder<T, string> Grade<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            var options = ruleBuilder;
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

        private static bool IsUsernameValid(string username)
        {
            return true;
            if (string.IsNullOrEmpty(username))
            {
                return true;
            }

            var allowedUsernameCharacters = new Regex("^(?=.{6,50}$)(?![_.])(?!.*[_.]{2})[a-zA-Z0-9._]+(?<![_.])$");

            if (allowedUsernameCharacters.IsMatch(username))
            {
                return true;
            }
            return false;
        }

        private static bool IsValidName(string name)
        {
            var allowedNameCharacters = new Regex(@"^[\p{L} \.'\-]+$");

            if (allowedNameCharacters.IsMatch(name))
            {
                return true;
            }

            return false;
        }

        #endregion
    }
}
