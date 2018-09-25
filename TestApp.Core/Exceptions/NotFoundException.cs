using System;
using System.Net;

namespace TestApp.Core.Exceptions
{
    public class NotFoundException : ApiException
    {
        public NotFoundException(string msg) : base(msg, HttpStatusCode.NotFound)
        {
        }

        public NotFoundException(string msg, Exception innerException) : base(msg, HttpStatusCode.NotFound, innerException)
        {
        }
    }
}