namespace Pertuk.Contracts.V1.Requests.Auth
{
    public class RefreshTokenRequestModel
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}