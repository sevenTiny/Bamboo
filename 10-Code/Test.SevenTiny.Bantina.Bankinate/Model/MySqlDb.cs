using SevenTiny.Bantina.Bankinate;
using SevenTiny.Bantina.Bankinate.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Test.SevenTiny.Bantina.Bankinate.Model
{
    [DataBase("test")]
    public class MySqlDb : MySqlDbContext<MySqlDb>
    {
        public MySqlDb() : base("localhost")
        {
        }
    }
}
