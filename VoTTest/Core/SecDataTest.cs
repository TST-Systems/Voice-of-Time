using System.Security.Cryptography;
using System.Text.Json;
using VoTCore.Package.SecData;
using VoTCore.Secure;
using VoTCore.User;

/**
 * @author      - Timeplex
 * 
 * @created     - 21.01.2023
 * 
 * @last_change - 09.02.2023
 */
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

        [Fact]
        public void TestClientShare()
        {
            var PublicKey    = new PublicRSA();
            var PublicClient = new PublicClient(12345, "Someone", PublicKey);
            var ShareBody    = new SecData_ClientShare(PublicClient);

            var serialized   = JsonSerializer.Serialize(ShareBody);
            var deserialized = JsonSerializer.Deserialize<SecData_ClientShare>(serialized);

            Assert.Equal(PublicClient.Username, deserialized.Username);
            Assert.Equal(PublicClient.ID,       deserialized.SourceID);
        }
    }
}
