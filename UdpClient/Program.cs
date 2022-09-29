using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using FancyLibrary;
using FancyLibrary.Bridges;
using FancyLibrary.Nursery;


namespace Client {

    class Udpclient {
        private static byte[]? data = null;
        private static string? input = null;
        private static UdpClient? angle = null;
        private static IPEndPoint? stitch = null;

        static async Task Main() {
            UdpBridgeClient client = new(624, 626);

            // client.OnPacketReceived += (p) => {
            //     if (p.Method == RequestMethod.Request) {
            //         Console.WriteLine($"receive: {Consts.Encoding.GetString(p.Content)}");
            //         client.Response(0, p.Seq, p.Content);
            //     }
            // };
            // client.MessageSent += s => Console.WriteLine($"send: {s}");

            client.RegisterNotifyHandler((NurseryInformationStruct sct) => {
                Console.WriteLine($"notify: {sct}");
            });


            input = "";

            client.OnClientOpened += async () => {
                while (!input.Equals("exit")) {
                    input = Console.ReadLine() ?? string.Empty;
                    // input = "AutoInput";

                    switch (input) {
                        case "info":
                            client.Info(false);
                            continue;
                        case "detail":
                            client.Info(true);
                            continue;
                        default:
                            NurseryInformationStruct res = await client.Request(new NurseryInformationStruct() {
                                Id = 1234,
                                ProcessName = "I'm client",
                                Memory = 333 << 10,
                                CPU = 66.27,
                            });
                            Console.WriteLine($"response: {res}");
                            break;
                    }
                    Thread.Sleep(1000);
                }
            };
            
            while (true) {
                Thread.Sleep(1000);
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
