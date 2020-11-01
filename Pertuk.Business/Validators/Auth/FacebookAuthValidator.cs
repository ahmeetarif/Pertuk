using FluentValidation;
using Pertuk.Contracts.V1.Requests.Auth;

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