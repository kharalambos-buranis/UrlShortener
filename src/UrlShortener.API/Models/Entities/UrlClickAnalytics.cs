namespace UrlShortener.API.Models.Entities
{
    public class UrlClickAnalytics
    {
        public string ShortCode { get; set; }
        public DateTimeOffset ClickedAt { get; set; }
        public string UserAgent { get; set; }
        public string IpAddress { get; set; }

    }
}
