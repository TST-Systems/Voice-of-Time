using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoTCore.Package;

public abstract class MessageStatus {

    public bool status_sent { get; }

    public bool status_received { get; }

    public bool status_failed { get; }


    //Message needs generatedHash
    //sentMessage needs to be compred to the receivedMessage
    //how do we keep them apart?

    var generatedHash = x.GetHashCode();

    if (generatedHash.Equals(receivedHash))
        return true;

    if (Message.Equals(generatedHash))
        return status_sent = true;



}
