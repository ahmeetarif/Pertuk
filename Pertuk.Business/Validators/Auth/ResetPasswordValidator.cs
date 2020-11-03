using FluentValidation;
using Pertuk.Business.Extensions.ValidationExt;
using Pertuk.Contracts.V1.Requests.Auth;

namespace Pertuk.Business.Validators.Auth
{
    public class ResetPasswordValidator : AbstractValidator<ResetPasswordRequestModel>
    {
        public ResetPasswordValidator()
        {
            RuleFor(x => x.Email).Email();
            RuleFor(x => x.NewPassword).Password();
        }
    }
}
