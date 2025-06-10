using System.Text.Json.Serialization;

namespace UrlShortener.API.Models.Requests
{
    public class UpdateUrlRequest
    {
       // [JsonIgnore]
      //  public string ShortCode { get; set; }

        public string OriginalUrl { get; set; }
        public DateTimeOffset? ExpiresAt { get; set; }
    }
}
