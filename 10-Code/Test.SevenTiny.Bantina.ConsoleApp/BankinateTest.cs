using SevenTiny.Bantina;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Test.SevenTiny.Bantina.Model;
using static Newtonsoft.Json.JsonConvert;

namespace Test.SevenTiny.Bantina.ConsoleApp
{
    public class BankinateTest
    {
        public static void Test()
        {
            BugTest();
        }

        private static void MongoDbTest()
        {
            using (MongoTestDbContext db = new MongoTestDbContext())
            {
                //db.Add<Grade>(new Grade { Name = "Three", GradeId = 9 });

                IList<Grade> dbs = db.QueryList<Grade>(t => true);
                Console.WriteLine(SerializeObject(dbs));
            }
        }

        private static void MySqlDbContextTest()
        {
            var result1 = StopwatchHelper.Caculate(100, () =>
               {
                   using (MySqlTestDbContext db = new MySqlTestDbContext())
                   {
                       //for (int i = 1; i < 4; i++)
                       //{
                       //    Student stu = new Student();
                       //    stu.Age = i;
                       //    stu.Name = $"monky-{i}";
                       //    stu.GradeId = i;
                       //    db.Add(stu);
                       //}

                       //var result = db.QueryOne<Student>(t => t.Name.Equals("monky-6"));
                       //result.Name = "monky-6";
                       //result.Age = 6;
                       //db.Update(t => t.Id == 109, result);

                       //var result = db.QueryCount<Student>(t => t.Name.Contains("1"));
                       //var result = db.QueryListPaging<Student>(3,3,t=>t.Age,t => t.Name.EndsWith("3"),true);

                       //var grades = db.QueryList<Grade2>(t => true);
                       var list = db.QueryList<Student>(t => t.Name.Contains("monky"));
                       //var student = db.QueryOne<Student>(t => true);
                       //Console.WriteLine(student.Name);
                   }
               });
            Console.WriteLine();
            Console.WriteLine($"QueryOne 100 sec：{result1.TotalMilliseconds} ms");
        }

        private static void BankinateCacheTest()
        {
            int i = 0;
            var result1 = StopwatchHelper.Caculate(3, () =>
            {
                using (var db = new MySqlTestDbContext())
                {
                    i++;
                    //Task.Run(() =>
                    //{
                    //    db.AddAsync(new Student { Name = "jony-" + i });
                    //});
                    Console.WriteLine(i);

                    var c1 = db.QueryCount<Student>(t => t.Name.EndsWith("1"));

                    Console.WriteLine(db.IsFromCache);

                    var stu = db.QueryOne<Student>(t => t.Id == 205);

                    Console.WriteLine(db.IsFromCache);

                    var stu2 = db.QueryOne<Student>(t => t.Id == 206);

                    Console.WriteLine(db.IsFromCache);

                    var stu3 = db.QueryOne<Student>(t => t.Id == 207);

                    Console.WriteLine(db.IsFromCache);

                    var c2 = db.QueryCount<Student>(t => t.Name.EndsWith("2"));

                    Console.WriteLine(db.IsFromCache);

                    var stu4 = db.QueryOne<Student>(t => t.Id == 207);

                    Console.WriteLine(db.IsFromCache);

                    var stu5 = db.QueryList<Student>(t => t.Name.StartsWith("monk"));

                    Console.WriteLine(db.IsFromCache);

                    var stu6 = db.QueryOne<Student>(t => t.Name.Contains("m"));

                    Console.WriteLine(db.IsFromCache);

                    var c3 = db.QueryCount<Student>(t => t.Name.EndsWith("3"));

                    Console.WriteLine(db.IsFromCache);

                    var stu7 = db.QueryOne<Student>(t => t.Name.Contains("k"));

                    Console.WriteLine(db.IsFromCache);

                    var list2 = db.QueryListPaging<Student>(1, 2, t => t.GradeId, true);
                    Console.WriteLine("paging " + db.IsFromCache);
                }
            });
            Console.WriteLine();
            Console.WriteLine($"QueryOne {i} sec：{result1.TotalMilliseconds} ms");
        }

        private static void SqlServerTest()
        {
            int i = 0;
            var result1 = StopwatchHelper.Caculate(1000, () =>
            {
                using (var db = new SqlServerTestDbContext())
                {
                    i++;
                    //db.Add(new Student {Id=i, Name = "monky-"+i,GradeId=1 });
                    //Task.Run(() =>
                    //{
                    //    db.AddAsync(new Student { Name = "jony-" + i });
                    //});
                    Console.WriteLine(i);

                    //var c1 = db.QueryCount<Student>(t => t.Name.EndsWith("1"));

                    //Console.WriteLine(db.IsFromCache);

                    //var stu = db.QueryOne<Student>(t => t.Id == 205);

                    //Console.WriteLine(db.IsFromCache);

                    //var stu2 = db.QueryOne<Student>(t => t.Id == 206);

                    //Console.WriteLine(db.IsFromCache);

                    //var stu3 = db.QueryOne<Student>(t => t.Id == 207);

                    //Console.WriteLine(db.IsFromCache);

                    //var c2 = db.QueryCount<Student>(t => t.Name.EndsWith("2"));

                    //Console.WriteLine(db.IsFromCache);

                    //var stu4 = db.QueryOne<Student>(t => t.Id == 207);

                    //Console.WriteLine(db.IsFromCache);

                    //var stu5 = db.QueryList<Student>(t => t.Name.StartsWith("monk"));

                    //Console.WriteLine(db.IsFromCache);

                    //var stu6 = db.QueryOne<Student>(t => t.Name.Contains("m"));

                    //Console.WriteLine(db.IsFromCache);

                    //var c3 = db.QueryCount<Student>(t => t.Name.EndsWith("3"));

                    //Console.WriteLine(db.IsFromCache);

                    //var stu7 = db.QueryOne<Student>(t => t.Name.Contains("k"));

                    //Console.WriteLine(db.IsFromCache);

                    var list2 = db.QueryListPaging<Student>(1, 2, t => t.GradeId, true);
                    Console.WriteLine("paging " + db.IsFromCache);
                }
            });
            Console.WriteLine();
            Console.WriteLine($"QueryOne {i} sec：{result1.TotalMilliseconds} ms");
        }

        private static void BugTest()
        {
            using (var db = new SqlServerTestDbContext())
            {
                Expression<Func<TB_Book, bool>> filter = t => t.IsDelete == 2;
                int count2 = db.QueryCount(filter);

                List<TB_Book> bookList = db.QueryListPaging<TB_Book>(1, 30, t => t.CreateTime, filter, true);
                int count = db.QueryCount<TB_Book>(filter);
            }
        }
    }
}
