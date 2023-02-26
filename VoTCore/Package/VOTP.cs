using VoTCore.Package.Interfaces;
using JSON = System.Text.Json.JsonSerializer;
/**
 * @author      - Timeplex
 * 
 * @created     - 29.11.2022
 * 
 * @last_change - 12.02.2023
 */
namespace VoTCore.Package
{
    /// <summary>
    /// Modular package for sending and receiving data over the internet 
    /// </summary>
    public class VOTP
    {
        public IVOTPHeader Header { get; }

        public IVOTPBody? Body { get; }
        // ID for recognition of anserws
        public long PackageID { get; set; } = -1;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="header"></param>
        /// <param name="data"></param>
        public VOTP(IVOTPHeader header, IVOTPBody? data = null)
        {
            this.Header = header;
            this.Body = data;
        }

        /// <summary>
        /// Constructor to convert an serialized version back to normal
        /// </summary>
        /// <param name="json">Musst be an by it self generatet json String to meet the requirements {}\0{}(\0{})</param>
        /// <exception cref="ArgumentNullException">No input</exception>
        /// <exception cref="ArgumentException">Input error</exception>
        public VOTP(string? json)
        {
            if (json == null) 
                throw new ArgumentNullException(json);

            var split = json.Split('\0');

            if (split.Length < 2 && split.Length > 3) 
                throw new ArgumentException("The given json string does not meet the requirements!");

            // Try getting the Preheader of package
            VOTPInfo vOTPInfo;
            try
            {
                var _vOTPInfo = JSON.Deserialize<VOTPInfo>(split[0]);
                if (_vOTPInfo == null) throw new ArgumentException("Json does not meet the requirements");
                vOTPInfo = _vOTPInfo;
                PackageID = vOTPInfo.PackageID;
            }
            catch(Exception ex) {
                throw new ArgumentException("Preheader error:\n" + ex.Message);
            }

            // Get Header
            try
            {
                var _headerType = Constants.HeaderTypes[vOTPInfo.Version];
                if (_headerType == null) throw new ArgumentException("Header version unknown!");
                Type headerType = _headerType;
                var _header = (IVOTPHeader?)JSON.Deserialize(split[1], headerType);
                if (_header == null) throw new ArgumentException("Header can not be converted!");
                Header = _header;
            }
            catch(Exception ex)
            {
                throw new ArgumentException("Heaer error:\n" + ex.Message);
            }

            //Get Body if exists
            if (split.Length == 2) return;
            try
            {
                var _bodyType = Constants.BodyTypes[vOTPInfo.Type];
                if (_bodyType == null) throw new ArgumentException("Body type unknown!");
                Type bodyType = _bodyType;
                var _body = (IVOTPBody?)JSON.Deserialize(split[2], bodyType);
                if (_body == null) throw new ArgumentException("Body can not be converted!");
                Body = _body;
            }
            catch(Exception ex)
            {
                throw new ArgumentException("Body error:\n" + ex.Message);
            }
        }

        /// <summary>
        /// Serialize the Package by serialize each part by its own and combining them later
        /// </summary>
        /// <returns>JSON string that represents the package</returns>
        public string Serialize()
        {
            string serialized = string.Empty;
            var packageInfo = new VOTPInfo(this);

            serialized += JSON.Serialize(packageInfo);
            serialized += (char)0;
            serialized += JSON.Serialize(Convert.ChangeType(Header, Header.GetType()));
            if(Body != null)
            {
                serialized += (char)0;
                serialized += JSON.Serialize(Convert.ChangeType(Body, Body.GetType()));
            }
            return serialized;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;

            if (obj is not VOTP their) return false;

            if (!this.Header.Equals(their.Header)) return false;
            if (this.Body == null) 
                if (their.Body == null) return true; 
                else                    return false;
            if (!this.Body.Equals(their.Body))     return false;

            return true;
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }
}
