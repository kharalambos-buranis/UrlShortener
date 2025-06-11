namespace UrlShortener.API.Data
{
    public class CassandraOptions
    {
        public string ContactPoint { get; set; }
        public int Port { get; set; } 
        public string Keyspace { get; set; } 
    }
}

