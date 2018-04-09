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
            Student5 stu5 = new Student5 { HealthLevel = 100, SchoolClass = new SchoolClass { Name = "class1" } };

            var test0 = StopwatchHelper.Caculate(1000000, () =>
            {
                Student stu = Mapper.AutoMapper<Student,Student5>(stu5, t => t.Name = "jony");
            });
            Console.WriteLine("使用反射调用 1 百万次耗时：");
            Console.WriteLine(test0.TotalMilliseconds);

            Console.WriteLine();

            var test1 = StopwatchHelper.Caculate(1000000, () =>
            {
                Student stu = Mapper<Student5, Student>.AutoMapper(stu5, t => t.Name = "jony");
            });
            Console.WriteLine("使用Expression表达式树调用 1 百万次耗时：");
            Console.WriteLine(test1.TotalMilliseconds);

            Console.WriteLine();

            var test2 = StopwatchHelper.Caculate(1000000, () =>
            {
                Student stu = new Student { HealthLevel = stu5.HealthLevel, Name = "jony" };
            });
            Console.WriteLine("使用代码直接构建 1 百万次耗时：");
            Console.WriteLine(test2.TotalMilliseconds);

            //Student1 stu1 = new Student1 { Uid = Guid.NewGuid() };
            //Student2 stu2 = new Student2 { Name = "jony" };
            //Student3 stu3 = new Student3 { Age = 13 };
            //Student4 stu4 = new Student4 { BodyHigh = 180 };
            //Student5 stu5 = new Student5 { HealthLevel = 100, SchoolClass = new SchoolClass { Name = "class1" } };

            //var normal = StopwatchHelper.Caculate(1000000, () =>
            //{
            //    Student stu = new Student { Uid = stu1.Uid, Name = stu2.Name, Age = stu3.Age, BodyHigh = stu4.BodyHigh, HealthLevel = stu5.HealthLevel };
            //   // Console.WriteLine(stu.BodyHigh);
            //});
            //Console.WriteLine($"normal used:{normal.TotalMilliseconds}");

            //var expression = StopwatchHelper.Caculate(100000, () =>
            //{
            //    Student stu = Mapper<Student1, Student2, Student3, Student4, Student5, Student>.AutoMapper(stu1, stu2, stu3, stu4, stu5, t => { t.Name = "333"; });
            //    //Console.WriteLine(SerializeObject(stu));
            //});
            //Console.WriteLine($"expression used:{expression.TotalMilliseconds}");

            //var reflection = StopwatchHelper.Caculate(100000, () =>
            //{
            //    Student stu = Mapper.AutoMapper<Student, Student1, Student2, Student3, Student4, Student5>(stu1, stu2, stu3, stu4, stu5);
            //});
            //Console.WriteLine($"reflection used:{reflection.TotalMilliseconds}");
        }
    }
}
