using SevenTiny.Bantina.AutoMapper;

namespace Test.Model
{
    public class Grade
    {
        public object _id { get; set; }
        public int GradeId { get; set; }
        [Mapper("GradeName")]
        public string Name { get; set; }
        public string name { get; set; }
        public int age { get; set; }
    }
}
