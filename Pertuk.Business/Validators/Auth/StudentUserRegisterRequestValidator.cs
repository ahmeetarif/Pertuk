﻿using FluentValidation;
using Pertuk.Business.Extensions.ValidationExt;
using Pertuk.Dto.Requests.Auth;

namespace Pertuk.Business.Validators.Auth
{
    public class StudentUserRegisterRequestValidator : AbstractValidator<StudentUserRegisterRequestModel>
    {
        public StudentUserRegisterRequestValidator()
        {
            RuleFor(x => x.Email).Email();

            RuleFor(x => x.Fullname).Fullname();

            RuleFor(x => x.Password).Password();

            RuleFor(x => x.Username).Username();

            RuleFor(x => x.Grade).Grade();

            RuleFor(x => x.Department).Department();
        }
    }
}
