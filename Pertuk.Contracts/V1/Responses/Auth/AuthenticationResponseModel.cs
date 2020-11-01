namespace Pertuk.Contracts.V1.Responses.Auth
{
    public class AuthenticationResponseModel
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string Message { get; set; }
    }
}