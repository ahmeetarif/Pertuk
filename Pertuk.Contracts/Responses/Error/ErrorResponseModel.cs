using System.Collections.Generic;

namespace Pertuk.Contracts.Responses.Error
{
    public class ErrorResponseModel
    {
        public List<ErrorModel> Errors { get; set; } = new List<ErrorModel>();
    }
}