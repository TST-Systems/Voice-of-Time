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
    /// <summary>
    /// Body for receiving stashed data
    /// </summary>
    public class StashData : IVOTPBody
    {
        [JsonIgnore]
        public virtual BodyType Type { get; } = BodyType.STASHDATA;
        /// <summary>
        /// Stashed data
        /// </summary>
        public StashMessage Data { get; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="data">Stashed data</param>
        public StashData(StashMessage data)
        {
            Data = data;
        }
    }
}
