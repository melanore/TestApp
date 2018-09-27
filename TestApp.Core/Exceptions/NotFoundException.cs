using System.Net;

namespace TestApp.Core.Exceptions
{
    public class NotFoundException : ApiException
    {
        public NotFoundException(string msg) : base(msg, HttpStatusCode.NotFound) { }
    }
}