using System;
using System.Net;

namespace TestApp.Core.Exceptions
{
    public class ApiException : Exception
    {
        public ApiException(string message, HttpStatusCode? statusCode) : base(message)
        {
            StatusCode = statusCode ?? HttpStatusCode.InternalServerError;
        }

        public ApiException(string message, HttpStatusCode? statusCode, Exception innerException) : base(message, innerException)
        {
            StatusCode = statusCode ?? HttpStatusCode.InternalServerError;
        }

        public HttpStatusCode StatusCode { get; }
    }
}