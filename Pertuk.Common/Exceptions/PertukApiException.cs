using System;

namespace Pertuk.Common.Exceptions
{
    public class PertukApiException : Exception
    {
        public PertukApiException()
        {

        }

        public PertukApiException(string message) : base(message)
        {

        }

        public PertukApiException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}