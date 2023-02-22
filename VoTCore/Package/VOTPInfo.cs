using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

/**
 * @author      - Timeplex
 * 
 * @created     - 11.12.2022
 * 
 * @last_change - 12.02.2023
 */
namespace VoTCore.Package
{
    /// <summary>
    /// Pre-header for VOTP for inforamtions about Types of header and body for deseriliasation
    /// </summary>
    public class VOTPInfo
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="package"></param>
        public VOTPInfo(VOTP package)
        {
            Version = package.Header.Version;
            if (package.Body != null) Type = package.Body.Type;
            else                      Type = BodyType.NONE;
            PackageID = package.PackageID;
        }

        /// <summary>
        /// JSON constructor
        /// </summary>
        /// <param name="version"></param>
        /// <param name="type"></param>
        /// <param name="packageID"></param>
        [JsonConstructor]
        public VOTPInfo(short version, BodyType type, long packageID)
        {
            Version   = version;
            Type      = type;
            PackageID = packageID;
        }

        /// <summary>
        /// Constructor for only getting the VOTPinfo for peeking what types are inside the package
        /// </summary>
        /// <param name="json">The intier json string of the package</param>
        /// <exception cref="SerializationException"></exception>
        public VOTPInfo(string json)
        {
            var split = json.Split('\0');
            if (split.Length > 3 || split.Length < 2) throw new SerializationException("String is not a VOTP!");

            var copy = JsonSerializer.Deserialize<VOTPInfo>(split[0]);
            if (copy is null) throw new SerializationException("Package Info can not be deserialized!");

            Version   = copy.Version;
            Type      = copy.Type;
            PackageID = copy.PackageID;
        }

        public short Version { get; }

        public BodyType Type { get; }

        public long PackageID { get; }
    }
}
