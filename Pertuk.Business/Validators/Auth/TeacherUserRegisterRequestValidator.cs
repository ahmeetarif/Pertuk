using FluentValidation;
using Pertuk.Business.Extensions.ValidationExt;
using Pertuk.Dto.Requests.Auth;

namespace Pertuk.Business.Validators.Auth
{
    public class TeacherUserRegisterRequestValidator : AbstractValidator<TeacherUserRegisterRequestModel>
    {
        public TeacherUserRegisterRequestValidator()
        {
            RuleFor(x => x.Fullname).Fullname();

            RuleFor(x => x.Email).Email();

            RuleFor(x => x.Username).Username();

            RuleFor(x => x.Password).Password();

            RuleFor(x => x.Department).Department();
        }
    }
}
