using SevenTiny.Bantina;
using SevenTiny.Bantina.AutoMapper;
using System;
using Test.SevenTiny.Bantina.Model;
using static Newtonsoft.Json.JsonConvert;

namespace Test.SevenTiny.Bantina.ConsoleApp
{
    public class AutoMapperTest
    {
        public static void Test()
        {
            Student student = new Student
            {
                Uid = Guid.NewGuid(),
                Age = 11,
                Name = "bankinate"
            };

            School school = new School
            {
                Age = 22,
                Name = "美国加州斯坦福大学",
                Is211 = true,
                Is985 = false
            };

            Student1 stu1 = new Student1 { Uid = Guid.NewGuid() };
            Student2 stu2 = new Student2 { Name = "jony" };
            Student3 stu3 = new Student3 { Age = 13 };
            Student4 stu4 = new Student4 { BodyHigh = 180 };
            Student5 stu5 = new Student5 { HealthLevel = 100 };

            var reflection = StopwatchHelper.Caculate(100000, () =>
            {
                Student stu = Mapper.AutoMapper<Student, Student1, Student2, Student3, Student4, Student5>(stu1, stu2, stu3, stu4, stu5, t => t.Age = 200);
                Console.WriteLine(stu.Age);
            });

            var normal = StopwatchHelper.Caculate(100000, () =>
            {
                Student stu = new Student { Uid = stu1.Uid, Name = stu2.Name, Age = stu3.Age, BodyHigh = stu4.BodyHigh, HealthLevel = stu5.HealthLevel };
                Console.WriteLine(stu.BodyHigh);
            });

            Console.WriteLine($"reflection used:{reflection.TotalMilliseconds}");
            Console.WriteLine($"normal used:{normal.TotalMilliseconds}");
        }
    }
}
