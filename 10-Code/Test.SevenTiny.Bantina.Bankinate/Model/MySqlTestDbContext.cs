using SevenTiny.Bantina.Bankinate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Test.SevenTiny.Bantina.Bankinate.Model
{
    public class MySqlTestDbContext : MySqlDbContext<MySqlTestDbContext>
    {
        public MySqlTestDbContext() : base("")
        {
        }
    }
}
