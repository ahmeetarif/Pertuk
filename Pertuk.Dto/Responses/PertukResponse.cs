using System.Net;
using System.Runtime.Serialization;

namespace Pertuk.Dto.Responses
{
    [DataContract]
    public class PertukResponse
    {
        public static PertukResponse Create(HttpStatusCode statusCode, object result = null, string errorMessage = null)
        {
            return new PertukResponse(statusCode, result, errorMessage);
        }

        [DataMember]
        public string Version { get { return "1.0"; } }

        [DataMember]
        public int StatusCode { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string ErrorMessage { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public object Result { get; set; }

        public PertukResponse(HttpStatusCode statusCode, object result = null, string errorMessage = null)
        {
            StatusCode = (int)statusCode;
            Result = result;
            ErrorMessage = errorMessage;
        }
    }
}