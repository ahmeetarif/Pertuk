﻿namespace Pertuk.Contracts.V1.Requests.Auth
{
    public class ResetPasswordRequestModel
    {
        public string Email { get; set; }
        public string NewPassword { get; set; }
    }
}