namespace UrlShortener.API.Models.Requests
{
    public class ShortenUrlRequest
    {
        public string OriginalUrl { get; set; }
        public DateTimeOffset? ExpiresAt { get; set; }
        public string Alias { get; set; }
    }
}
