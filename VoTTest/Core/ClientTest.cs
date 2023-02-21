
/**
 * @author      - Timeplex
 * 
 * @created     - 22.12.2022
 * 
 * @last_change - 22.12.2022
 * 
 * @Outdated    - 01.02.2022
 */
/*
namespace VoTTest.Core
{
    public class ClientTest
    {
        [Fact]
        public void AddKeyTest()
        {
            Client c = new(101, "Schmitd", RSA.Create());

            var rsa1 = RSA.Create();
            var rsa2 = RSA.Create();
            var rsa3 = RSA.Create();

            var rsa1Pub = rsa1.ExportParameters(false);
            var rsa2Pub = rsa2.ExportParameters(false);
            var rsa3Pub = rsa3.ExportParameters(false);

            var rsaR1 = RSA.Create(rsa1Pub);
            var rsaR2 = RSA.Create(rsa2Pub);
            var rsaR3 = RSA.Create(rsa3Pub);

            Assert.Equal(KeyStatus.SELF, c.AddPublicKey(101, rsaR1));
            Assert.Equal(KeyStatus.SELF, c.AddPublicKey(101, rsaR2));
            Assert.Equal(KeyStatus.SELF, c.AddPublicKey(101, rsaR3));

            Assert.Equal(KeyStatus.ADD,       c.AddPublicKey(102, rsaR1));
            Assert.Equal(KeyStatus.OVERWRITE, c.AddPublicKey(102, rsaR2));
            Assert.Equal(KeyStatus.OVERWRITE, c.AddPublicKey(102, rsaR3));

            Assert.Equal(KeyStatus.ADD,       c.AddPublicKey(103, rsaR1));
            Assert.Equal(KeyStatus.NO_CHANGE, c.AddPublicKey(103, rsaR1));

            Assert.Equal(KeyStatus.PRIVATE_KEY_FOUND, c.AddPublicKey(104, rsa1));
        }

    }
}
*/