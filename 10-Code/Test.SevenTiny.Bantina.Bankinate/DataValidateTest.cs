using System;
using Test.SevenTiny.Bantina.Bankinate.Model;
using Xunit;

namespace Test.SevenTiny.Bantina.Bankinate
{
    [Trait("desc", "测试需要先打开校验属性值的开关")]
    [Trait("desc", "当前标签的限制为1-10")]
    public class DataValidateTest
    {
        public MySqlPropertyValidateDb Db => new MySqlPropertyValidateDb() { };

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(11)]
        [InlineData(999)]
        public void ValidateTest1(int value)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                Db.Add(new PropertyVerifyTestModel_Int_1_10
                {
                    Key = value,
                });
            });
        }

        [Theory]
        [InlineData(11)]
        [InlineData(999)]
        public void ValidateTest2(int value)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                Db.Add(new PropertyVerifyTestModel_Int_Max_10
                {
                    Key = value
                });
            });
        }

        [Theory]
        [InlineData(null)]
        public void ValidateTest3(int? value)
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                Db.Add(new PropertyVerifyTestModel_Int_Require
                {
                    Key = value
                });
            });
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public void ValidateTest4(float value)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                Db.Add(new PropertyVerifyTestModel_Float_Min_1
                {
                    Key = value
                });
            });
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(11)]
        [InlineData(999)]
        public void ValidateTest5(double value)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                Db.Add(new PropertyVerifyTestModel_Double_1_10
                {
                    Key = value
                });
            });
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(11)]
        [InlineData(999)]
        public void ValidateTest6(decimal value)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                Db.Add(new PropertyVerifyTestModel_Decimal_1_10
                {
                    Key = value
                });
            });
        }

        [Theory]
        [InlineData("12345678901111")]
        public void ValidateTest7(string value)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                Db.Add(new PropertyVerifyTestModel_String_Max_10
                {
                    Key = value
                });
            });
        }

        [Theory]
        [InlineData("")]
        [InlineData("12345678901111")]
        public void ValidateTest8(string value)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                Db.Add(new PropertyVerifyTestModel_String_1_10
                {
                    Key = value
                });
            });
        }

        [Theory]
        [InlineData(null)]
        public void ValidateTest9(string value)
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                Db.Add(new PropertyVerifyTestModel_String_Require
                {
                    Key = value
                });
            });
        }

        [Fact]
        public void ValidateTest10()
        {
            Assert.Throws<InvalidCastException>(() =>
            {
                Db.Add(new PropertyVerifyTestModel_DateTime_1_10
                {
                    Key = DateTime.Now
                });
            });
        }
    }
}
