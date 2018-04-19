using System;
using System.Collections.Generic;
using System.Text;
using Test.SevenTiny.Bantina.Model;
using static Newtonsoft.Json.JsonConvert;

namespace Test.SevenTiny.Bantina.ConsoleApp
{
    public class BankinateTest
    {
        public static void Test()
        {
            MySqlDbContextTest();
        }

        private static void MongoDbTest()
        {
            using (MongoTestDbContext db = new MongoTestDbContext ())
            {
                //db.Add<Grade>(new Grade { Name = "Three", GradeId = 9 });

                IList<Grade> dbs = db.Query<Grade>(t => true);
                Console.WriteLine(SerializeObject(dbs));
            }
        }

        private static void MySqlDbContextTest()
        {
            using (MySqlTestDbContext db = new MySqlTestDbContext())
            {
                //for (int i = 0; i < 100; i++)
                //{
                //    Student stu = new Student();
                //    stu.Age = i;
                //    stu.Name = $"monky-{i}";
                //    db.Add(stu);
                //}

            }
        }
    }
}
