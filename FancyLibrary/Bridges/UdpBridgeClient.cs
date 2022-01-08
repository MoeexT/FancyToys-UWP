using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Timers;

using FancyLibrary.Utils;


namespace FancyLibrary.Bridges {

    public class UdpBridgeClient: BridgeClient {
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

        public bool SendHeartbeat { get; set; } = false;
        public bool ReplyHeartbeat { get; set; } = true;

        private readonly UdpClient localClient;
        private readonly IPEndPoint remoteEndPoint;
        private readonly Timer timer;
        private readonly Task receiveTask;
        private readonly Task detectTask;
        private const int heartbeatTimeSpan = 1000;
        private const int timerInterval = 500;

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
                    Deal(result.Buffer);
                } catch (Exception e) { Debug.WriteLine(e.Message); }
            }
        }

        private void Deal(byte[] bytes) {
            bool success = Converter.FromBytes(bytes, out DatagramStruct ds);
            if (!success) return;

            switch (ds.Type) {
                case DatagramType.Heartbeat:
                    // stop the timer. start the timer after sending next heartbeat datagram
                    timer.Stop();
                    if (ReplyHeartbeat) Heartbeat();
                    break;
                case DatagramType.Message:
                    // invoke the message receiver
                    OnMessageReceived?.Invoke(ds.Port, ds.Content);
                    break;
                default:
                    Debug.WriteLine($"Invalid datagram type:{bytes}");
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
            localClient.SendAsync(Converter.GetBytes(PDU(DatagramType.Message, port, bytes)), bytes.Length, remoteEndPoint);
            OnMessageSent?.Invoke();
        }

        /// <summary>
        /// send a string, for testing
        /// </summary>
        /// <param name="msg"></param>
        public void Send(string msg) =>
            Send(-1, Converter.GetBytes(PDU(DatagramType.Message, -1, Consts.Encoding.GetBytes(msg))));

        private void Heartbeat(string msg = "heartbeat") =>
            Send(0, Converter.GetBytes(PDU(DatagramType.Heartbeat, 0, Consts.Encoding.GetBytes(msg))));

        /// <summary>
        /// Assuming it is a connection:
        /// Send a heartbeat datagram per 5 minutes to detect whether the connection is OK
        /// </summary>
        /// <returns>whether the connection is OK</returns>
        private async void Detect() {
            while (true) {
                Heartbeat();
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
            Debug.WriteLine("The server has disconnected.");
        }

        private static DatagramStruct PDU(DatagramType type, int port, byte[] sdu) {
            return new DatagramStruct {
                Type = type,
                Sid = 0,
                Port = port,
                Content = sdu
            };
        }

        /// <summary>
        /// Close the client
        /// </summary>
        public override void Close() {
            receiveTask.Dispose();
            detectTask.Dispose();
            localClient.Close();
            timer.Dispose();
            OnClientClosed?.Invoke();
        }
    }

}