using System.Net;

namespace UrlShortener.API.Services.Exceptions
{
    public class ConflictException : BaseException
    {
        public ConflictException(string? message) : base(message) { }

        public override string Title => "Conflict";
        public override HttpStatusCode StatusCode => HttpStatusCode.Conflict;
    }
}
