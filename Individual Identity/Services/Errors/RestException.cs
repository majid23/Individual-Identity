using System.Net;

namespace Individual_Identity.Services.Errors
{
    public class RestException : Exception
    {
        public RestException(HttpStatusCode code, string errors = null)
        {
            Code = code;
            Errors = errors;
        }

        public HttpStatusCode Code { get; }
        public string Errors { get; }
    }
}
