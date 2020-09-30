using System.ComponentModel.DataAnnotations;

namespace Pertuk.Dto.Requests.Auth
{
    public class FacebookAuthRequestModel
    {
        public string AccessToken { get; set; }
    }
}