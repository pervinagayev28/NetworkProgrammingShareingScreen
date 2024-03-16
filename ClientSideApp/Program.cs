using System.Net;
using System.Net.Sockets;
using System.Text;

var client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
var remoteEP = new IPEndPoint(IPAddress.Parse("192.168.100.9"), 27000);

var buffer = new byte[1024];
while (true)
{
    Console.Write("Message daxil edin: ");
    buffer = Encoding.UTF8.GetBytes(Console.ReadLine());
    client.SendTo(buffer, remoteEP);
}


