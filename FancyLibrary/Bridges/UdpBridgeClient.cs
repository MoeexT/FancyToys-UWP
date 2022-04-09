using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Timers;

using FancyLibrary.Utils;

using Debugger = FancyLibrary.Logging.Debugger;


namespace FancyLibrary.Bridges {

    internal class UdpBridgeClient: BridgeClient {
        public override event ClientOpenedEventHandler OnClientOpened;
        public override event ClientClosedEventHandler OnClientClosed;

        /// <summary>
        /// Received a message from remote endpoint, MessageManager should process it.
        /// </summary>
        public override event MessageReceivedEventHandler OnMessageReceived;

        /// <summary>
        /// UDP Client has sent a message to remote endpoint. Subscribe this event if in need.
        /// </summary>
        public override event MessageSentEventHandler OnMessageSent;

        public bool SendHeartbeat { get; set; }
        public bool ReplyHeartbeat { get; set; } = true;
        private bool isConnect;

        private readonly UdpClient localClient;
        private readonly IPEndPoint remoteEndPoint;
        private readonly Timer timer;
        private readonly Task receiveTask;
        private readonly Task detectTask;
        private const int heartbeatTimeSpan = 3000;
        private const int timerInterval = 5000;

        public UdpBridgeClient(int localPort, int remotePort) {
            localClient = new UdpClient(localPort);
            remoteEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), remotePort);

            // heartbeat timer
            timer = new Timer(timerInterval) {
                Enabled = true,
                AutoReset = true,
            };
            timer.Elapsed += OnTimerElapsed;
            receiveTask = Task.Run(Receive);
            detectTask = Task.Run(Detect);
        }

        /// <summary>
        /// Receive message asynchronously from server
        /// </summary>
        public override async void Receive() {
            while (true) {
                try {
                    UdpReceiveResult result = await localClient.ReceiveAsync();
                    if (!result.RemoteEndPoint.Equals(remoteEndPoint)) continue;
                    timer.Stop();
                    timer.Start();

                    if (!isConnect) {
                        isConnect = true;
                        OnClientOpened?.Invoke();
                    }
                    Deal(result.Buffer);
                } catch (Exception e) {
                    // Debugger.Println($"Receive failed: {e.Message}");
                }
            }
        }

        private void Deal(byte[] bytes) {
            bool success = Converter.FromBytes(bytes, out DatagramStruct ds);
            if (!success) return;

            switch (ds.Type) {
                case DatagramType.Heartbeat:
                    if (ReplyHeartbeat) Replay();
                    break;
                case DatagramType.Replay:
                    break;
                case DatagramType.Message:
                    OnMessageReceived?.Invoke(ds.Port, ds.Content);
                    Debugger.Println(Consts.Encoding.GetString(ds.Content));
                    break;
                case DatagramType.Package:
                    // invoke the message receiver
                    OnMessageReceived?.Invoke(ds.Port, ds.Content);
                    break;
                default:
                    Debugger.Println($"Unknown message:{Consts.Encoding.GetString(ds.Content)}");
                    break;
            }
        }

        /// <summary>
        /// Send a message to server
        /// </summary>
        /// <param name="port">sender's port</param>
        /// <param name="bytes">a string u wanna send</param>
        /// <returns></returns>
        public override void Send(int port, byte[] bytes) {
            byte[] content = Converter.GetBytes(PDU(DatagramType.Package, port, bytes));
            localClient.SendAsync(content, content.Length, remoteEndPoint);
            OnMessageSent?.Invoke();
        }

        /// <summary>
        /// send a string, for testing
        /// </summary>
        /// <param name="msg"></param>
        public void Send(string msg) {
            byte[] content = Converter.GetBytes(PDU(DatagramType.Message, -1, Consts.Encoding.GetBytes(msg)));
            localClient.SendAsync(content, content.Length, remoteEndPoint);
        }

        private void Heartbeat(string msg = "heartbeat") {
            byte[] content = Converter.GetBytes(PDU(DatagramType.Heartbeat, 0, Consts.Encoding.GetBytes(msg)));
            localClient.SendAsync(content, content.Length, remoteEndPoint);
        }

        private void Replay(string msg = "Replay") {
            byte[] content = Converter.GetBytes(PDU(DatagramType.Replay, 0, Consts.Encoding.GetBytes(msg)));
            localClient.SendAsync(content, content.Length, remoteEndPoint);
        }

        /// <summary>
        /// Assuming it is a connection:
        /// Send a heartbeat datagram per 5 minutes to detect whether the connection is OK
        /// </summary>
        /// <returns>whether the connection is OK</returns>
        private async void Detect() {
            while (true) {
                if (SendHeartbeat) {
                    Heartbeat();
                }
                await Task.Delay(heartbeatTimeSpan);
            }
        }

        /// <summary>
        /// Invoke ClientClosed event when the timer elapse.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTimerElapsed(object sender, ElapsedEventArgs e) {
            OnClientClosed?.Invoke();
            isConnect = false;
        }

        private static DatagramStruct PDU(DatagramType type, int port, byte[] sdu) {
            return new DatagramStruct {
                Type = type,
                Seq = 0,
                Port = port,
                Content = sdu
            };
        }

        /// <summary>
        /// Close the client
        /// </summary>
        public override void Close() {
            receiveTask?.Dispose();
            detectTask?.Dispose();
            localClient.Close();
            timer.Dispose();
            OnClientClosed?.Invoke();
            isConnect = false;
        }
    }

}
