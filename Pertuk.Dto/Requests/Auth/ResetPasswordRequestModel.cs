namespace Pertuk.Dto.Requests.Auth
{
    public class ResetPasswordRequestModel
    {
        public string Token { get; set; }
        public string Email { get; set; }
        public string NewPassword { get; set; }
    }
}