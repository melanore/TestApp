using System.Collections.Generic;
using System.Net;

namespace TestApp.Core.Exceptions
{
    public class ValidationException : ApiException
    {
        public ValidationException(string msg) : base(msg, HttpStatusCode.UnprocessableEntity) { }
        public ValidationException(string msg, List<string> details) : base(msg, HttpStatusCode.UnprocessableEntity, details) { }
    }
}