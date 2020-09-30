using FluentValidation;
using Pertuk.Business.Extensions.ValidationExt;
using Pertuk.Dto.Requests.Auth;

namespace Pertuk.Business.Validators.Auth
{
    public class ConfirmEmailValidator : AbstractValidator<ConfirmEmailRequestModel>
    {
        public ConfirmEmailValidator()
        {
            RuleFor(x => x.UserId).UserId();
        }
    }
}