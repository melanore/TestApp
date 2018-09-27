using System;
using System.Collections.Generic;
using System.Net;

namespace TestApp.Core.Exceptions
{
    public class ApiException : Exception
    {
        public ApiException(string message, HttpStatusCode? statusCode, List<string> details = null) : base(message)
        {
            StatusCode = statusCode ?? HttpStatusCode.InternalServerError;
            Details = details;
        }

        public ApiException(string message, HttpStatusCode? statusCode, Exception innerException, List<string> details = null) : base(message, innerException)
        {
            StatusCode = statusCode ?? HttpStatusCode.InternalServerError;
            Details = details;
        }

        public HttpStatusCode StatusCode { get; }
        public List<string> Details { get; }
    }
}