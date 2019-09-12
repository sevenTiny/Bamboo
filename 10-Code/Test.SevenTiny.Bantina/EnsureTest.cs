using SevenTiny.Bantina.Validation;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Test.SevenTiny.Bantina
{
    public class EnsureTest
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void ArgumentNotNullOrEmpty(object data)
        {
            Assert.Throws<ArgumentNullException>(() => { Ensure.ArgumentNotNullOrEmpty(data, nameof(data)); });
        }

        [Fact]
        public void ArgumentNotNullOrEmpty2()
        {
            Assert.Throws<ArgumentNullException>(() => { Ensure.ArgumentNotNullOrEmpty(new object[] { }, null); });
        }

        [Fact]
        public void ArgumentNotNullOrEmpty3()
        {
            Assert.Throws<ArgumentNullException>(() => { Ensure.ArgumentNotNullOrEmpty(new string[] { }, null); });
        }

        [Fact]
        public void ArgumentNotNullOrEmpty4()
        {
            Assert.Throws<ArgumentNullException>(() => { Ensure.ArgumentNotNullOrEmpty(new double[] { }, null); });
        }

        [Fact]
        public void ArgumentNotNullOrEmpty5()
        {
            Assert.Throws<ArgumentNullException>(() => { Ensure.ArgumentNotNullOrEmpty(new float[] { }, null); });
        }

        [Fact]
        public void ArgumentNotNullOrEmpty6()
        {
            Assert.Throws<ArgumentNullException>(() => { Ensure.ArgumentNotNullOrEmpty(new dynamic[] { }, null); });
        }

        [Fact]
        public void ArgumentNotNullOrEmpty7()
        {
            Assert.Throws<ArgumentNullException>(() => { Ensure.ArgumentNotNullOrEmpty(new List<object> { }, null); });
        }
    }
}
