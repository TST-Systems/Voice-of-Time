using Voice_of_Time.Transfer;
using VoTCore.Communication;
using VoTCore.Package;

var head = new VOTPHeaderV1(1, 1, 1, 1);
var body = new FileMessage(0, "Hello Galaxy", 1, 87549875, null);

var package = new VOTP(head, body);

var serialized = package.Serialize();

Console.WriteLine(serialized);

var deserialize = new VOTP(serialized);

Console.ReadKey();

SocketClient.SetStreamAsync("Hey There!!!");

