using Microsoft.AspNetCore.Http;

namespace Pertuk.Contracts.Requests.Auth
{
    public class RegisterRequestModel
    {
        public string Fullname { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public IFormFile ProfileImage { get; set; }
    }
}