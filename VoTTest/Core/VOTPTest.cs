using System.Text.Json;
using VoTCore.Communication;
using VoTCore.Package;
using VoTCore.Package.Header;

namespace VoTTest.Core
{
    public class VOTPTest
    {

        readonly Random rnd = new();

        [Fact]
        public void Constructo_Test()
        {
            var header = new HeaderStd(rnd.Next(), rnd.Next(), (byte)rnd.Next(), (byte)rnd.Next());
            var body = new TextMessage("Hello World", rnd.NextInt64(), rnd.NextInt64());

            var package = new VOTP(header, body);
            var packageEmptyBody = new VOTP(header, default);

            Assert.Equal(header, package.Header);
            Assert.Equal(body, package.Data);

            Assert.Equal(header, packageEmptyBody.Header);
            Assert.Equal(default, packageEmptyBody.Data);
        }
        

        [Fact]
        public void Serialize_Normal_Test()
        {
            /// GENERATE PACKAGE
            var header = new HeaderStd(rnd.Next(), rnd.Next(), (byte)rnd.Next(), (byte)rnd.Next());
            var body = new TextMessage("Hello World", rnd.NextInt64(), rnd.NextInt64());

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
            var json_Header = JsonSerializer.Deserialize<HeaderStd>(split[1]);
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
            var header = new HeaderStd(rnd.Next(), rnd.Next(), (byte)rnd.Next(), (byte)rnd.Next());

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
            var json_Header = JsonSerializer.Deserialize<HeaderStd>(split[1]);
            Assert.NotNull(json_Header);
            Assert.Equal(json_Header, header);
        }

        [Fact]
        public void Deserialize_Normal_Test()
        {
            /// GENERATE PACKAGE
            var header = new HeaderStd(rnd.Next(), rnd.Next(), (byte)rnd.Next(), (byte)rnd.Next());
            var body = new TextMessage("Hello World", rnd.NextInt64(), rnd.NextInt64());

            var package = new VOTP(header, body);
            ///

            /// Execute function
            var serialized = package.Serialize();
            Console.WriteLine(serialized);
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
            var header = new HeaderStd(rnd.Next(), rnd.Next(), (byte)rnd.Next(), (byte)rnd.Next());

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

        [Fact]
        public void Equal_Test()
        {
            /// GENERATE PACKAGE
            var header1 = new HeaderStd(rnd.Next(), rnd.Next(), (byte)rnd.Next(), (byte)rnd.Next());
            var header2 = new HeaderStd(rnd.Next(), rnd.Next(), (byte)rnd.Next(), (byte)rnd.Next());

            while (header1.Equals(header2))
            {
                header2 = new HeaderStd(rnd.Next(), rnd.Next(), (byte)rnd.Next(), (byte)rnd.Next());
            }

            var body1 = new TextMessage("Hello World", rnd.NextInt64(), rnd.NextInt64());
            var body2 = new TextMessage("Hello World", rnd.NextInt64(), rnd.NextInt64());

            while (body1.Equals(body2))
            {
                body2 = new TextMessage("Hello World", rnd.NextInt64(), rnd.NextInt64());
            }

            var package1_1 = new VOTP(header1, body1);
            var package1_2 = new VOTP(header1, body2);
            var package2_1 = new VOTP(header2, body1);
            var package2_2 = new VOTP(header2, body2);

            var packageNoBody1 = new VOTP(header1, default);
            var packageNoBody2 = new VOTP(header2, default);
            ///

            /// Test results
            // Crossover Test
            Assert.Equal   (package1_1, package1_1);
            Assert.NotEqual(package1_1, package1_2);
            Assert.NotEqual(package1_1, package2_1);
            Assert.NotEqual(package1_1, package2_2);
            Assert.NotEqual(package1_1, packageNoBody1);
            Assert.NotEqual(package1_1, packageNoBody2);

            Assert.NotEqual(package1_2, package1_1);
            Assert.Equal   (package1_2, package1_2);
            Assert.NotEqual(package1_2, package2_1);
            Assert.NotEqual(package1_2, package2_2);
            Assert.NotEqual(package1_2, packageNoBody1);
            Assert.NotEqual(package1_2, packageNoBody2);

            Assert.NotEqual(package2_1, package1_1);
            Assert.NotEqual(package2_1, package1_2);
            Assert.Equal   (package2_1, package2_1);
            Assert.NotEqual(package2_1, package2_2);
            Assert.NotEqual(package2_1, packageNoBody1);
            Assert.NotEqual(package2_1, packageNoBody2);

            Assert.NotEqual(package2_2, package1_1);
            Assert.NotEqual(package2_2, package1_2);
            Assert.NotEqual(package2_2, package2_1);
            Assert.Equal   (package2_2, package2_2);
            Assert.NotEqual(package2_2, packageNoBody1);
            Assert.NotEqual(package2_2, packageNoBody2);

            Assert.NotEqual(packageNoBody1, package1_1);
            Assert.NotEqual(packageNoBody1, package1_2);
            Assert.NotEqual(packageNoBody1, package2_1);
            Assert.NotEqual(packageNoBody1, package2_2);
            Assert.Equal   (packageNoBody1, packageNoBody1);
            Assert.NotEqual(packageNoBody1, packageNoBody2);

            Assert.NotEqual(packageNoBody2, package1_1);
            Assert.NotEqual(packageNoBody2, package1_2);
            Assert.NotEqual(packageNoBody2, package2_1);
            Assert.NotEqual(packageNoBody2, package2_2);
            Assert.NotEqual(packageNoBody2, packageNoBody1);
            Assert.Equal   (packageNoBody2, packageNoBody2);

            // Intern variable Test
            Assert.False(package1_1.Equals(header1));
            Assert.False(package1_1.Equals(header2));
            Assert.False(package1_1.Equals(body1));
            Assert.False(package1_1.Equals(body2));

            Assert.False(package1_2.Equals(header1));
            Assert.False(package1_2.Equals(header2));
            Assert.False(package1_2.Equals(body1));
            Assert.False(package1_2.Equals(body2));

            Assert.False(package2_1.Equals(header1));
            Assert.False(package2_1.Equals(header2));
            Assert.False(package2_1.Equals(body1));
            Assert.False(package2_1.Equals(body2));

            Assert.False(package2_2.Equals(header1));
            Assert.False(package2_2.Equals(header2));
            Assert.False(package2_2.Equals(body1));
            Assert.False(package2_2.Equals(body2));

            Assert.False(package2_2.Equals(header1));
            Assert.False(package2_2.Equals(header2));
            Assert.False(package2_2.Equals(body1));
            Assert.False(package2_2.Equals(body2));

            // Null Test
            Assert.False(package1_1.Equals(null));
            Assert.False(package1_2.Equals(null));
            Assert.False(package2_1.Equals(null));
            Assert.False(package2_2.Equals(null));
            Assert.False(packageNoBody1.Equals(null));
            Assert.False(packageNoBody2.Equals(null));
        }

    }
}
