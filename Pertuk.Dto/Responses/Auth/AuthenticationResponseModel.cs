using System.Collections.Generic;

namespace Pertuk.Dto.Responses.Auth
{
    public class AuthenticationResponseModel
    {
        public bool IsSuccess { get; set; }
        public string Token { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}