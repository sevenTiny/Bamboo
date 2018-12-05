using SevenTiny.Bantina.Bankinate;
using SevenTiny.Bantina.Bankinate.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Test.SevenTiny.Bantina.Bankinate.Model
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
    }
}
