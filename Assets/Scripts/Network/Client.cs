using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Timers;
using Callbacks;
using UnityEngine;
using Timer = System.Timers.Timer;

namespace Network
{
    public sealed class Client
    {
        private static readonly object locker = new object();
        private static Client instance;
        private Socket handler;
        private string ip;
        private Action<string> onError;
        private Action onStart;
        private Socket socket;
        private Thread thread;
        private readonly Timer timerPingPong;
        private readonly Timer timerTimeOut;
        public bool imReady;
        public bool opReady;

        private Client()
        {
            timerTimeOut = new Timer(30000) {AutoReset = true};
            timerTimeOut.Elapsed += OnTimeOut;

            timerPingPong = new Timer(10000) {AutoReset = true};
            timerPingPong.Elapsed += OnPing;
        }

        public bool isHost { get; private set; }
        public bool isReady { get; private set; }

        public static Client Instance
        {
            get
            {
                lock (locker)
                {
                    return instance ??= new Client();
                }
            }
        }

        private void Init()
        {
            try
            {
                var host = Dns.GetHostEntry(ip);
                IPAddress ipAddress = null;

                foreach (var address in host.AddressList)
                    if (address.MapToIPv4().ToString() == ip)
                        ipAddress = address;

                if (ipAddress == null)
                {
                    CallError("IP address not bound");
                    return;
                }

                var endPoint = new IPEndPoint(ipAddress, 1024);

                if (isHost)
                {
                    handler = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    handler.Bind(endPoint);
                    handler.Listen(1);

                    Debug.Log("Waiting for connection...");

                    TasksDispatcher.Instance.Schedule(delegate { onStart(); });
                    timerTimeOut.Start();

                    handler.BeginAccept(result =>
                    {
                        socket = handler.EndAccept(result);
                        handler.Close();
                        var state = new State();
                        isReady = true;
                        timerTimeOut.Stop();
                        Ping();

                        Debug.Log("Connection received from " + socket.RemoteEndPoint);

                        //Send(SenderParser.Connection(Connection.Connect));

                        socket.BeginReceive(state.buffer, 0, State.BufferSize, 0, ReceiveCallback, state);
                    }, handler);
                }
                else
                {
                    socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                    TasksDispatcher.Instance.Schedule(delegate { onStart(); });

                    socket.BeginConnect(endPoint, result =>
                    {
                        socket.EndConnect(result);
                        var state = new State();
                        isReady = true;
                        timerTimeOut.Stop();
                        Ping();

                        Debug.Log("Connected to " + socket.RemoteEndPoint);

                        //Send(SenderParser.Connection(Connection.Connect));

                        socket.BeginReceive(state.buffer, 0, State.BufferSize, 0, ReceiveCallback, state);
                    }, socket);
                }
            }
            catch (Exception e)
            {
                isReady = false;
                Debug.LogError(e.ToString());

                if (e.Message.Length > 0)
                    CallError(e.Message);
            }
        }

        public void Send(byte[] bytes)
        {
            if (socket == null)
            {
                CallError("Client socket is null");
                return;
            }

            socket.BeginSend(bytes, 0, bytes.Length, SocketFlags.None, result =>
            {
                try
                {
                    socket.EndSend(result);
                    //Debug.Log("-> " + Encoding.Default.GetString(bytes) + " :: " + bytes.Length + " bytes");
                }
                catch (Exception e)
                {
                    isReady = false;
                    CallError(e.Message);
                }
            }, socket);
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            if (socket == null)
            {
                CallError("Client socket is null");
                return;
            }

            var state = (State) result.AsyncState;
            var bytes = socket.EndReceive(result);
            if (bytes <= 0) return;

            //Debug.Log("<- " + Encoding.Default.GetString(state.buffer) + " :: " + bytes + " bytes");
            ReceiverParser.Message(state);

            socket.BeginReceive(state.buffer, 0, State.BufferSize, 0, ReceiveCallback, state);
        }

        public void StartClient(bool isHost, string ip)
        {
            this.isHost = isHost;
            this.ip = ip;

            Debug.Log("Starting " + (isHost ? "host" : "client"));
            
            thread = new Thread(Init) {IsBackground = true};
            thread.Start();
        }

        private void OnTimeOut(object source, ElapsedEventArgs e)
        {
            CallError("Connection timed out");
        }

        private void OnPing(object source, ElapsedEventArgs e)
        {
            Ping();
        }

        private void CallError(string message)
        {
            Debug.LogError(message);
            TasksDispatcher.Instance.Schedule(delegate { onError(message); });
            Dispose();
        }

        private void Ping()
        {
            Debug.Log("Ping");
            Send(SenderParser.Connection(Connection.Ping));
            timerTimeOut.Start();
            timerPingPong.Stop();
        }

        public void Pong()
        {
            //Debug.Log("Pong");
            timerTimeOut.Stop();
            timerPingPong.Start();
        }

        public void SetListeners(Action onStart, Action<string> onError)
        {
            this.onStart = onStart;
            this.onError = onError;
        }

        public void Dispose()
        {
            Debug.Log("Client disposed");

            lock (locker)
            {
                socket?.Close();
                handler?.Close();
                thread?.Interrupt();
                timerTimeOut?.Dispose();
                timerPingPong?.Dispose();
                instance = null;
            }
        }
    }
}