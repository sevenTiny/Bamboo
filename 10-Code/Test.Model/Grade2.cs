using SevenTiny.Bantina.Bankinate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Test.Model
{
    [Table("Grade")]
    public class Grade2
    {
        [Key]
        [Column]
        public int Id { get; set; }
        [Column]
        public string Name { get; set; }
    }
}
