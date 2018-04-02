using System;
using System.Collections.Generic;
using System.Text;

namespace Test.SevenTiny.Bantina.Model.DTO
{
    public class StudentReferee
    {
        public Guid Uid { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string SchoolName { get; set; }
        public bool Is211 { get; set; }
        public bool Is985 { get; set; }
    }
}
