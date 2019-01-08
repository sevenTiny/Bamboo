using System;
using System.Collections.Generic;
using System.Text;
using Test.SevenTiny.Bantina.Bankinate.Model;
using Xunit;

namespace Test.SevenTiny.Bantina.Bankinate
{
    public class MysqlTest
    {
        public MySqlDb Db => new MySqlDb();

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
        public void Query_Where()
        {
            var re = Db.Queryable<Student>().Where(t => t.Name.EndsWith("3")).ToList();
            var re5 = Db.Queryable<Student>().Where(t => t.Name.Equals("3")).Where(t => t.Age == 3).ToList();
        }

        [Fact]
        public void Query_Select()
        {
            var re2 = Db.Queryable<Student>().Where(t => t.Age == 3).Select(t => new { t.Age, t.Name }).ToList();
        }

        [Fact]
        public void Query_OrderBy()
        {
            var re3 = Db.Queryable<Student>().Where(t => t.Age == 3).Select(t => new { t.Age, t.Name }).OrderBy(t => t.Age).ToList();
        }

        [Fact]
        public void Query_Paging()
        {
            var re4 = Db.Queryable<Student>().Where(t => t.Name.Contains("1")).Select(t => new { t.Age, t.Name }).OrderBy(t => t.Age).Paging(0, 10).ToList();
            var re5 = Db.Queryable<Student>().Where(t => t.Name.Contains("1")).Select(t => new { t.Age, t.Name }).OrderByDescending(t => t.Age).Paging(0, 10).ToList();
            var re6 = Db.Queryable<Student>().Where(t => t.Name.Contains("1")).Select(t => new { t.Age, t.Name }).OrderBy(t => t.Age).Paging(1, 10).ToList();
        }
    }
}
