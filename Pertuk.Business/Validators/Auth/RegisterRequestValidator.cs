using FluentValidation;
using Pertuk.Business.Extensions.ValidationExt;
using Pertuk.Dto.Requests.Auth;

namespace Pertuk.Business.Validators.Auth
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequestModel>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.Fullname).Fullname();

            RuleFor(x => x.Username).Username();

            RuleFor(x => x.Email).Email();

            RuleFor(x => x.Password).Password();
        }
    }
}
