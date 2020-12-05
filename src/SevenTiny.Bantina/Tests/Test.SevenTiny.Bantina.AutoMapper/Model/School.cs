using SevenTiny.Bantina.AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Test.Model
{
   public class School
    {
        public Guid Uid { get; set; }
        [Mapper("SchoolName")]
        public string Name { get; set; }
        public int Age { get; set; }
        public bool Is211 { get; set; }
        public bool Is985 { get; set; }
    }
}
