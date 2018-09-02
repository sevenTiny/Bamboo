using SevenTiny.Bantina.Bankinate;
using SevenTiny.Bantina.Bankinate.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Test.Model
{
    [DataBase("Test")]
    public class SqlServerTestDbContext : SqlServerDbContext<SqlServerTestDbContext>
    {
        public SqlServerTestDbContext() : base("data source=.;initial catalog=Test;persist security info=True;user id=sa;password=CYj(9yyz*8;MultipleActiveResultSets=True;App=EntityFramework")
        {
            OpenTableCache = true;
            OpenQueryCache = false;
        }
    }
}
