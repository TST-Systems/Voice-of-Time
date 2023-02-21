using System.Text.Json;
using VoTCore.Package.StashData;

namespace VoTTest.Core
{
    public class StashDataTest
    {
        [Fact]
        public void TestStashAdd()
        {
            var message = "Hallo Welt";
            var target = 0;
            var expires = DateTime.Now;

            var body = new StashData_Add(message, target, expires);

            var serialized  = JsonSerializer.Serialize(body);
            var deserilized = JsonSerializer.Deserialize<StashData_Add>(serialized);

            Assert.Equal(message, deserilized.Data.Message);
            Assert.Equal(target, deserilized.Data.TargetID);
            Assert.Equal(expires, deserilized.Data.Expires);
        }
    }
}
