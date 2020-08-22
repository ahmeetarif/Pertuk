namespace Pertuk.Common.Exceptions
{
    public class PertukExceptionServiceErrorResponse
    {
        private string _message = "Something went wrong! Please try again later.";
        public int StatusCode { get; set; } = 500;
        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                if (value == "Exception of type 'Pertuk.Common.Exceptions.PertukApiException' was thrown.")
                {
                    value = _message;
                }
                else
                {
                    _message = value;
                }
            }
        }
    }
}