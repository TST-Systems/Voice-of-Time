using System.Security.Cryptography;
using System.Text.Json;
using VoTCore.Package;
using VoTCore.Package.Header;
using VoTCore.Package.SecData;
using VoTCore.Secure;
using VoTCore.User;

/**
 * @author      - Timeplex
 * 
 * @created     - 21.01.2023
 * 
 * @last_change - 10.02.2023
 */
namespace VoTTest.Core
{
    /// <summary>
    /// Tests for bodys of type SecData
    /// </summary>
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
            Aes aesKeyEncrypted;

            try
            {
                aesKeyEncrypted = body.GetKey();

                Assert.False(Enumerable.SequenceEqual(key, aesKeyEncrypted.Key));
                Assert.False(Enumerable.SequenceEqual(iv, aesKeyEncrypted.IV));
            }
            catch (Exception) { }

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

            var serialized   = JsonSerializer.Serialize(PublicClient);
            var deserialized = JsonSerializer.Deserialize<PublicClient>(serialized);

            Assert.Equal(PublicClient.Username,           deserialized.Username);
            Assert.Equal(PublicClient.UserID,             deserialized.UserID);
            Assert.Equal(PublicClient.Key.Modulus,  deserialized.Key.Modulus);
            Assert.Equal(PublicClient.Key.Exponent, deserialized.Key.Exponent);
        }

        [Fact]
        public void TestClientShareVOTP()
        {
            var PublicKey = new PublicRSA();
            var PublicClient = new PublicClient(12345, "Someone", PublicKey);

            var header = new HeaderAck(true);
            var package = new VOTP(header, PublicClient);

            var serialized = package.Serialize();
            var deserialized = new VOTP(serialized);

            Assert.True(deserialized.Header is HeaderAck);
            Assert.True(deserialized.Body is PublicClient);
        }
    }
}
