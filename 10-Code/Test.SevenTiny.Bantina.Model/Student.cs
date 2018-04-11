using System;
using System.Collections.Generic;
using System.Text;

namespace Test.SevenTiny.Bantina.Model
{
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public int BodyHigh { get; set; }
        public int HealthLevel { get; set; }

        public string GetName()
        {
            return this.Name;
        }
    }
}
