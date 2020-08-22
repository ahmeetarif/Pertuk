using Pertuk.Dto.Models;
using System.Collections.Generic;

namespace Pertuk.Dto.Responses.Error
{
    public class ErrorResponseModel
    {
        public List<ErrorModel> Errors { get; set; } = new List<ErrorModel>();
    }
}
