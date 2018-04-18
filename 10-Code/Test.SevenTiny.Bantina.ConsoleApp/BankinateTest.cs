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
            MongoDbTest();
        }

        private static void MongoDbTest()
        {
            using (TestDbContext db = new TestDbContext ())
            {
                //db.Add<Grade>(new Grade { Name = "Three", GradeId = 9 });

                IList<Grade> dbs = db.Query<Grade>(t => true);
                Console.WriteLine(SerializeObject(dbs));
            }
        }
    }
}
