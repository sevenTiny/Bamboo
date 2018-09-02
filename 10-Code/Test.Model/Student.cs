using SevenTiny.Bantina.Bankinate;
using SevenTiny.Bantina.Bankinate.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Test.Model
{
    [Table("Student")]
    [TableCaching]
    public class Student
    {
        [AutoIncrease]
        [Key]
        public int Id { get; set; }
        [Column]
        public string Name { get; set; }
        [Column]
        public int Age { get; set; }
        [Column]
        public int GradeId { get; set; }
        public Grade2 Grade { get; set; }

        public int BodyHigh { get; set; }
        public int HealthLevel { get; set; }

        public string GetName()
        {
            return this.Name;
        }
    }
}
