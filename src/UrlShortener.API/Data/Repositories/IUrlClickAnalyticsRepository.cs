using UrlShortener.API.Models.Entities;
using UrlShortener.API.Models.Responses;

namespace UrlShortener.API.Data.Repositories
{
    public interface IUrlClickAnalyticsRepository
    {
        public Task Insert(string shortCode, DateTime clickDate, string userAgent, string ip);
        public Task<IEnumerable<UrlClickAnalyticsResponse>> Get(string shortCode);
    }
}
