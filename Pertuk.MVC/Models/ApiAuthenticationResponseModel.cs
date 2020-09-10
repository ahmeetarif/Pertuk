using System.Collections.Generic;

namespace Pertuk.MVC.Models
{
    public class ApiAuthenticationResponseModel
    {
        public bool IsSuccess { get; set; }
        public string Token { get; set; }
        public string Message { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}
