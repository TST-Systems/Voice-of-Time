using System.Collections.Generic;
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
using VoTCore.Algorithms;
using VoTCore;
using VoTCore.Communication;
using System.Text;
using VoTCore.Communication.Data;
using VoTCore.Package.Header;
using VoTCore.Package;
using VoTCore.Secure;

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
                String username = argsString.GetValue("UserName").ToString();
                String ip = argsString.GetValue("serverIP").ToString(); ; 

                int port = 15050;

                await SystemCalls.Connect(ip, port, username);

				var mainWindow = Electron.WindowManager.BrowserWindows.First();
                var connection = ClientData.CurrentConnection;
                Electron.IpcMain.Send(mainWindow, "createServer-reply", connection);
            });

            
            Electron.IpcMain.On("connectServer", async (ip) =>
            {
                if (ip is not String ipString) throw new Exception();
                await SystemCalls.Connect(ipString);

                var mainWindow = Electron.WindowManager.BrowserWindows.First();
                Electron.IpcMain.Send(mainWindow, "connectServer-reply", ClientData.CurrentConnection);
            });

            Electron.IpcMain.On("listChats", async (args) =>
            {
                List<PrivatChat> chats = await SystemCalls.ListChats();
                chats.Sort((t1, t2) => { return string.Compare(t1.Title, t2.Title); });

                
                List<(String ChatID, String ChatName)> values = new ();
                foreach (var chat in chats)
                {
                    values.Add((chat.ChatID.ToString(),  chat.Title.ToString()));
                }

                var mainWindow = Electron.WindowManager.BrowserWindows.First();
                Electron.IpcMain.Send(mainWindow, "listChats-replyIDs", values);

                
            });

            Electron.IpcMain.On("readChat", async (ID) =>
            {
                long chatID = long.Parse(ID.ToString());
                List<Message> messages = new List<Message>();
                messages = await SystemCalls.ReadMessanges(chatID);


                List<(String userName, String dateOfCraetion, String messageContent)> messageMeta = new ();
                foreach(var message in messages)
                {
                    String userName = ClientData.CurrentClient.UserDB[message.AuthorID].ToString();
                    String dateOfCreation = message.DateOfCreation.ToString();
                    String messageContent = message.MessageString;

                    if (userName != null) messageMeta.Add((userName, dateOfCreation, messageContent));
                }

                var mainWindow = Electron.WindowManager.BrowserWindows.First();
                Electron.IpcMain.Send(mainWindow, "readChat-replyI", messageMeta);
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

                    //Set Username
                    await Requests.SetUsername(socket, username, userID);

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
                //Console.WriteLine();
                //Console.WriteLine("An Error ocured!");
                //Console.WriteLine(ex.Message);
                return false; 
            }

            ClientData.SaveData();

            return true;
        }

        public static async Task<List<PrivatChat>> ListChats()
        {
            List <PrivatChat> listOfChats = new List<PrivatChat>();
            if (ClientData.CurrentConnection is null || ClientData.CurrentClient is null)
            {
                return listOfChats;
            }

            var currentClientInstace = ClientData.CurrentClient;

            if (currentClientInstace is null)
            {
                return listOfChats;
            }

            foreach (var chat in currentClientInstace.TextChats)
            {
                if (chat is PrivatChat pC)
                {
                    listOfChats.Add(pC);
                }
                else
                {
                   //Channel coming Soon  
                }
            }

            return listOfChats;
        }

        public static async Task<bool> CreateChatAsync(List<long> particepents, String chatname)
        {
            if (ClientData.CurrentConnection is null)
            {
                //Console.WriteLine("You are currently on no Server!");
                return true;
            }

            var (socket, client, _) = ClientData.GetCurrentConnection();

            // Sort the List and remove multiple entraces
            particepents.Sort();
            particepents = particepents.Distinct().ToList();

            // Check if users exist when not try to find out if user exists
            foreach (var particepent in particepents)
            {
                if (client.UserDB.ContainsKey(particepent)) continue;

                if (await Requests.TryGettingUserAsync(socket, client, particepent)) continue;

                particepents.Remove(particepent);
            }

            // Get ChatID from Server and Register it 
            var chatID = await Requests.GetChatID(socket, client.UserID);


            if (particepents.Count <= 3)
            {
                for (int j = 0; j < particepents.Count; j++)
                {
                    chatname += client.UserDB[particepents[j]].Username;
                    if (j + 1 < particepents.Count)
                    {
                        chatname += ", ";
                    }
                }
            }
            else
            {
                if (chatname is null or "") chatname = "Group";
                chatname = chatname.Trim();
            }

            var chat = new PrivatChat(chatID, chatname, particepents);

            client.TextChats.Add(chat);

            // Invite all remaining users
            foreach (var particepen in particepents)
            {
                var pubClient = client.UserDB[particepen];

                if (pubClient is null) throw new Exception("Public user was deletet while processing!");

                await Requests.InviteUserToGroupAsync(socket, client, pubClient, new(chat), DataHandling.REMOVE_AFTER_GET_ACK);
            }

            return true;
        }

        public static async Task<bool> WriteMessage(long chatID, string messageContent)
        {
            if (ClientData.CurrentConnection is null || ClientData.CurrentClient is null)
            {
                return true;
            }
            ClientSocket currentConnection = ClientData.GetConnection((Guid)ClientData.CurrentConnection) ?? throw new Exception();
            if (messageContent == "") return false;

            var listOfChats = ClientData.CurrentClient.TextChats.Where((TxtCht) => { if (TxtCht is not PrivatChat pC) return false; return pC.ChatID == chatID; }).ToArray();

            if (listOfChats.Length != 1) return false;
            
            PrivatChat chat = (PrivatChat) listOfChats[0];

            var header = new HeaderStd(ClientData.CurrentClient.UserID, chat.ChatID, 1);
            var message = new TextMessage(messageContent, ClientData.CurrentClient.UserID, DateTime.Now);
            var toStash = new VOTP(header, message).Serialize();
            toStash = Convert.ToBase64String(CryproManager.AesEncyrpt(chat.Key, Encoding.UTF8.GetBytes(toStash)));

            var Receipt = await Requests.AddStashMessage(currentConnection, ClientData.CurrentClient, chat.ChatID, toStash);
            

            ClientData.CurrentClient.ReceiptStatusDictionary[Receipt] = ReceiptStatus.IGNORE;

            chat.AddMessage(message);

            return true;
        }

        public static async Task<List<Message>> ReadMessanges(long chatID)
        {
            if (ClientData.CurrentConnection is null || ClientData.CurrentClient is null)
            {
                return null;
            }

            var listOfChats = ClientData.CurrentClient.TextChats.Where((TxtCht) => { if (TxtCht is not PrivatChat pC) return false; return pC.ChatID == chatID; }).ToArray();

            if (listOfChats.Length != 1) return null;

            PrivatChat chat = (PrivatChat)listOfChats[0];

            List<Message> list = new List<Message>();

            list = chat.GetMessages();
            
            return list;
        }

    }
}

