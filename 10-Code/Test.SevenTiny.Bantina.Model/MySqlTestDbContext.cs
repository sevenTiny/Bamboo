using SevenTiny.Bantina.Bankinate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Test.SevenTiny.Bantina.Model
{
    public class MySqlTestDbContext : MySqlDbContext<MySqlTestDbContext>
    {
        public MySqlTestDbContext() : base("xxx")
        {
            LocalCache = true;
        }
    }
}
