namespace Pertuk.Dto.Requests.Auth
{
    public class ResetPasswordRequestModel
    {
        public string DigitCode { get; set; }
        public string Email { get; set; }
        public string NewPassword { get; set; }
    }
}