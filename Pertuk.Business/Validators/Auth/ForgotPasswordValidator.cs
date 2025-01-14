﻿using FluentValidation;
using Pertuk.Business.Extensions.ValidationExt;
using Pertuk.Contracts.V1.Requests.Auth;

namespace Pertuk.Business.Validators.Auth
{
    public class ForgotPasswordValidator : AbstractValidator<ForgotPasswordRequestModel>
    {
        public ForgotPasswordValidator()
        {
            RuleFor(x => x.Email).Email();
        }
    }
}