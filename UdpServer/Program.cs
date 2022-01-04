using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using FancyLibrary.Bridges;


namespace UdpServer {

    class Server {
        private static byte[]? data = null;
        private static string? input = null;
        private static UdpClient? stitch = null;
        private static IPEndPoint? angle = null;


        static void Main() {
            UdpBridgeClient server = new(626, 624) {
                ReplyHeartbeat = true
            };
            server.OnMessageReceived += (p, s) => Console.WriteLine($"receive: {s}");
            // server.MessageSent += s => Console.WriteLine($"send: {s}");

            while (true) {
                server.Send(Console.ReadLine());
            }
        }

        private static void Start() {
            stitch = new UdpClient(626);
            angle = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 624);

            Console.WriteLine("Waiting...");
            // stitch.BeginReceive(new AsyncCallback(Receive), null);
            Task.Run(Receive);

            while (true) {
                try {
                    input = Console.ReadLine();
                    data = Encoding.ASCII.GetBytes(input ?? "null");
                    stitch.Send(data, data.Length, angle);
                } catch (Exception e) {
                    Console.WriteLine("Send Exception");
                    Console.WriteLine(e.Message);
                }
            }
        }
        

        private static async void Receive() {
            while (true) {
                try {
                    UdpReceiveResult result = await stitch.ReceiveAsync();
                    if (!result.RemoteEndPoint.Equals(angle)) continue;
                    data = result.Buffer;
                    Console.WriteLine(Encoding.ASCII.GetString(data, 0, data.Length));
                    data = Encoding.UTF8.GetBytes("");
                    await stitch.SendAsync(data, data.Length, angle);
                } catch (Exception e) {
                    Console.WriteLine("Receive Exception");
                    Console.WriteLine(e.Message);
                }
            }
        }
    }

}