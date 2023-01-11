using System.Security.Cryptography;
using Voice_of_Time;
using Voice_of_Time.Transfer;
using Voice_of_Time_Server.Transfer;
using VoTCore;
using VoTCore.Secure;

namespace VoTTest.Core
{
    public class ConnectionTest
    {
        [Fact]
        public async void EchoTest()
        {
            // Define Server and Client
            Func<string, string> echoFunction = new((input) => input);
            var server = new SocketServer(15050, echoFunction);
            var client = new CSocketHold("localhost", 15050);
            // Create Message to Send
            var messageToSend = "Hello World!";
            if(Constants.BUFFER_SIZE_BYTE < messageToSend.Length) Assert.Fail("Buffer for Test to small!");
            // Init Server and Client
            _ = server.StartListining();
            Assert.True(await client.AutoStart());
            // Send Message
            var echoMessage = await client.EnqueueItem(messageToSend);
            Assert.Equal(messageToSend, echoMessage);
        }

    }
}
