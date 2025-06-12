namespace UrlShortener.API.Models.Responses
{
    public class UrlClickAnalyticsResponse
    {
        public DateTimeOffset ClickedAt { get; set; }
        public string UserAgent { get; set; }
        public string IpAddress { get; set; } 
    }
}
