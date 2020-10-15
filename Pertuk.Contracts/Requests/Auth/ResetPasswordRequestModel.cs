namespace Pertuk.Contracts.Requests.Auth
{
    public class ResetPasswordRequestModel
    {
        public string DigitCode { get; set; }
        public string Email { get; set; }
        public string NewPassword { get; set; }
    }
}