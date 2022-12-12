using VoTCore.Communication;
using VoTCore.Package;

namespace VoTTest.Core
{
    public class VOTPTest
    {
        [Fact]
        public void Serialize_Normal_Test()
        {
            var header = new VOTPHeaderV1(0, 0, 0, 0);
            var body = new TextMessage(0, "Hello World", 100, 102);

            var package = new VOTP(header, body);

            var serialized = package.Serialize();

            var split = serialized.Split('\0');

            Assert.True(split.Length == 3);
        }
    }
}
