using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Sockets;
using System.Net;
using System;
using System.Drawing;
using System.Text.Json;


namespace ClientSideWebTemplate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UdpController : ControllerBase
    {
        byte[] buffer = new byte[ushort.MaxValue - 29];
        static IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 27001);
        static IPEndPoint remoteep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 27000);

        static UdpClient client;
        int maxlen = ushort.MaxValue - 29;
        int len = 0;
        List<byte> list = new List<byte>();
        private static bool once = true;
        public void InitializerUdp() =>
            client = new UdpClient(endpoint);

        [HttpGet("send")]
        public async Task<Image> GetImage()
        {
            if (once)
            {
                InitializerUdp();
                await client.SendAsync(buffer, remoteep);
                once = false;
            }
            do
            {
                var result = await client.ReceiveAsync();
                buffer = result.Buffer;
                len = buffer.Length;
                list.AddRange(buffer);
            } while (len == maxlen);
            return Convert(list.ToArray());

        }

        public Image Convert(byte[] buffer)
        {
            using (var ms = new MemoryStream(buffer))
                return Image.FromStream(ms);
        }

    }
}
