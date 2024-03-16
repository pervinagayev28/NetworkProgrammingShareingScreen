
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 27000);
var server = new UdpClient(endpoint);
EndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
var buffer = new byte[ushort.MaxValue - 29];

var clients = new List<User>();



while (true)
{
    new Task(async () =>
    {
        UdpReceiveResult resultClient = await server.ReceiveAsync();
        var clientEP = resultClient.RemoteEndPoint;
        var user = clients.FirstOrDefault(c => c.client == clientEP);
        if (user is null)
        {
            await server.SendAsync(Encoding.UTF8.GetBytes("enter your name please: "), clientEP);
            var dataGram = await server.ReceiveAsync();

            user = new()
            {
                UserName = Encoding.UTF8.GetString(dataGram.Buffer),
                client = clientEP
            };
            clients.Add(user);
        }
        while (true)
        {
            var dataGram = await server.ReceiveAsync();
            IPEndPoint? client = clients.FirstOrDefault(c => Encoding.UTF8.GetString(dataGram.Buffer).Contains(c.UserName))?.client;
            new Thread(async () =>
            {
                if (client is not null)
                {
                    var imgStream = captureScreenAsync();
                    var chunks = imgStream.ToArray().Chunk(ushort.MaxValue - 29).ToList();
                    foreach (var item in chunks)
                        try { await server.SendAsync(item, item.Length, clientEP); } catch { break; };
                }
            }).Start();

        }
    }).Start();

}




MemoryStream? captureScreenAsync()
{
    using (Bitmap? bitmap = new Bitmap(1920, 1080))
    {
        using (Graphics gr = Graphics.FromImage(bitmap))
            gr?.CopyFromScreen(0, 0, 0, 0, new Size(1920, 1080));
        using (MemoryStream memoryStream = new MemoryStream())
        {
            bitmap?.Save(memoryStream, ImageFormat.Jpeg);
            return memoryStream;
        }
    }

}




// Server Side => TcpListener

//var ip = IPAddress.Parse("127.0.0.1");
//var port = 27001;


//var listenerEP = new IPEndPoint(ip, port);
//BinaryReader br;
//BinaryWriter bw;

//var listener = new TcpListener(listenerEP);
//listener.Start();

//Console.WriteLine($"{listener.Server.LocalEndPoint}  Listener Started .....");


//var clients = new List<User>();

//while (true)
//{
//    TcpClient client = listener.AcceptTcpClient();
//    _ = Task.Run(() =>
//    {
//        br = new(client.GetStream());
//var user = clients.firstordefault(c => c.client.client?.remoteendpoint == client.client.remoteendpoint);
//if (user is null)
//{
//    bw = new(client.getstream());
//    bw.write("enter your name please: ");
//    var username = br.readstring();
//    user = new()
//    {
//        username = username,
//        client = client
//    };
//    clients.add(user);
//}

//        Console.WriteLine(user?.UserName + " connected...");

//        var clientStream = client.GetStream();
//        var reader = new BinaryReader(clientStream);


//        var readString = "";
//        var userIpEndPoint = "";
//        var index = 0;


//        while (true)
//        {
//            readString = reader.ReadString();//Name
//            TcpClient? client = clients.FirstOrDefault(c => readString.Contains(c.UserName))?.client;
//            if (client is not null)
//            {
//                var writer = new BinaryWriter(client.GetStream());
//                writer.Write(readString.Substring(readString.IndexOf(" ") + 1));
//            }
//        }
//    });
//}