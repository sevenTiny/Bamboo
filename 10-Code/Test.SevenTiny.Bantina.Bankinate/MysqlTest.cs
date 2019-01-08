using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Test.SevenTiny.Bantina.Bankinate.Model;
using Xunit;

namespace Test.SevenTiny.Bantina.Bankinate
{
    public class MysqlTest
    {
        public MySqlDb Db => new MySqlDb();

        [Fact]
        [Trait("desc", "初始化测试数据，当跑全部下列用例的时候，删除所有数据并执行预置数据操作！")]
        public void InitTestDatas()
        {
            using (var db = new MySqlDb())
            {
                //清空所有数据
                db.Delete<Student>(t => true);

                //预置测试数据
                List<Student> students = new List<Student>();
                for (int i = 0; i < 1000; i++)
                {
                    students.Add(new Student { Name = "7tiny_" + i, Age = i, SchoolTime = DateTime.Now });
                }
                db.Add<Student>(students);
            }
            Assert.True(true);
        }

        [Fact]
        public void Query_Where()
        {
            var re = Db.Queryable<Student>().Where(t => t.Name.EndsWith("3")).ToList();
            Assert.Equal(100, re.Count);
        }

        [Fact]
        public void Query_Where_Multi()
        {
            var re = Db.Queryable<Student>().Where(t => t.Name.Contains("3")).Where(t => t.Age == 3).ToList();
            Assert.Single(re);
        }

        [Fact]
        public void Query_Select()
        {
            var re = Db.Queryable<Student>().Where(t => t.Age < 3).Select(t => new { t.Age, t.Name }).ToList();
            Assert.Equal(3, re.Count);
        }

        [Fact]
        public void Query_OrderBy()
        {
            var re = Db.Queryable<Student>().Where(t => t.Age < 9).Select(t => new { t.Age, t.Name }).OrderByDescending(t => t.Age).ToList();
            Assert.True(re.Count == 9 && re.First().Age == 8 && re.First().Id == 0);//没有查id，id应该=0
        }

        [Fact]
        public void Query_Top()
        {
            var re = Db.Queryable<Student>().Where(t => t.Age > 3).Select(t => new { t.Age, t.Name }).OrderByDescending(t => t.Age).Top(30).ToList();
            Assert.Equal(30, re.Count);
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
