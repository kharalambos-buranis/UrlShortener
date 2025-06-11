using UrlShortener.API.Models.Entities;
using UrlShortener.API.Models.Requests;

namespace UrlShortener.API.Data.Repositories
{
    using Cassandra;

    namespace UrlShortener.API.Data.Repositories
    {
        public class ShortUrlRepository : IShortUrlRepository
        {
            private readonly Cassandra.ISession _session;

            public ShortUrlRepository(Cassandra.ISession session)
            {
                _session = session;
            }

            public async Task Insert(ShortUrl shortUrl, CancellationToken cancellationToken)
            {
                var preparedStatement = await _session.PrepareAsync("INSERT INTO ShortUrls (short_code, original_url, created_at, expires_at, click_count, is_active) values (?, ?, ?, ?, ?, ?)");

                await _session.ExecuteAsync(preparedStatement.Bind(
                    shortUrl.ShortCode,
                    shortUrl.OriginalUrl,
                    shortUrl.CreatedAt,
                    shortUrl.ExpiresAt,
                    shortUrl.ClickCount,
                    shortUrl.IsActive));
            }

            public async Task<ShortUrl?> GetSingle(string shortCode, CancellationToken cancellationToken)
            {
                var preparedStatement = await _session.PrepareAsync("SELECT * FROM ShortUrls WHERE short_code = ?");

                var rowSet = await _session.ExecuteAsync(preparedStatement.Bind(shortCode));

                var row = rowSet.FirstOrDefault();

                return MapRowToShortUrl(row);
            }
            
            public async Task SetUrlInactiveAsync(string shortCode)
            {
                var query = "UPDATE ShortUrls SET is_active = false WHERE short_code = ?";
                await _session.ExecuteAsync(new SimpleStatement(query, shortCode));
            }

            public async Task IncrementClickCounterAsync(string shortCode)
            {
                var query = "UPDATE UrlClickCounters SET click_count = click_count + 1 WHERE short_code = ?";
                await _session.ExecuteAsync(new SimpleStatement(query, shortCode));
            }

            public async Task InsertAnalyticsRecordAsync(string shortCode, DateTime clickDate, string userAgent, string ip)
            {
                var query = "INSERT INTO UrlClickAnalytics (short_code, clicked_at, user_agent, ip_address) VALUES (?, ?, ?, ?)";
                await _session.ExecuteAsync(new SimpleStatement(query, shortCode, clickDate, userAgent, ip));
            }
            //hjZpcxj
            //wtfQD16   exp
            public async Task Update(string shortCode,UpdateUrlRequest request, CancellationToken cancellationToken)
            {
                var preparedStatement = await _session.PrepareAsync("UPDATE ShortUrls SET original_url = ?, expires_at = ? WHERE short_code = ?");

                await _session.ExecuteAsync(preparedStatement.Bind(request.OriginalUrl, request.ExpiresAt, shortCode));
            }

            public async Task Delete(string shortCode, CancellationToken cancellationToken)
            {
                var preparedStatement = await _session.PrepareAsync("DELETE FROM ShortUrls WHERE short_code = ?");

                await _session.ExecuteAsync(preparedStatement.Bind(shortCode));
            }

            private ShortUrl? MapRowToShortUrl(Row? row)
            {
                return row is null ?
                    null :
                    new ShortUrl
                    {
                        ShortCode = row.GetValue<string>("short_code"),
                        OriginalUrl = row.GetValue<string>("original_url"),
                        CreatedAt = row.GetValue<DateTime>("created_at"),
                        ExpiresAt = row.GetValue<DateTime?>("expires_at"),
                        IsActive = row.GetValue<bool>("is_active"),
                        ClickCount = row.GetValue<int>("click_count")
                    };
            }
        }
    }



}
