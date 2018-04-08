using System;
using System.Collections.Generic;
using System.Text;

namespace Test.SevenTiny.Bantina.Model
{
    public class Student
    {
        public Guid Uid { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
<<<<<<< Updated upstream
=======
        public int BodyHigh { get; set; }
        public int HealthLevel { get; set; }

        public string GetName()
        {
            return this.Name;
        }
>>>>>>> Stashed changes
    }
}
