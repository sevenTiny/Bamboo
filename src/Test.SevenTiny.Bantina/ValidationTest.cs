using SevenTiny.Bantina.Validation;
using Xunit;

namespace Test.SevenTiny.Bantina
{
    public class ValidationTest
    {
        [Theory]
        [InlineData("a12345")]
        [InlineData("12345")]
        [InlineData("1")]
        [InlineData("a")]
        public void Alnum(string data)
        {
            Assert.False(ParameterValidationHelper.IsAlnum(data, 2, 5));
        }
    }
}
