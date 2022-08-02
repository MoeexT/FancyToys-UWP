using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using FancyLibrary;
using FancyLibrary.Bridges;
using FancyLibrary.Nursery;


namespace UdpServer {

    class Server {
        private static byte[]? data = null;
        private static string? input = null;
        private static UdpClient? stitch = null;
        private static IPEndPoint? angle = null;

        static async Task Main() {
            UdpBridgeClient server = new(626, 624);

            // server.OnPacketReceived += (p) => {
            //     if (p.Method == RequestMethod.Request) {
            //         p.Ack = p.Seq;
            //         server.Response(0, p.Ack, p.Content);
            //     }
            // };

            // Task.Run(async () => {
            //     while (true) {
            //         server.Send(input);
            //         await Task.Delay(2333);
            //     }
            // });
            
            server.RegisterRequestHandler((NurseryInformationStruct sct) => {
                sct.Id = 2333;
                sct.ProcessName = "I'm server";
                sct.Memory = 666 << 10;
                sct.CPU = 22.33;
                return sct;
            });
            
            while (input != "exit") {
                // TODO 发送缓存始终有最后一个 Task<packet>
                input = Console.ReadLine() ?? string.Empty;

                switch (input) {
                    case "info":
                        server.Info(false);
                        continue;
                    case "detail":
                        server.Info(true);
                        continue;
                    case "exit":
                        continue;
                    default:
                        // TODO DatagramStruct res = await server.Request(0, Consts.Encoding.GetBytes(input));
                        var sct = new NurseryInformationStruct() {
                            ProcessName = input,
                        };
                        Console.WriteLine(Marshal.SizeOf(sct));
                        server.Notify(sct);
                        break;
                }
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
