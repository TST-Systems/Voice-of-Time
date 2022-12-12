using System.Text.Json;
using VoTCore.Communication;
using VoTCore.Package;

namespace VoTTest.Core
{
    public class VOTPTest
    {
        readonly Random rnd = new();

        [Fact]
        public void Serialize_Normal_Test()
        {
            /// GENERATE PACKAGE
            var header = new VOTPHeaderV1(rnd.Next(), rnd.Next(), (byte)rnd.Next(), (byte)rnd.Next());
            var body = new TextMessage((short)rnd.Next(), "Hello World", rnd.NextInt64(), rnd.NextInt64());

            var package = new VOTP(header, body);
            ///

            /// Execute function
            var serialized = package.Serialize();
            ///

            /// Test results
            // Are their 3 parts (Preheader, header, body)
            var split = serialized.Split('\0');
            Assert.Equal(3, split.Length);

            // Is the Preheader ok?
            var info = JsonSerializer.Deserialize<VOTPInfo?>(split[0]);
            Assert.NotNull(info);
            Assert.Equal(body.Type,      info.Type);
            Assert.Equal(header.Version, info.Version);

            // Is the Heaer ok?
            var json_Header = JsonSerializer.Deserialize<VOTPHeaderV1>(split[1]);
            Assert.NotNull(json_Header);
            Assert.Equal(json_Header, header);

            // Is the Body ok?
            var json_Body = JsonSerializer.Deserialize<TextMessage>(split[2]);
            Assert.NotNull(json_Body);
            // TODO: Implement Equality check
            // Assert.Equal(json_Body, body);
            Assert.Equal(body.AuthorID, json_Body.AuthorID);

        }

        [Fact]
        public void Serialize_BodyNull_Test()
        {
            /// GENERATE PACKAGE
            var header = new VOTPHeaderV1(rnd.Next(), rnd.Next(), (byte)rnd.Next(), (byte)rnd.Next());

            var package = new VOTP(header, default);
            ///

            /// Execute function
            var serialized = package.Serialize();
            ///

            /// Test results
            // Are their 2 parts (Preheader, header)
            var split = serialized.Split('\0');
            Assert.Equal(2, split.Length);

            // Is the Preheader ok?
            var info = JsonSerializer.Deserialize<VOTPInfo?>(split[0]);
            Assert.NotNull(info);
            Assert.Equal(default,        info.Type);
            Assert.Equal(header.Version, info.Version);

            // Is the Heaer ok?
            var json_Header = JsonSerializer.Deserialize<VOTPHeaderV1>(split[1]);
            Assert.NotNull(json_Header);
            Assert.Equal(json_Header, header);
        }

        [Fact]
        public void Deserialize_Normal_Test()
        {
            /// GENERATE PACKAGE
            var header = new VOTPHeaderV1(rnd.Next(), rnd.Next(), (byte)rnd.Next(), (byte)rnd.Next());
            var body = new TextMessage((short)rnd.Next(), "Hello World", rnd.NextInt64(), rnd.NextInt64());

            var package = new VOTP(header, body);
            ///

            /// Execute function
            var serialized = package.Serialize();
            var deserialized = new VOTP(serialized);
            ///

            /// Test results
            Assert.NotNull(deserialized);
            Assert.NotNull(deserialized.Data);
            Assert.NotNull(deserialized.Header);

            Assert.Equal(deserialized,        package);
            Assert.Equal(deserialized.Data,   body);
            Assert.Equal(deserialized.Header, header);
        }

        [Fact]
        public void Deserialize_BodyNull_Test()
        {
            /// GENERATE PACKAGE
            var header = new VOTPHeaderV1(rnd.Next(), rnd.Next(), (byte)rnd.Next(), (byte)rnd.Next());

            var package = new VOTP(header, default);
            ///

            /// Execute function
            var serialized = package.Serialize();
            var deserialized = new VOTP(serialized);
            ///

            /// Test results
            Assert.NotNull(deserialized);
            Assert.Null(deserialized.Data);
            Assert.NotNull(deserialized.Header);

            Assert.Equal(deserialized, package);
            Assert.Equal(default, deserialized.Data);
            Assert.Equal(deserialized.Header, header);
        }


    }
}
