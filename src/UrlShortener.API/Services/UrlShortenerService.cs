using Cassandra;
using UrlShortener.API.Data.Repositories;
using UrlShortener.API.Models.Entities;
using UrlShortener.API.Models.Requests;
using UrlShortener.API.Models.Responses;
using UrlShortener.API.Services.Exceptions;


namespace UrlShortener.API.Services
{
    public class UrlShortenerService : IUrlShortenerService
    {
        private const int NumberOfCharsInShortLink = 7;
        private const string Characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        private readonly Random _random = new();
        private readonly IShortUrlRepository _repository;
        private readonly IUrlClickAnalyticsRepository _clickAnalyticsRepository;

        public UrlShortenerService(IShortUrlRepository repository, IUrlClickAnalyticsRepository clickAnalyticsRepository)
        {
            _repository = repository;
            _clickAnalyticsRepository = clickAnalyticsRepository;
        }

        public async Task<ShortUrl> ShortenUrl(ShortenUrlRequest request, CancellationToken cancellationToken)
        {
            var shortCode = string.IsNullOrWhiteSpace(request.Alias)
                ? GenerateShortCode()
                : request.Alias;

            if (!string.IsNullOrWhiteSpace(request.Alias))
            {
                var existing = await _repository.GetSingle(shortCode, cancellationToken);
                if (existing != null)
                {
                    throw new ConflictException("The alias is already in use.");
                }
            }

            var shortUrl = new ShortUrl
            {
                ShortCode = shortCode,
                OriginalUrl = request.OriginalUrl,
                CreatedAt = DateTimeOffset.UtcNow,
                ExpiresAt = request.ExpiresAt,
                ClickCount = 0,
                IsActive = true
            };

            await _repository.Insert(shortUrl, cancellationToken);

            return shortUrl;
        }


        public async Task<ShortUrlResponse> GetUrlDetails(string shortCode, CancellationToken cancellationToken)
        {
            var shortUrl = await _repository.GetSingle(shortCode, cancellationToken);

            if (shortUrl is null)
            {
                throw new NotFoundException("The URL not found");
            }

            var analytics = await _clickAnalyticsRepository.Get(shortCode);

            return new ShortUrlResponse
            {
                ShortCode = shortUrl.ShortCode,
                OriginalUrl = shortUrl.OriginalUrl,
                CreatedAt = shortUrl.CreatedAt,
                ExpiresAt = shortUrl.ExpiresAt,
                ClickCount = shortUrl.ClickCount,
                IsActive = shortUrl.IsActive,
                Clicks = analytics.ToList()
            };
        }

        public async Task DeactivateUrlAsync(string shortCode)
        {
            await _repository.SetUrlInactiveAsync(shortCode);

            throw new NotFoundException("This URL has expired");
        }

        public async Task DeactivateExpiredUrlsAsync(CancellationToken cancellationToken)
        {
            var expiredUrls = await _repository.GetExpiredUrlsAsync(cancellationToken);

            foreach (var url in expiredUrls)
            {
                await _repository.SetUrlInactiveAsync(url.ShortCode, cancellationToken);
            }
        }

        public async Task RecordClickAsync(string shortCode, string userAgent, string ipAddress)
        {
            await _repository.IncrementClickCounterAsync(shortCode);
            await _clickAnalyticsRepository.Insert(shortCode, DateTime.UtcNow, userAgent, ipAddress);
        }

        public async Task UpdateUrl(string shortCode,UpdateUrlRequest request, CancellationToken cancellationToken)
        {
            var shortUrl = await _repository.GetSingle(shortCode, cancellationToken);

            if (shortUrl is null)
            {
                throw new NotFoundException("The URL not found");
            }

            await _repository.Update(shortCode, request, cancellationToken);
        }

        public Task DeleteUrl(string shortCode, CancellationToken cancellationToken)
        {
            var shortUrl = _repository.GetSingle(shortCode, cancellationToken);

            if (shortUrl is null)
            {
                throw new NotFoundException("The URL not found");
            }

            return _repository.Delete(shortCode, cancellationToken);
        }

        private string GenerateShortCode()
        {
            var codeChars = new char[NumberOfCharsInShortLink];

            for (int i = 0; i < NumberOfCharsInShortLink; i++)
            {
                var randomIndex = _random.Next(Characters.Length - 1);

                codeChars[i] = Characters[randomIndex];
            }

            return new string(codeChars);
        }
    }
}
