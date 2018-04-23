using SevenTiny.Bantina.Bankinate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Test.SevenTiny.Bantina.Model
{
    public class MySqlTestDbContext : MySqlDbContext<MySqlTestDbContext>
    {
        public MySqlTestDbContext() : base("server=101.201.66.247;Port=39901;database=test;uid=root;pwd=CYj(9yyz*8;Allow User Variables=true;")
        {
            LocalCache = true;
        }
    }
}
