using System;
using System.Collections.Generic;
using System.Text;
using Test.SevenTiny.Bantina.Bankinate.Model;
using Xunit;

namespace Test.SevenTiny.Bantina.Bankinate
{
    public class MysqlTest
    {
        /// <summary>
        /// 初始化测试数据
        /// </summary>
        [Fact]
        public void InitTestDatas()
        {
            using (var db = new MySqlDb())
            {
                List<Student> students = new List<Student>();
                for (int i = 0; i < 1000; i++)
                {
                    students.Add(new Student { Name = "7tiny_" + i, Age = i, SchoolTime = DateTime.Now });
                }
                db.Add<Student>(students);
            }
        }

        [Fact]
        public void QueryAll()
        {
            using (var db = new MySqlDb())
            {
                var re1 = db.Queryable<Student>().Where(t => t.Name.EndsWith("3")).ToList();
                //var re5 = db.Queryable<Student>().Where(t => t.Name.Equals("3")).Where(t => t.Age == 3).ToList();
                //var re2 = db.Queryable<Student>().Where(t => t.Age == 3).Select(t => new { t.Age, t.Name }).ToList();
                //var re3 = db.Queryable<Student>().Where(t => t.Age == 3).Select(t => new { t.Age, t.Name }).OrderBy(t => t.Age).ToList();
                //var re4 = db.Queryable<Student>().Where(t => t.Age == 3).Select(t => new { t.Age, t.Name }).OrderBy(t => t.Age).Paging(1, 10).ToList();
            }
        }
    }
}
