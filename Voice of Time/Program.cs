using System.Runtime.Serialization;
using System.Text.Json;
using VoTCore.Communication;
using VoTCore.Package;

var head = new VOTPHeaderV1(1, 1, 1, 1);
var body = new MediaMessage(0, "Hello Galaxy", 1, 87549875, null);

var package = new VOTP(head, body);

var serialized = package.Serialize();

Console.WriteLine(serialized);

var deserialize = new VOTP(serialized);
/*
var info = new VOTPInfo(package);
var serialized  = JsonSerializer.Serialize(info);
var serializedb = JsonSerializer.Serialize(Convert.ChangeType(package.Data, package.Data.GetType()));
var serializedh = JsonSerializer.Serialize(package.Header as VOTPHeaderV1);

Console.WriteLine(serialized.ToString());
Console.WriteLine(serializedh.ToString()); 
Console.WriteLine(serializedb.ToString());

var deSerialized = JsonSerializer.Deserialize<VOTPInfo>(serialized);

VOTP newVOTP;
serializedh = null;

if (deSerialized != null)
if(deSerialized.Version == 1)
    try
    {
        newVOTP = new(JsonSerializer.Deserialize<VOTPHeaderV1>(serializedh), JsonSerializer.Deserialize<FileMessage>(serializedb));
    
    }catch(Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
*/
Console.ReadKey();






