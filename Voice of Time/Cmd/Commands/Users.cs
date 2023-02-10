using VoTCore.Algorithms;
using VoTCore.Controll;
using VoTCore.Package;
using VoTCore.Package.AData;
using VoTCore.Package.Header;
using VoTCore.Package.SData;
using VoTCore.Package.SecData;

namespace Voice_of_Time.Cmd.Commands
{
    internal class Users : IConsoleCommandAsync
    {
        public string Command => "users";

        private readonly string[] aliases = { "u", "user", "client", "clients" };
        public string[] Aliases => aliases;

        public string Usage => "users list";

        public async Task<bool> ExecuteCommand(string command, string[] args)
        {
            if(args.Length <= 0) return false;

            if (args[0].ToLower() == "list")
            {
                await RequestAllUser();
            }

            return false;
        }

        private async Task RequestAllUser()
        {
            List<long> userID = await RequestAllUserIDs();

            var loadingBar = new LoadingBar(userID.Count);

            var currentClient = ClientData.CurrentClient ?? throw new Exception("No activ connection!");

            loadingBar.Start();

            foreach (var user in userID)
            {
                var suc = await TryGettingUserAsync(currentClient.UserID, user);
                if(suc)
                {
                    loadingBar.Update(loadingBar.CurrentStep + 1);
                }
                else
                {
                    loadingBar.Pause();
                    Console.WriteLine($"Coudn't get User: #{Base36.Encode(user)}");
                    loadingBar.Start();
                }
            }

            loadingBar.Stop();

            var userList = new List<long>(currentClient.UserDB.Keys);
            userList.Sort();

            var pattern = "{0,12}|{1}";

            Console.WriteLine(pattern, "ID", "Username");
            Console.WriteLine(pattern, "------------", "------------");

            foreach (var pubClient in userList)
            {
                var client = currentClient.UserDB[pubClient];
                Console.WriteLine(pattern, Base36.Encode(client.ID), client.Username);
            }
        }

        private async Task<List<long>> RequestAllUserIDs()
        {
            var currentClient = ClientData.CurrentClient ?? throw new Exception("No activ connection!");

            var head       = new HeaderReq(ClientData.CurrentClient.UserID, RequestType.GET_USERID_LIST);
            var package    = new VOTP(head);
            var result     = await (ClientData.GetConnection(ClientData.CurrentConnection ?? throw new Exception("No activ connection")) ?? throw new Exception("No activ connection")).EnqueueItem(package.Serialize());
            var resPackage = new VOTP(result);

            if(resPackage.Header is not HeaderAck resHeader) throw new Exception("Server didn't responded correctly!");
            if(resHeader.Successful is false) throw new Exception("Server couldn't responded correctly!");

            if (resPackage.Body is not AData_Long resBody) throw new Exception("Server didn't responded correctly!");

            return new(resBody.Data);
        }

        // Dupplicat from Chat
        private static async Task<bool> TryGettingUserAsync(long senderID, long targetID)
        {
            if (ClientData.CurrentClient is null || ClientData.CurrentConnection is null) return false;

            var header = new HeaderReq(senderID, RequestType.GET_PUBLIC_USER);
            var body = new SData_Long(targetID);
            var package = new VOTP(header, body);

            var connection = ClientData.GetConnection((Guid)ClientData.CurrentConnection) ?? throw new Exception("Connection is not Registert!");

            var result = new VOTP(await connection.EnqueueItem(package.Serialize()));

            if (result.Header is not HeaderAck resHeader) throw new Exception("Server didn't responded correctly!");
            if (resHeader.Successful is false) return false;

            if (result.Body is null) return false;
            if (result.Body is not SecData_ClientShare resBody) throw new Exception("Server didn't responded correctly!");

            var UserEntry = resBody.GetPublicClient();

            return ClientData.CurrentClient.AppendOrOverridePublicClint(UserEntry);
        }
    }
}
