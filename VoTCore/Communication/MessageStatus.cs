namespace VoTCore.Communication
{
    public class MessageStatus
    {
        private bool status_sent;
        private bool status_received;
        private bool status_failed;


        bool serverGotMessage = false;
        bool serverReceivedReceipt = false;

        public bool Status_sent { get => status_sent; }
        public bool Status_received { get => status_received; }
        public bool Status_failed { get => status_failed; }


        //Staus wird der Nachricht vom Server zugewiesen
        public void AssignStatusSent()
        {
            if (serverGotMessage == true)
            {
                status_sent = true;
            }
            else
            {
                status_sent = false;
                Console.WriteLine("Message couldn't be sent.");
            }

        }


        public void AssignStatusReceived()
        {
            if (serverReceivedReceipt == true)
            {
                status_received = true;

            }
            else
            {
                status_received = false;
                Console.WriteLine("Message wasn't received.");
            }

        }


        public void AssignStatusFailed()
        {
            if (status_sent == false && status_received == false)
            {

                status_failed = true;
                Console.WriteLine("Failed.");
            }

        }

    }
}