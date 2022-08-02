using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;

using FancyLibrary.Utils;
using FancyLibrary.Utils.Collections;

using Debugger = FancyLibrary.Logging.Debugger;


namespace FancyLibrary.Bridges {

    public class UdpBridgeClient: BridgeClient {
        public override event ClientOpenedEventHandler OnClientOpened;
        public override event ClientClosedEventHandler OnClientClosed;

        public delegate void NotifyHandler<T>(T sct);
        public delegate T RequestHandler<T>(T request);

        private readonly Dictionary<ulong, TaskCompletionSource<DatagramStruct>> _responseQueue;

        // map structure's type to number used to serialization
        private byte _structTypePortPointer = 0;
        private readonly DoubledMap<Type, byte> _structTypePort;
        // handlers for notification
        private readonly Dictionary<byte, object> _notifyHandlers;
        // handlers for request
        private readonly Dictionary<byte, object> _requestHandlers;
        private readonly Dictionary<byte, MethodInfo> _notifyReceivers;

        private MethodInfo _handleNotify;
        private MethodInfo _handleRequest;

        public bool SendHeartbeat { get; set; }
        public bool ReplyHeartbeat { get; set; } = true;
        private bool isConnect;
        private bool autoCleanSendCache;
        private ulong seqId;

        private readonly UdpClient localClient;
        private readonly IPEndPoint remoteEndPoint;
        private readonly Timer timer;
        private readonly Task receiveTask;
        private readonly Task detectTask;
        private const int heartbeatTimeSpan = 3000;
        private const int timerInterval = 5000;
        private const int sendTimeout = 5000;
        private const int sendCacheCleanInterval = 10000;

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
            // Task.Run(CleanReceiveCache);
            _responseQueue = new Dictionary<ulong, TaskCompletionSource<DatagramStruct>>();

            _structTypePort = new DoubledMap<Type, byte>();
            _notifyHandlers = new Dictionary<byte, object>();
            _requestHandlers = new Dictionary<byte, object>();

            _handleNotify = GetType().GetMethod(nameof(handleNotify), BindingFlags.Instance | BindingFlags.NonPublic);
            _handleRequest = GetType().GetMethod(nameof(handleRequest), BindingFlags.Instance | BindingFlags.NonPublic);

        }

        /// <summary>
        /// Receive message asynchronously from server
        /// </summary>
        public override async void Receive() {
            while (true) {
                try {
                    UdpReceiveResult result = await localClient.ReceiveAsync();

                    if (!result.RemoteEndPoint.Equals(remoteEndPoint)) {
                        Console.WriteLine($"{result.RemoteEndPoint}, {remoteEndPoint}");
                        continue;
                    }
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
            Console.WriteLine("receive");
            bool success = Converter.FromBytes(bytes, out DatagramStruct ds);
            if (!success) return;
            byte t = ds.StructType;

            switch (ds.Method) {
                case RequestMethod.Request:
                    Console.WriteLine("receive request " + _structTypePort[t]);
                    _handleRequest?.MakeGenericMethod(_structTypePort[t]).Invoke(this, new object[] { ds.Seq, t, ds.Content });
                    break;
                case RequestMethod.Notify:
                    Console.WriteLine("received notify");
                    _handleNotify?.MakeGenericMethod(_structTypePort[t]).Invoke(this, new object[] { t, ds.Content });
                    if (ReplyHeartbeat) Reply();
                    break;
                case RequestMethod.Response:
                    Console.WriteLine("receive response");
                    if (_responseQueue.TryGetValue(ds.Ack, out TaskCompletionSource<DatagramStruct> tcs)) {
                        _responseQueue.Remove(ds.Ack);
                        tcs.SetResult(ds);
                    }
                    break;
                default:
                    Debugger.Println($"Unknown message:{Consts.Encoding.GetString(ds.Content)}");
                    break;
            }
        }

        public void RegisterRequestHandler<T>(RequestHandler<T> handler) {
            Type t = typeof(T);

            if (!_structTypePort.TryGetValue(t, out byte p)) {
                _structTypePort.Set(t, _structTypePortPointer++);
            }
            _requestHandlers.Add(_structTypePort[t], handler);
        }

        public void RegisterNotifyHandler<T>(NotifyHandler<T> handler) {
            Type t = typeof(T);

            if (!_structTypePort.TryGetValue(t, out byte p)) {
                _structTypePort.Set(t, _structTypePortPointer++);
            }
            _notifyHandlers.Add(_structTypePort[t], handler);
        }

        public async Task<T> Request<T>(T sct) {
            DatagramStruct req = PDU(RequestMethod.Request, _structTypePort[sct.GetType()], 0, Converter.GetBytes(sct));

#pragma warning disable CS4014
            send(req);
#pragma warning restore CS4014

            var tcs = new TaskCompletionSource<DatagramStruct>();
            _responseQueue[req.Seq] = tcs;

            new Timer(sendTimeout) {
                Enabled = true,
            }.Elapsed += (sender, args) => {
                tcs.SetException(new TimeoutException("Request has timeout."));
            };

            DatagramStruct response = await tcs.Task;

            if (Converter.FromBytes(response.Content, out T s)) {
                return s;
            }
            throw new Converter.ConverterException("Convert byte array to struct failed");
        }

        private void handleRequest<T>(ulong seq, byte type, byte[] content) {
            if (!_requestHandlers.TryGetValue(type, out object o) || o is not RequestHandler<T> handler) {
                Debugger.Println($"没有处理{type}的request handler");
                return;
            }

            if (Converter.FromBytes(content, out T s)) {
                response(seq, type, Converter.GetBytes(handler(s)));
            }
        }

        public void Notify(object sct) {
            send(PDU(RequestMethod.Notify, _structTypePort[sct.GetType()], 0, Converter.GetBytes(sct)));
        }

        private void handleNotify<T>(byte type, byte[] content) {
            if (!_notifyHandlers.TryGetValue(type, out object o) || o is not NotifyHandler<T> handler) {
                Debugger.Println($"没有处理{type}的notify handler");
                return;
            }

            if (Converter.FromBytes(content, out T s)) {
                handler(s);
            }
        }

        private void response(ulong ack, byte type, byte[] content) {
            send(PDU(RequestMethod.Response, type, ack, content));
        }

        private Task<int> send(DatagramStruct ds) {
            byte[] content = Converter.GetBytes(ds);
            return localClient.SendAsync(content, content.Length, remoteEndPoint);
        }

        public async void Info(bool detail = false) {
            Console.WriteLine($"Dict size: {_responseQueue.Count}");

            if (detail) {
                foreach (KeyValuePair<ulong, TaskCompletionSource<DatagramStruct>> kv in _responseQueue) {
                    Console.WriteLine($"{kv.Key}: {await kv.Value.Task}");
                }
            }
        }

        [Obsolete]
        private void Heartbeat(string msg = "heartbeat") {
            send(PDU(RequestMethod.Notify, _structTypePort[msg.GetType()], 0, Consts.Encoding.GetBytes(msg)));
        }

        [Obsolete]
        private void Reply(string msg = "Reply") {
            send(PDU(RequestMethod.Notify, _structTypePort[msg.GetType()], 0, Consts.Encoding.GetBytes(msg)));
        }

        private async void CleanReceiveCache() {
            var prevCached = new HashSet<ulong>();
            uint count = 0;

            while (true) {
                foreach (ulong key in _responseQueue.Keys) {
                    if (prevCached.Contains(key)) {
                        _responseQueue.Remove(key);
                        count++;
                    } else {
                        prevCached.Add(key);
                    }
                }
                Console.WriteLine($"cleaned cache count: {count}");
                await Task.Delay(sendCacheCleanInterval);
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

        private DatagramStruct PDU(RequestMethod method, byte type, ulong ack, byte[] content) {
            return new DatagramStruct {
                Method = method,
                StructType = type,
                Seq = seqId++,
                Ack = ack,
                Content = content,
            };
        }

    }

}
