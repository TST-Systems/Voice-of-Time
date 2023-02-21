
using System.Runtime.Serialization;
/**
* @author      - SalzstangeManga, Timeplex
* 
* @created     - 23.01.2023
* 
* @last_change - 03.02.2023
*/
namespace VoTCore.Communication.Data
{
    [DataContract]
    public class MessageStatus
    {
        [DataMember]
        protected bool statusSent;
        [DataMember]
        protected bool statusReceived;
        [DataMember]
        protected bool statusFailed;

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