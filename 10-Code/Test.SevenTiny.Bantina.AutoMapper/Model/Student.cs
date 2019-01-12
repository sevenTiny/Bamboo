namespace Test.Model
{
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
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
