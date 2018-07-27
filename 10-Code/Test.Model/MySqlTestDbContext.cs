using SevenTiny.Bantina.Bankinate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Test.Model
{
    public class MySqlTestDbContext : MySqlDbContext<MySqlTestDbContext>
    {
        public MySqlTestDbContext() : base("")
        {
            LocalCache = true;
        }
    }
}
