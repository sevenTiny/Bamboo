using SevenTiny.Bantina.Bankinate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Test.Model
{
    public class SqlServerTestDbContext : SqlServerDbContext<SqlServerTestDbContext>
    {
        public SqlServerTestDbContext() : base("data source=.;initial catalog=DB_QX_Frame_MS_CMS;persist security info=True;user id=sa;password=CYj(9yyz*8;MultipleActiveResultSets=True;App=EntityFramework")
        {
            TableCache = true;
        }
    }
}
