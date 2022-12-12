using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using VoTCore.Communication;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace VoTCore.Package
{
    public class VOTP
    {
        public VOTP(IVOTPHeader header, IVOTPBody? data)
        {
            this.Header = header;
            this.Data = data;
        }

        /// <summary>
        /// Function to convert an serialized version back to normal
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
                var _vOTPInfo = JsonSerializer.Deserialize<VOTPInfo>(split[0]);
                if (_vOTPInfo == null) throw new ArgumentException("Json does not meet the requirements");
                vOTPInfo = _vOTPInfo;

            }
            catch(Exception ex) {
                throw new ArgumentException("Preheader error:\n" + ex.Message);
            }

            // Get Header
            try
            {
                var _headerType = Constants.HeaderTypes[vOTPInfo.Version];
                if (_headerType == null) throw new ArgumentException("Header (version) unknown!");
                Type headerType = _headerType;
                var _header = (IVOTPHeader?)JsonConvert.DeserializeObject(split[1], headerType);
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
                if (_bodyType == null) throw new ArgumentException("Body (type) unknown!");
                Type bodyType = _bodyType;
                var _body = (IVOTPBody?)JsonConvert.DeserializeObject(split[2], bodyType);
                if (_body == null) throw new ArgumentException("Body can not be converted!");
                Data = _body;
            }
            catch(Exception ex)
            {
                throw new ArgumentException("Body error:\n" + ex.Message);
            }
        }

        public IVOTPHeader Header { get; }

        public IVOTPBody? Data { get; }

        public string Serialize()
        {
            string serialized = string.Empty;
            var packageInfo = new VOTPInfo(this);

            serialized += JsonSerializer.Serialize(packageInfo);
            serialized += (char)0;
            serialized += JsonSerializer.Serialize(Convert.ChangeType(Header, Header.GetType()));
            if(Data != null)
            {
                serialized += (char)0;
                serialized += JsonSerializer.Serialize(Convert.ChangeType(Data, Data.GetType()));
            }
            return serialized;
        }
    }
}
