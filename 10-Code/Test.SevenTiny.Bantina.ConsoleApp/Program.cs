using SevenTiny.Bantina;
using SevenTiny.Bantina.Internationalization;
using SevenTiny.Bantina.Logging;
using SevenTiny.Bantina.Redis;
using System;
using System.Threading;
using System.Threading.Tasks;
using SevenTiny.Bantina.Security;
using Test.SevenTiny.Bantina.Model;
using Test.SevenTiny.Bantina.Model.DTO;
using SevenTiny.Bantina.AutoMapper;
using static Newtonsoft.Json.JsonConvert;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Test.SevenTiny.Bantina.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
<<<<<<< Updated upstream
            //ILog logger = new LogManager();

            //IRedisCache redis = RedisManager.Instance;
            //redis.Post("name", $"zhangsan");
=======
>>>>>>> Stashed changes



<<<<<<< Updated upstream
            Student student = new Student
            {
                Uid = Guid.NewGuid(),
                Age = 16,
                Name = "bankinate"
            };

            StudentReferee stuRef = Mapper.AutoMapper<StudentReferee, Student>(student);

            Console.WriteLine(SerializeObject(stuRef));
=======
>>>>>>> Stashed changes

            Console.WriteLine("any key to exit ...");
            Console.ReadKey();
        }
    }
}
