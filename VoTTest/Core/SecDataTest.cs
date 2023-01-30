using System.Security.Cryptography;
using VoTCore.Package.SecData;

namespace VoTTest.Core
{
    public class SecDataTest
    {
        [Fact]
        public void TestAesCrypto()
        {
            Random rdm = new Random();

            RSA rsaKey = RSA.Create();
            Aes aesKey = Aes.Create();

            var sender  = rdm.NextInt64();
            var reciver = rdm.NextInt64();

            var key = aesKey.Key;
            var iv  = aesKey.IV;

            SecData_Key_Aes body = new(aesKey, sender);

            body.EncryptData(rsaKey, reciver);
            Aes aesKeyEncrypted = Aes.Create();

            try
            {
                aesKeyEncrypted = body.GetKey();

                Assert.False(Enumerable.SequenceEqual(key, aesKeyEncrypted.Key));
                Assert.False(Enumerable.SequenceEqual(iv, aesKeyEncrypted.IV));
            }catch(Exception) { }

            body.DecryptData(rsaKey);
            aesKeyEncrypted = body.GetKey();
            Assert.True(Enumerable.SequenceEqual(key, aesKeyEncrypted.Key));
            Assert.True(Enumerable.SequenceEqual(iv, aesKeyEncrypted.IV));
        }
    }
}
