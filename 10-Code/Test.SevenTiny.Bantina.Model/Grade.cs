using SevenTiny.Bantina.AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Test.SevenTiny.Bantina.Model
{
    public class Grade
    {
        public int Id { get; set; }
        [Mapper("GradeName")]
        public string Name { get; set; }
    }
}
