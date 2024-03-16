// Server Side => TcpListener

using System.Net;
using System.Net.Sockets;

internal class User
{
    public string UserName { get; set; }
    public IPEndPoint client{ get; set; }
   
}