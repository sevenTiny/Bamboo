using System;
using System.Collections.Generic;
using System.Text;
using Test.SevenTiny.Bantina.Bankinate.Model;
using Xunit;

namespace Test.SevenTiny.Bantina.Bankinate
{
    public class MysqlTest
    {
        [Fact]
        public void QueryAll()
        {
            using (var db = new MySqlDb())
            {
                //var re1 = db.Queryable<Student>().Where(t => t.Name.Equals("3")).ToList();
                //var re5 = db.Queryable<Student>().Where(t => t.Name.Equals("3")).Where(t => t.Age == 3).ToList();
                //var re2 = db.Queryable<Student>().Where(t => t.Age == 3).Select(t => new { t.Age, t.Name }).ToList();
                //var re3 = db.Queryable<Student>().Where(t => t.Age == 3).Select(t => new { t.Age, t.Name }).OrderBy(t => t.Age).ToList();
                //var re4 = db.Queryable<Student>().Where(t => t.Age == 3).Select(t => new { t.Age, t.Name }).OrderBy(t => t.Age).Paging(1, 10).ToList();
            }
        }
    }
}
