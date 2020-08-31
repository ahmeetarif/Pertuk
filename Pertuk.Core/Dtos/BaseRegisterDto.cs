using Microsoft.AspNetCore.Http;

namespace Pertuk.Core.Dtos
{
    public class BaseRegisterDto
    {
        public string Fullname { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Department { get; set; }
        public IFormFile ProfileImage { get; set; }
    }
}