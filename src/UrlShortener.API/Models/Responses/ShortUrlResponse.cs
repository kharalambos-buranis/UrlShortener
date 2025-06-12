namespace UrlShortener.API.Models.Responses
{
    public class ShortUrlResponse
    {
        public string ShortCode { get; set; }
        public string OriginalUrl { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? ExpiresAt { get; set; }
        public long ClickCount { get; set; }
        public bool IsActive { get; set; }
        public List<UrlClickAnalyticsResponse> Clicks { get; set; }
    }
}
