using FluentValidation;
using Pertuk.Business.Extensions.ValidationExt;
using Pertuk.Contracts.V1.Requests.Auth;

namespace Pertuk.Business.Validators.Auth
{
    public class LoginRequestValidator : AbstractValidator<LoginRequestModel>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.Email).Email();

            RuleFor(x => x.Password).Password();
        }
    }
}