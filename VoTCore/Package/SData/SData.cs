using System.Text.Json.Serialization;
using VoTCore.Package.Interfaces;

/**
 * @author      - Timeplex
 * 
 * @created     - 14.01.2023
 * 
 * @last_change - 20.01.2023
 */
namespace VoTCore.Package.SData
{
    /// <summary>
    /// Base class for single data bodys
    /// </summary>
    /// <typeparam name="T">type of data</typeparam>
    public abstract class SData<T> : IVOTPBody
    {
        /// <summary>
        /// Data
        /// </summary>
        public T? Data { get; }
    
        [JsonIgnore]
        public BodyType Type { get; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="type">Type of body</param>
        protected SData(T? data, BodyType type)
        {
            Data = data;
            Type = type;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (obj is not SData<T> their) return false;

            if (Type != their.Type) return false;

            if (Data == null && their.Data == null) return true;

            if (Data != null && their.Data != null)
                if(Data.Equals(their.Data)) return true;

            return false;
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }
}
