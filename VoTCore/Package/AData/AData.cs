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
    public abstract class AData<T> : IVOTPBody
    {
        [JsonIgnore]
        public abstract BodyType Type { get; }

        public T[] Data { get; }

        public AData(T[] data) 
        { 
            Data = data;
        }

        public AData(List<T> data)
        {
            Data = data.ToArray();
        }
    }
}
