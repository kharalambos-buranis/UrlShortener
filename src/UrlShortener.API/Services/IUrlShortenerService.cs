using UrlShortener.API.Models.Entities;
using UrlShortener.API.Models.Requests;
using UrlShortener.API.Models.Responses;

namespace UrlShortener.API.Services
{
    public interface IUrlShortenerService
    {
        Task<ShortUrl> ShortenUrl(ShortenUrlRequest request, CancellationToken cancellationToken);
        public Task<ShortUrlResponse> GetUrlDetails(string shortCode, CancellationToken cancellationToken);
        Task UpdateUrl(string shortCode,UpdateUrlRequest request, CancellationToken cancellationToken);
        Task DeleteUrl(string shortCode, CancellationToken cancellationToken);
        Task DeactivateUrlAsync(string shortCode);
        Task RecordClickAsync(string shortCode, string userAgent, string ipAddress);
        public Task DeactivateExpiredUrlsAsync(CancellationToken cancellationToken);
    }
}
