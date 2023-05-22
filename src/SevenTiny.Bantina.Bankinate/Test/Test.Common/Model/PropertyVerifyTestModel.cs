using SevenTiny.Bantina.Bankinate.Validation;
using System;

namespace Test.Common.Model
{
    public class PropertyVerifyTestModel_Int_1_10
    {
        [RangeLimit(1, 10)]
        public int Key { get; set; }
    }
    public class PropertyVerifyTestModel_Int_Max_10
    {
        [MaxLimit(10)]
        public int Key { get; set; }
    }

    public class PropertyVerifyTestModel_Int_Require
    {
        [Require]
        public int? Key { get; set; }
    }
    public class PropertyVerifyTestModel_Float_Min_1
    {
        [MinLimit(1)]
        public float Key { get; set; }
    }
    public class PropertyVerifyTestModel_Double_1_10
    {
        [RangeLimit(1, 10)]
        public double Key { get; set; }

    }
    public class PropertyVerifyTestModel_Decimal_1_10
    {
        [RangeLimit(1, 10)]
        public decimal Key { get; set; }
    }
    public class PropertyVerifyTestModel_String_Max_10
    {
        [StringLength(10)]
        public string Key { get; set; }
    }
    public class PropertyVerifyTestModel_String_1_10
    {
        [StringLength(1, 10)]
        public string Key { get; set; }
    }
    public class PropertyVerifyTestModel_String_Require
    {
        [Require]
        public string Key { get; set; }
    }
    public class PropertyVerifyTestModel_DateTime_1_10
    {
        /// <summary>
        /// DateTime使用double类型的RangeLimit会在Convert.ToDouble(value)的时候抛出Convert异常
        /// </summary>
        [RangeLimit(1, 10)]
        public DateTime Key { get; set; }
    }
}
