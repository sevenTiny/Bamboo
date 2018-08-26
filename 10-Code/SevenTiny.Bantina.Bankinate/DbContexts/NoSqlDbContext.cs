using System;
using System.Collections.Generic;
using System.Text;

namespace SevenTiny.Bantina.Bankinate.DbContexts
{
    public class NoSqlDbContext : DbContext
    {
        public NoSqlDbContext(DataBaseType dataBaseType) : base(dataBaseType)
        {
        }
    }
}
