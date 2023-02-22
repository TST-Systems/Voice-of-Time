using VoTCore.Algorithms;

/**
 * @author      - Timeplex
 * 
 * @created     - 01.02.2023
 * 
 * @last_change - 01.02.2023
 */
namespace VoTTest.Core
{
    /// <summary>
    /// Test of the Base36 algorithm
    /// </summary>
    public class Base36Test
    {
        [Fact]
        public void TestCoding()
        {
            // Test upper limit
            var longMax = long.MaxValue;
            var longMaxEncoded = Base36.Encode(longMax);
            var longMaxDecoded = Base36.Decode(longMaxEncoded);

            Assert.Equal(longMax, longMaxDecoded);

            // Test lower limit
            var longMin = long.MinValue;
            var longMinEncoded = Base36.Encode(longMin);
            var longMinDecoded = Base36.Decode(longMinEncoded);

            Assert.Equal(0, longMinDecoded); // <- Doc: Base36 Min=0 => X < 0 --> 0

            // Test Zero
            var Zero = 0;
            var ZeroEncoded = Base36.Encode(Zero);
            var ZeroDecoded = Base36.Decode(ZeroEncoded);

            Assert.Equal(Zero, ZeroDecoded);

            // Testing some random values
            Random rdm = new();
            foreach (int i in Enumerable.Range(0,100))
            {
                var expected = rdm.NextInt64();
                var actual   = Base36.Decode(Base36.Encode(expected));

                if (expected >= 0)
                {
                    Assert.Equal(expected, actual);
                    continue;
                }

                Assert.Equal(0, actual);
            }
        }

        [Fact]
        public void TestFilling() 
        {
            // 0    => 0   |  0=   |  0==  |  0===  |
            // 35   => Z   |  Z=   |  Z==  |  Z===  |
            // 36   => 10  |  10   |  10=  |  10==  |
            // 1295 => ZZ  |  ZZ   |  ZZ=  |  ZZ==  |
            // 1296 => 100 |  100  |  100  |  100=  |

            var v0   = 0;
            var vZ   = 35;
            var v00  = 36;
            var vZZ  = 1295;
            var v000 = 1296;

            Assert.Equal("0",    Base36.Encode(v0));
            Assert.Equal("Z",    Base36.Encode(vZ));
            Assert.Equal("10",   Base36.Encode(v00));
            Assert.Equal("ZZ",   Base36.Encode(vZZ));
            Assert.Equal("100",  Base36.Encode(v000));
                                 
            Assert.Equal("0",    Base36.Encode(v0,   0));
            Assert.Equal("Z",    Base36.Encode(vZ,   0));
            Assert.Equal("10",   Base36.Encode(v00,  0));
            Assert.Equal("ZZ",   Base36.Encode(vZZ,  0));
            Assert.Equal("100",  Base36.Encode(v000, 0));
                                 
            Assert.Equal("0",    Base36.Encode(v0,   1));
            Assert.Equal("Z",    Base36.Encode(vZ,   1));
            Assert.Equal("10",   Base36.Encode(v00,  1));
            Assert.Equal("ZZ",   Base36.Encode(vZZ,  1));
            Assert.Equal("100",  Base36.Encode(v000, 1));
                                 
            Assert.Equal("0=",   Base36.Encode(v0,   2));
            Assert.Equal("Z=",   Base36.Encode(vZ,   2));
            Assert.Equal("10",   Base36.Encode(v00,  2));
            Assert.Equal("ZZ",   Base36.Encode(vZZ,  2));
            Assert.Equal("100",  Base36.Encode(v000, 2));
                                 
            Assert.Equal("0==",  Base36.Encode(v0,   3));
            Assert.Equal("Z==",  Base36.Encode(vZ,   3));
            Assert.Equal("10=",  Base36.Encode(v00,  3));
            Assert.Equal("ZZ=",  Base36.Encode(vZZ,  3));
            Assert.Equal("100",  Base36.Encode(v000, 3));

            Assert.Equal("0===", Base36.Encode(v0,   4));
            Assert.Equal("Z===", Base36.Encode(vZ,   4));
            Assert.Equal("10==", Base36.Encode(v00,  4));
            Assert.Equal("ZZ==", Base36.Encode(vZZ,  4));
            Assert.Equal("100=", Base36.Encode(v000, 4));
        }
    }
}
