using SevenTiny.Bantina;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Test.SevenTiny.Bantina
{
    public class ResultTest
    {
        [Fact]
        public void TipType_Success()
        {
            Assert.Equal(TipType.Success, Result.Success().TipType);
        }

        [Fact]
        public void TipType_Warning()
        {
            Assert.Equal(TipType.Warning, Result.Warning().TipType);
        }

        [Fact]
        public void TipType_Info()
        {
            Assert.Equal(TipType.Info, Result.Info().TipType);
        }

        [Fact]
        public void TipType_Error()
        {
            Assert.Equal(TipType.Error, Result.Error().TipType);
        }

        [Fact]
        public void Result_Message()
        {
            string message = "testMessage";
            Assert.Equal(message, Result.Success(message).Message);
        }

        [Fact]
        public void Result_Success()
        {
            Assert.True(Result.Success().IsSuccess);
        }

        [Fact]
        public void Result_Continue()
        {
            string message = "testMessage";
            var result = Result.Success().Continue(re =>
            {
                return Result.Success(message);
            });
            Assert.True(result.IsSuccess);
            Assert.Equal(message, result.Message);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Result_ContinueAssert(bool assert)
        {
            string message = "testMessage";
            var result = Result.Success().ContinueAssert(r => assert, message);
            Assert.Equal(assert, result.IsSuccess);
            if (!assert)
                Assert.Equal(message, result.Message);
        }

        [Theory]
        [InlineData(TipType.Success)]
        [InlineData(TipType.Warning)]
        [InlineData(TipType.Info)]
        [InlineData(TipType.Error)]
        public void Result_Continue_TipType(TipType tipType)
        {
            string message = "testMessage";
            var result = Result.Success().Continue(re =>
            {
                switch (tipType)
                {
                    case TipType.Success:
                        return Result.Success(message);
                    case TipType.Warning:
                        return Result.Warning(message);
                    case TipType.Info:
                        return Result.Info(message);
                    case TipType.Error:
                        return Result.Error(message);
                    default:
                        return Result.Success(message);
                }
            });
            Assert.Equal(tipType, result.TipType);
            Assert.Equal(message, result.Message);
        }

        [Fact]
        public void Result_ContinueTryCatch()
        {
            var errorMessage = "参数不能为空";

            var result = Result.Success()
                .ContinueWithTryCatch(_ =>
                {
                    throw new ArgumentNullException(errorMessage);
                }, null, errorMessage);

            Assert.Equal(errorMessage, result.Message);
        }

        [Fact]
        public void Result_ContinueEnsureArgumentNotNullOrEmpty()
        {
            var result = Result.Success()
                .ContinueEnsureArgumentNotNullOrEmpty(null, "age");

            Assert.Equal("Parameter cannot be null or empty. Parameter name: age", result.Message);
        }
    }
}
