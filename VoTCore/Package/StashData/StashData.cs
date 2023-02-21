using System.Text.Json.Serialization;
using VoTCore.Data;
using VoTCore.Package.Interfaces;

/**
 * @author      - Timeplex
 * 
 * @created     - 18.02.2023
 * 
 * @last_change - 18.02.2023
 */
namespace VoTCore.Package.StashData
{
    public class StashData : IVOTPBody
    {
        [JsonIgnore]
        public virtual BodyType Type { get; } = BodyType.STASHDATA;

        public StashMessage Data { get; }

        public StashData(StashMessage data)
        {
            Data = data;
        }
    }
}
