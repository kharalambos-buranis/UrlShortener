using UrlShortener.API.Models.Requests;
using UrlShortener.API.Models.Entities;

namespace UrlShortener.API.Data.Repositories
{
    public interface IShortUrlRepository
    {
        Task Delete(string shortCode, CancellationToken cancellationToken);
        Task<ShortUrl?> GetSingle(string shortCode, CancellationToken cancellationToken);
        Task Insert(ShortUrl shortUrl, CancellationToken cancellationToken);
        Task Update(string shortCode,UpdateUrlRequest request, CancellationToken cancellationToken);
        Task IncrementClickCounterAsync(string shortCode);
        Task InsertAnalyticsRecordAsync(string shortCode, DateTime utcNow, string userAgent, string ipAddress);
        Task SetUrlInactiveAsync(string shortCode);
    }
}
