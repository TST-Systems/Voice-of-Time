namespace VoTCore.Communication.Data
{
    public class MessageStatus
    {
        private bool statusSent;
        private bool statusReceived;
        private bool statusFailed;

        //debug variables
        bool serverGotMessage = false;
        bool serverReceivedReceipt = false;

        public bool StatusSent { get => statusSent; }
        public bool StatusReceived { get => statusReceived; }
        public bool StatusFailed { get => statusFailed; }


        //Status is assigned to the message by the server
        public void AssignStatusSent()
        {
            if (serverGotMessage == true)
            {
                statusSent = true;
            }
            else
            {
                statusSent = false;
                Console.WriteLine("Message couldn't be sent.");
            }

        }


        public void AssignStatusReceived()
        {
            if (serverReceivedReceipt == true)
            {
                statusReceived = true;

            }
            else
            {
                statusReceived = false;
                Console.WriteLine("Message wasn't received.");
            }

        }


        public void AssignStatusFailed()
        {
            if (statusSent == false && statusReceived == false)
            {

                statusFailed = true;
                Console.WriteLine("Failed.");
            }

        }

    }
}