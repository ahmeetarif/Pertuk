using FluentValidation;
using Pertuk.Dto.Requests.Auth;

namespace Pertuk.Business.Validators.Auth
{
    public class FacebookAuthValidator : AbstractValidator<FacebookAuthRequestModel>
    {
        public FacebookAuthValidator()
        {
            RuleFor(x => x.AccessToken).NotEmpty().WithMessage("Please provide an AccessToken!");
        }
    }
}
