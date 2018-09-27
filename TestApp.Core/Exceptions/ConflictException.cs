using System.Net;

namespace TestApp.Core.Exceptions
{
    public class ConflictException : ApiException
    {
        public ConflictException(string msg) : base(msg, HttpStatusCode.Conflict) { }
    }
}