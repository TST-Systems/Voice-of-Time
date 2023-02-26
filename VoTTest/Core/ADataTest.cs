using VoTCore.Package.AData;

using JS = System.Text.Json.JsonSerializer;

/**
 * @author      - Timeplex
 * 
 * @created     - 10.02.2023
 * 
 * @last_change - 10.02.2023
 */
namespace VoTTest.Core
{
    /// <summary>
    /// Test for AData Bodies
    /// </summary>
    public class ADataTest
    {
        /// <summary>
        /// Test of AData with long as Type
        /// - Serialize & Deserialize
        /// </summary>
        [Fact]
        public void TestAD_Long()
        {
            Random rdm  = new Random();
            // Empty Array
            var case1   = new AData_Long(Array.Empty<long>());

            var result1 = JS.Deserialize<AData_Long>(JS.Serialize<AData_Long>(case1));

            Assert.True(case1.Data.SequenceEqual(result1.Data));

            // One Element
            var case2 = new AData_Long(new long[] {rdm.NextInt64()});

            var result2 = JS.Deserialize<AData_Long>(JS.Serialize<AData_Long>(case2));

            Assert.True(case2.Data.SequenceEqual(result2.Data));

            // Random amount of ELements
            var amountToGenerate = rdm.Next(10, 101);
            var case3 = new AData_Long(new long[amountToGenerate]);
            for(int i = 0; i < case3.Data.Length; i++) 
            {
                case3.Data[i] = rdm.NextInt64();
            }

            var result3 = JS.Deserialize<AData_Long>(JS.Serialize<AData_Long>(case3));

            Assert.True(case3.Data.SequenceEqual(result3.Data));
        }
    }
}
