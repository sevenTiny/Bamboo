using SevenTiny.Bantina.Bankinate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Test.SevenTiny.Bantina.Bankinate.Model
{
    public class MySqlDb : MySqlDbContext<MySqlDb>
    {
        public MySqlDb() : base("")
        {
        }
    }
}
