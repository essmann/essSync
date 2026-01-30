using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace essSync.tests
{
    [TestFixture]
    public class WebSocketTest
    {
        [Test]
        public async Task TestWebSocketConnection()
        {
            using var ws = new ClientWebSocket();

            try
            {
                await ws.ConnectAsync(new Uri("ws://localhost:8080/test"), CancellationToken.None);
                Console.WriteLine("Connected!");

                var message = Encoding.UTF8.GetBytes("Hello from C# client");
                await ws.SendAsync(new ArraySegment<byte>(message),
                    WebSocketMessageType.Text, true, CancellationToken.None);

                var buffer = new byte[1024];
                var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                Console.WriteLine($"Received: {Encoding.UTF8.GetString(buffer, 0, result.Count)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}