using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Timers;

using FancyLibrary.Logger;
using FancyLibrary.Utils;



namespace FancyLibrary.Bridges {

    public class UdpBridgeClient: BridgeClient {
        public override event ClientOpenedEventHandler ClientOpened;
        public override event ClientClosedEventHandler ClientClosed;

        /// <summary>
        /// Received a message from remote endpoint, MessageManager should process it.
        /// </summary>
        public override event MessageReceivedEventHandler MessageReceived;
        
        /// <summary>
        /// UDP Client has sent a message to remote endpoint. Subscribe this event if in need.
        /// </summary>
        public override event MessageSentEventHandler MessageSent;

        private delegate void SendQueueHasValueEventHandler(ConcurrentQueue<DatagramStruct> queue);

        private event SendQueueHasValueEventHandler OnSendQueueHasValue;

        public bool SendHeartbeat { get; set; } = false;
        public bool ReplyHeartbeat { get; set; } = true;

        private readonly UdpClient localClient;
        private readonly IPEndPoint remoteClient;
        private readonly Timer timer;
        private readonly Task receiveTask;
        private readonly Task detectTask;
        private readonly ConcurrentQueue<DatagramStruct> sendQueue;
        private const int heartbeatTimeSpan = 5 * 60 * 1000;
        private const int timerInterval = 3000;


        public UdpBridgeClient(int localPort, int remotePort) {
            localClient = new UdpClient(localPort);
            remoteClient = new IPEndPoint(IPAddress.Parse("127.0.0.1"), remotePort);
            sendQueue = new ConcurrentQueue<DatagramStruct>();
            OnSendQueueHasValue += Send;

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
                    if (!result.RemoteEndPoint.Equals(remoteClient)) continue;
                    Deal(result.Buffer);
                } catch (Exception e) {
                    LogClerk.Error(e.Message);
                }
            }
        }

        private void Deal(byte[] bytes) {
            bool success = Converter.FromBytes(bytes, out DatagramStruct ds);
            if (!success) return;

            switch (ds.Type) {
                case DatagramType.Heartbeat:
                    // stop the timer. start the timer after sending next heartbeat datagram
                    timer.Stop();
                    if (ReplyHeartbeat) HeartBeat("heartbeat");
                    break;
                case DatagramType.Message:
                    // invoke the message receiver
                    MessageReceived?.Invoke(ds.Content);
                    break;
                default:
                    LogClerk.Warn($"Invalid datagram type:{bytes}");
                    break;
            }
        }

        /// <summary>
        /// Send a message to server
        /// </summary>
        /// <param name="bytes">a string u wanna send</param>
        /// <returns></returns>
        public override void Send(byte[] bytes) {
            sendQueue.Enqueue(PDU(DatagramType.Message, bytes));

            if (sendQueue.Count != 0) {
                OnSendQueueHasValue?.Invoke(sendQueue);
            }
        }

        /// <summary>
        /// send a string, for testing
        /// </summary>
        /// <param name="msg"></param>
        public void Send(string msg) {
            Send(GlobalSettings.Encoding.GetBytes(msg));
        }

        private void HeartBeat(string msg) {
            sendQueue.Enqueue(PDU(DatagramType.Heartbeat, GlobalSettings.Encoding.GetBytes(msg)));
            if (sendQueue.Count != 0) {
                OnSendQueueHasValue?.Invoke(sendQueue);
            }
        }

        /// <summary>
        /// This is the real implementation of sending messages.
        /// Use a queue to deal with traffic congestion.
        /// </summary>
        /// <param name="queue"></param>
        private void Send(ConcurrentQueue<DatagramStruct> queue) {
            while (queue.Count > 0) {
                try {
                    bool success = queue.TryDequeue(out DatagramStruct ds);
                    byte[] dataSend = Converter.GetBytes(ds);
                    localClient.Send(dataSend, dataSend.Length, remoteClient);
                    MessageSent?.Invoke(ds.Content);
                } catch (Exception e) {
                    LogClerk.Error($"Send message failed{e.Message}");
                }
            }
        }

        /// <summary>
        /// Assuming it is a connection:
        /// Send a heartbeat datagram per 5 minutes to detect whether the connection is OK
        /// </summary>
        /// <returns>whether the connection is OK</returns>
        private async void Detect() {
            while (true) {
                if (SendHeartbeat) {
                    sendQueue.Enqueue(PDU(DatagramType.Heartbeat, GlobalSettings.Encoding.GetBytes("heartbeat")));
                    timer.Start();
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
            ClientClosed?.Invoke();
            LogClerk.Warn("The server has disconnected.");
        }

        private static DatagramStruct PDU(DatagramType type, byte[] sdu) {
            return new DatagramStruct {
                Type = type,
                Sid = 0,
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
            localClient.Dispose();
            timer.Dispose();
            ClientClosed?.Invoke();
        }
    }

}