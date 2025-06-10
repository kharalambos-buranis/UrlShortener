using UrlShortener.API.Models.Entities;
using UrlShortener.API.Data.Repositories;
using UrlShortener.API.Models.Requests;
using UrlShortener.API.Services.Exceptions;

namespace UrlShortener.API.Services
{
    public class UrlShortenerService : IUrlShortenerService
    {
        private const int NumberOfCharsInShortLink = 7;
        private const string Characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        private readonly Random _random = new();
        private readonly IShortUrlRepository _repository;

        public UrlShortenerService(IShortUrlRepository repository)
        {
            _repository = repository;
        }

        public async Task<ShortUrl> ShortenUrl(ShortenUrlRequest request, CancellationToken cancellationToken)
        {
            var shortUrl = new ShortUrl
            {
                ShortCode = GenerateShortCode(),
                OriginalUrl = request.OriginalUrl,
                CreatedAt = DateTimeOffset.Now,
                ExpiresAt = request.ExpiresAt,
                ClickCount = 0,
                IsActive = true
            };

            await _repository.Insert(shortUrl, cancellationToken);

            return shortUrl;
        }

        public async Task RecordClickAsync(string shortCode, string userAgent, string ipAddress)
        {
            await _cassandra.IncrementClickCounterAsync(shortCode);
            await _cassandra.InsertAnalyticsRecordAsync(shortCode, DateTime.UtcNow, userAgent, ipAddress);
        }

        public async Task<ShortUrl> GetUrlDetails(string shortCode, CancellationToken cancellationToken)
        {
            var shortUrl = await _repository.GetSingle(shortCode, cancellationToken);

            if (shortUrl is null)
            {
                throw new NotFoundException("The URL not found");
            }

            return shortUrl;
        }

        public async Task UpdateUrl(string shortCode,UpdateUrlRequest request, CancellationToken cancellationToken)
        {
            await _repository.Update(shortCode, request, cancellationToken);
        }

        public Task DeleteUrl(string shortCode, CancellationToken cancellationToken)
        {
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
