using System;
using System.Collections.Generic;
using System.Text;
using Test.Model;

namespace Test.SevenTiny.Bantina.Bankinate
{
    public class SqlServerDbContextTest
    {
        public void Test()
        {
            using (var db = new SqlServerTestDbContext())
            {
            }
        }
    }
}
