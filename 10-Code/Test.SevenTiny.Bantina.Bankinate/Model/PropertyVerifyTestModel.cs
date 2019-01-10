using SevenTiny.Bantina.Bankinate.Validation;
using System;

namespace Test.SevenTiny.Bantina.Bankinate.Model
{
    public class PropertyVerifyTestModel
    {
        public int IntKey { get; set; }
        public int InkKey2 { get; set; }
        public float FloatKey { get; set; }
        public double DoubleKey { get; set; }
        public decimal DecimalKey { get; set; }
        [StringLength(4)]
        public string StringKey1 { get; set; }
        public string StringKey2 { get; set; }
        public string StringKey3 { get; set; }
        public DateTime DateTimeKey1 { get; set; }
        public DateTime DateTimeKey2 { get; set; }
        public DateTime DateTimeKey3 { get; set; }
    }
}
