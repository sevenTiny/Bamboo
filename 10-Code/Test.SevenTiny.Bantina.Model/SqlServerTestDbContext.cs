using SevenTiny.Bantina.Bankinate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Test.SevenTiny.Bantina.Model
{
    public class SqlServerTestDbContext : SqlServerDbContext<SqlServerTestDbContext>
    {
        public SqlServerTestDbContext() : base("Data Source=.;Initial Catalog=Test;Persist Security Info=True;User ID=Sa;Password=CYj(9yyz*8")
        {
            LocalCache = true;
        }
    }
}
