using SevenTiny.Bantina.Security;
using Xunit;

namespace Test.SevenTiny.Bantina
{
    public class AesHeleprTest
    {
        private const string TestText = "123456";

        [Fact]
        public void CbcEncryptDecrypt()
        {
            var key = AesHelper.GenerateSecretKey();

            string data1 = AesHelper.CbcEncrypt(TestText, key);

            string data2 = AesHelper.CbcDecrypt(data1, key);

            Assert.Equal(TestText, data2);
        }

        [Fact]
        public void GcmEncryptDecrypt()
        {
            var key = AesHelper.GenerateSecretKey();

            string data1 = AesHelper.GcmEncrypt(TestText, key);

            string data2 = AesHelper.GcmDecrypt(data1, key);

            Assert.Equal(TestText, data2);
        }
    }
}
