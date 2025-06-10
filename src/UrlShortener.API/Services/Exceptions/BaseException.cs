using System.Net;

namespace UrlShortener.API.Services.Exceptions
{
    public abstract class BaseException : Exception
    {
        protected BaseException(string? message) : base(message)
        {
        }

        public abstract string Title { get; }
        public abstract HttpStatusCode StatusCode { get; }
    }
}
