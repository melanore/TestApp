using System.Collections.Generic;

namespace TestApp.Web.Filters
{
    public class ApiError
    {
        public ApiError(string message, string stackTrace)
        {
            Message = message;
            Details = new List<string>();
            StackTrace = stackTrace;
        }

        public string Message { get; }
        public List<string> Details { get; }
        public string StackTrace { get; set; }
    }
}