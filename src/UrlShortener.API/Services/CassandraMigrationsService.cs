namespace UrlShortener.API.Services
{
    public class CassandraMigrationService
    {
        private readonly Cassandra.ISession _session;

        public CassandraMigrationService(Cassandra.ISession session)
        {
            _session = session;
        }

        public void Migrate()
        {
            _session.Execute(@"
             CREATE TABLE IF NOT EXISTS ShortUrls (
                 short_code text PRIMARY KEY,
                 original_url text,
                 created_at timestamp,
                 expires_at timestamp,
                 click_count bigint,
                 is_active boolean
             );
         ");

            _session.Execute(@"
             CREATE TABLE IF NOT EXISTS UrlClickAnalytics (
                 short_code text,
                 clicked_at timestamp,
                 user_agent text,
                 ip_address text,
                 PRIMARY KEY (short_code, clicked_at)
             ) WITH CLUSTERING ORDER BY (clicked_at DESC);
         ");

        }
    }
}
