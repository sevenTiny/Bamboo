using SevenTiny.Bantina.Bankinate;
using SevenTiny.Bantina.Bankinate.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Test.SevenTiny.Bantina.Bankinate.Model
{
    [DataBase("SevenTinyTest")]
    public class SqlServerDb : SqlServerDbContext<SqlServerDb>
    {
        public SqlServerDb() : base("Data Source=.;Initial Catalog=SevenTinyTest;Integrated Security=True")
        {
            
        }
    }
}
