using System.Net;
using System.Net.Sockets;
using System.Text;

var client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);


try
{
    client.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 27000));
Console.Write("enter message: ");
client.Send(Encoding.UTF8.GetBytes(Console.ReadLine()!));

}
catch (Exception)
{

}