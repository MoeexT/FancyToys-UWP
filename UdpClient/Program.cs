using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using FancyLibrary.Bridges;


namespace Client {

    class Udpclient {
        private static byte[]? data = null;
        private static string? input = null;
        private static UdpClient? angle = null;
        private static IPEndPoint? stitch = null;

        static void Main() {
            UdpBridgeClient client = new(624, 626) {
                SendHeartbeat = true
            };
            client.MessageReceived += s => Console.WriteLine($"receive: {s}");
            // client.MessageSent += s => Console.WriteLine($"send: {s}");

            while (true) {
                client.Send(Console.ReadLine());
            }
        }

        private static void Start() {
            angle = new UdpClient(624);
            stitch = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 626);
            Console.WriteLine("Waiting...");

            // angle.BeginReceive(new AsyncCallback(Receive), null);
            Task.Run(Receive);

            while (true) {
                try {
                    input = Console.ReadLine();
                    data = Encoding.ASCII.GetBytes(input ?? "null");
                    angle.Send(data, data.Length, stitch);
                } catch (Exception e) {
                    Console.WriteLine("Send Exception");
                    Console.WriteLine(e.Message);
                }
            }
        }

        private static async void Receive() {
            while (true) {
                try {
                    UdpReceiveResult result = await angle.ReceiveAsync();
                    if (!result.RemoteEndPoint.Equals(stitch)) continue;
                    data = result.Buffer;
                    Console.WriteLine($"{Encoding.UTF8.GetString(data, 0, data.Length)}({data.Length})");
                } catch (Exception e) {
                    Console.WriteLine("Receive Exception");
                    Console.WriteLine(e.Message);
                }
            }
        }
    }

}