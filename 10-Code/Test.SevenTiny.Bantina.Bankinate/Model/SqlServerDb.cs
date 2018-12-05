using SevenTiny.Bantina.Bankinate;
using SevenTiny.Bantina.Bankinate.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Test.SevenTiny.Bantina.Bankinate.Model
{
    [DataBase("Test")]
    public class SqlServerDb : SqlServerDbContext<SqlServerDb>
    {
        public SqlServerDb() : base("data source=.;initial catalog=Test;persist security info=True;user id=sa;password=123456;MultipleActiveResultSets=True;App=EntityFramework")
        {
            OpenTableCache = true;
            OpenQueryCache = false;
        }
    }
}
