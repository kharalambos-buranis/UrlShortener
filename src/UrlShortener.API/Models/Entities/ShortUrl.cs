namespace UrlShortener.API.Models.Entities
{
    public class ShortUrl
    {
        public string ShortCode { get; set; }
        public string OriginalUrl { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? ExpiresAt { get; set; }
        public int ClickCount { get; set; }
        public bool IsActive { get; set; }

    }
}
