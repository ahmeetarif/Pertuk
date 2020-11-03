using System;
namespace Pertuk.Contracts.V1.Requests.Auth
{
    public class VerifyResetPasswordRequestModel
    {
        public string Email { get; set; }
        public string RecoveryCode { get; set; }
    }
}
