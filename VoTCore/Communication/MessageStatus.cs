public abstract class MessageStatus
{

    public bool status_sent { get; }

    public bool status_received { get; }

    public bool status_failed { get; }




    var serverGotMessage = false;
    var serverReceivedReceipt = false;

    public void assignStatusSent
    {
        if (serverGotMessage = true)
        {
            set { status_sent = true; }

        }else
{
    set { status_sent = false; };
    Console.WriteLine("Message couldn't be sent.");
}
                
    }


    public void assignStatusReceived
{
    if (serverReceivedReceipt = true)
        {
        set { status_received = true; }

    }else
{
        set { status_received = false; };
        Console.WriteLine("Message wasn't received.");
    }

}


    public void assignStatusFailed
{
    if (status_sent == false && status_received == false){

        set { status_failed = true; }
        Console.WriteLine("Failed.");
    }

}

}
