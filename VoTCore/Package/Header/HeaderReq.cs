﻿using System.Text.Json.Serialization;
using VoTCore.Package.Interfaces;

namespace VoTCore.Package.Header
{
    /// <summary>
    /// Header for user -> server communication for requesting keys, regestration and other usecases
    /// </summary>
    public class HeaderReq : IVOTPHeader
    {
        public HeaderReq(long senderID, RequestType request, long? targetID = null)
        {
            SenderID = senderID;
            Request  = request;
            TargetID = targetID;
        }

        [JsonIgnore]
        public short Version { get; } = 2;

        public long  SenderID { get; }

        public long? TargetID { get; }

        public RequestType Request { get; }


        public override bool Equals(object? obj)
        {
            if (obj is null)                return false;

            if (obj is not HeaderReq their) return false;

            if (Version  != their.Version)  return false;
            if (SenderID != their.SenderID) return false;
            if (Request  != their.Request)  return false;

            return true;
        }

        public override int GetHashCode()
        {
            return (int)(Version * SenderID * ((long)Request));
        }
    }
}