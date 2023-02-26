using System.Text.Json.Serialization;
using VoTCore.Package.Interfaces;

/**
 * @author      - Timeplex
 * 
 * @created     - 10.02.2023
 * 
 * @last_change - 10.02.2023
 */
namespace VoTCore.Package.AData
{
    /// <summary>
    /// Base class for all Array data bodys
    /// </summary>
    /// <typeparam name="T">Type of array data</typeparam>
    public abstract class AData<T> : IVOTPBody
    {
        [JsonIgnore]
        public abstract BodyType Type { get; }

        /// <summary>
        /// Array of data
        /// </summary>
        public T[] Data { get; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="data">Array of data</param>
        public AData(T[] data) 
        { 
            Data = data;
        }

        /// <summary>
        /// Import a List of data
        /// </summary>
        /// <param name="data">List of data</param>
        public AData(List<T> data)
        {
            Data = data.ToArray();
        }
    }
}
