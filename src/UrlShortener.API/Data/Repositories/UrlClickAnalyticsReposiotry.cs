

using Cassandra;
using System.Net.WebSockets;
using UrlShortener.API.Models.Entities;
using UrlShortener.API.Models.Responses;

namespace UrlShortener.API.Data.Repositories
{
    public class UrlClickAnalyticsReposiotry : IUrlClickAnalyticsRepository
    {
        private readonly Cassandra.ISession _session;

        public UrlClickAnalyticsReposiotry(Cassandra.ISession session)
        {
            _session = session;
        }

        public async Task Insert(string shortCode, DateTime clickDate, string userAgent, string ip)
        {
            var preparedStatement = await _session.PrepareAsync( "INSERT INTO UrlClickAnalytics (short_code, clicked_at, user_agent, ip_address) VALUES (?, ?, ?, ?)");
            await _session.ExecuteAsync(preparedStatement.Bind(shortCode, clickDate, userAgent, ip));
        }

        public async Task<IEnumerable<UrlClickAnalyticsResponse>> Get(string shortCode)
        {
            var preparedStatement = await _session.PrepareAsync("SELECT clicked_at, user_agent,ip_address FROM UrlClickAnalytics WHERE short_code = ?");

            var rowset = await _session.ExecuteAsync(preparedStatement.Bind(shortCode));

            return rowset.Select(row => new UrlClickAnalyticsResponse
            {
                //ShortCode = row.GetValue<string>("short_code"),
                ClickedAt = row.GetValue<DateTime>("clicked_at"),
                UserAgent = row.GetValue<string>("user_agent"),
                IpAddress = row.GetValue<string>("ip_address")
            });
        }

    }
}
