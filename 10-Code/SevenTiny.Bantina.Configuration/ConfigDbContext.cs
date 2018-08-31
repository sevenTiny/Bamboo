using SevenTiny.Bantina.Bankinate;

namespace SevenTiny.Bantina.Configuration
{
    internal class ConfigDbContext : MySqlDbContext<ConfigDbContext>
    {
        public ConfigDbContext(string connectionString) : base(connectionString)
        {
            OpenTableCache = false;
        }
    }
}
