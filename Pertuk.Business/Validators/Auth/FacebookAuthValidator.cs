using FluentValidation;
using Pertuk.Contracts.Requests.Auth;

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