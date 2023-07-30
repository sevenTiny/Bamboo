using SevenTiny.Bantina.Bankinate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bamboo.Configuration
{
    internal class ConfigDbContext : MySqlDbContext<ConfigDbContext>
    {
        public ConfigDbContext(string connectionString) : base(connectionString)
        {
        }
    }
}
