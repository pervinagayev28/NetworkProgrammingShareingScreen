

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

IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse("192.168.100.15"), 45001);

var server = new UdpClient(endpoint);

EndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);

var buffer = new byte[ushort.MaxValue - 29];

var clients = new List<User>();


while (true)

{

    UdpReceiveResult resultClient = await server.ReceiveAsync();
    Console.Write("FIRST ENTERACNE: ");
    Console.WriteLine(resultClient.RemoteEndPoint);

    new Thread(async () =>

    {

        var client = resultClient.RemoteEndPoint;
        var buffer = resultClient.Buffer;

        var user = clients.FirstOrDefault(c => c.client == client);

        if (user is null)

        {

            user = new()
            {
                UserName = Encoding.UTF8.GetString(buffer),
                client = client
            };

            clients.Add(user);

        }

        while (true)

        {
            await Console.Out.WriteLineAsync("STARTED WHILE LINE 70");
            var remoteName = server.Receive(ref client);
            await Console.Out.WriteAsync($"from name: {user.UserName}    =>   ");
            await Console.Out.WriteLineAsync($"to name: {Encoding.UTF8.GetString(remoteName)}");
            IPEndPoint? clientEp = clients.FirstOrDefault(c => Encoding.UTF8.GetString(remoteName).Contains(c.UserName))?.client;

            new Thread(async () =>
            {
                if (true)
                {
                    await Console.Out.WriteLineAsync("entered: " + clientEp);
                    var tempBuffer = new byte[ushort.MaxValue - 29];
                    int maxlen = buffer.Length;
                    int len = 0;
                    while (true)
                    {
                        await Console.Out.WriteLineAsync("sending");

                        try
                        {
                            do
                            {
                                var result = server.Receive(ref client);
                                await Console.Out.WriteLineAsync("CHUNKS");
                                tempBuffer = result;
                                len = tempBuffer.Length;
                                await Console.Out.WriteLineAsync("lrn: " + len.ToString());
                                try { await server.SendAsync(tempBuffer, tempBuffer.Length, client); } catch { await Console.Out.WriteLineAsync("error"); };
                            } while (len == maxlen);

                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    }


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