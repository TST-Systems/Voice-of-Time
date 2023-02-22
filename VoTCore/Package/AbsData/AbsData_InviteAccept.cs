using System.Text.Json.Serialization;
using VoTCore.Package.Interfaces;

namespace VoTCore.Package.AbsData
{
    /// <summary>
    /// Abstract data for a invited to send the server for informing it that an inventation was accepted
    /// </summary>
    public class AbsData_InviteAccept : IVOTPBody
    {
        [JsonIgnore]
        public BodyType Type { get => BodyType.ABSDATA_INVITE_ACCEPT; }

        /// <summary>
        /// Targeted chat
        /// </summary>
        public long Data { get; } // TODO: Rename after debugsession!

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="data">ID of chat to join</param>
        [JsonConstructor]
        public AbsData_InviteAccept(long data)
        {
            Data = data;
        }
    }
}
