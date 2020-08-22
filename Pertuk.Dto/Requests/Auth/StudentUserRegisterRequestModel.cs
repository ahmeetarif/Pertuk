using Pertuk.Core.Dtos;

namespace Pertuk.Dto.Requests.Auth
{
    public class StudentUserRegisterRequestModel : BaseRegisterDto
    {
        public int Grade { get; set; }
    }
}