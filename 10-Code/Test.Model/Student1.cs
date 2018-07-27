using SevenTiny.Bantina.AutoMapper;
using System;

namespace Test.Model
{
    [MapperClassAttribute(Name = "Student1")]
    public class Student1
    {
        public Guid Uid { get; set; }
    }
}
