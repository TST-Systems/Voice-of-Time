using System;
using Voice_of_Time.Transfer;


/*
var head = new VOTPHeaderV1(1, 1, 1, 1);
var body = new FileMessage(0, "Hello Galaxy", 1, 87549875, null);

var package = new VOTP(head, body);

var serialized = package.Serialize();

Console.WriteLine(serialized);

var deserialize = new VOTP(serialized);

Console.ReadKey();
*/


var socket = new SocketClient();
var _ = socket.SetStreamAsync("Hey There!");

//With an infinite while loop, you can see an output when testing because the function is asynchronous
while (true) { }