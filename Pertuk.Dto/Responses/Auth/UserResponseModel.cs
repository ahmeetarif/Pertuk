using System.Collections.Generic;

namespace Pertuk.Dto.Responses.Auth
{
    public class UserResponseModel
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}
