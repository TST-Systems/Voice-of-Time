using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text.Json.Nodes;
using ElectronNET.API;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Server.IIS.Core;
using Newtonsoft.Json.Linq;
using Voice_of_Time.Cmd.Commands;
using Voice_of_Time.Shared;
using Voice_of_Time.Shared.Functions;
using Voice_of_Time.Transfer;
using Voice_of_Time.User;

namespace Test_GUI_VoT.Pages
{
	public class IndexModel : PageModel
	{
		private readonly ILogger<IndexModel> _logger;


		public IndexModel(ILogger<IndexModel> logger)
		{
			_logger = logger;
		}

		public void OnGet()
		{
            Electron.IpcMain.On("createServer", async (args) =>
            {
				if (args is not JObject argsString) throw new Exception();  
				var connect = new Connect();
                String username = argsString.GetValue("serverName").ToString();
                String ip = argsString.GetValue("serverIP").ToString(); ; 

                int port = 15050;

                await SystemCalls.Connect(ip, port, username);

				var mainWindow = Electron.WindowManager.BrowserWindows.First();
                Electron.IpcMain.Send(mainWindow, "createServer-reply", ClientData.CurrentConnection);
            });

            
        }
	}

	public static partial class SystemCalls
	{
		public static async Task<bool> Connect(String ip, int port = 15050, String username = "")
		{
            try
            {
                // Connection to server
                ClientSocket socket = new(ip, port);
                socket.StartHandler();

                // Getting server Identity
                Guid serverID = await Requests.RequestServerID(socket);

                //Checking if a connection is already open
                var connectionOpen = ClientData.GetConnection(serverID);
                if (connectionOpen != null)
                {
                    socket.Dispose();
                    return false;
                }

                // Checking if server is known
                Client? client = ClientData.GetServerCient(serverID);
                if (client == null) // If not Register
                {
                    // Generate a new RSA key pair;
                    var clientKey = RSA.Create();

                    // Execute a key exchange
                    var serverKey = await Requests.KeyExchangeWithServer(socket, clientKey);

                    // Open a Secure communication
                    await Requests.OpenSecureCommunication(socket, clientKey);

                    // Register User on Server
                    var userID = await Requests.RegisterClient(socket);


                    // Saving Data of connection
                    var sKey = serverKey.ExportParameters(false).Modulus;

                    client = new(userID, username, clientKey);
                    client.UserDB.Add(0, new(0, "Server", serverKey));

                    ClientData.AddServerClient(client, serverID);
                }
                else // Else verify
                {
                    var userIsKnown = await Requests.ValidateSelf(socket, client);
                    if (!userIsKnown)
                    {
                        socket.Dispose();
                        return false;
                    }
                    await Requests.TestConnection(socket, serverID, client.UserID);
                }
                // Register the connection
                ClientData.AddConnection(serverID, socket);
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("An Error ocured!");
                Console.WriteLine(ex.Message);
                return false;
            }

            ClientData.SaveData();

            return true;
        }

    }
}

