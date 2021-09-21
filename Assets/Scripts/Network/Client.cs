using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
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
        private Timer timer;

        private Client()
        {
            myId = GetHashCode();
            Debug.Log("My id: " + myId);
        }

        public int myId { get; }
        public int opId { get; private set; }
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
                    timer.Start();

                    handler.BeginAccept(result =>
                    {
                        socket = handler.EndAccept(result);
                        handler.Close();
                        var state = new State();
                        isReady = true;
                        timer.Enabled = false;

                        Debug.Log("Connection received from " + socket.RemoteEndPoint);

                        Send(SenderParser.Connection(Connection.Connect));

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
                        timer.Enabled = false;

                        Debug.Log("Connected to " + socket.RemoteEndPoint);

                        Send(SenderParser.Connection(Connection.Connect));

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

        public void SetOpId(int id)
        {
            if (opId != 0)
            {
                CallError("Opponent ID already defined");
                return;
            }

            opId = id;
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
                    Debug.Log("-> " + Encoding.Default.GetString(bytes) + " :: " + bytes.Length + " bytes");
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

            Debug.Log("<- " + Encoding.Default.GetString(state.buffer) + " :: " + bytes + " bytes");
            ReceiverParser.Message(state);

            socket.BeginReceive(state.buffer, 0, State.BufferSize, 0, ReceiveCallback, state);
        }

        public void StartClient(bool isHost, string ip)
        {
            this.isHost = isHost;
            this.ip = ip;

            Debug.Log("Starting " + (isHost ? "host" : "client"));

            thread = new Thread(Init) {IsBackground = true};
            timer = new Timer(30000) {Enabled = true, AutoReset = false};
            timer.Elapsed += OnElapsed;
            thread.Start();
        }

        private void OnElapsed(object source, ElapsedEventArgs e)
        {
            CallError("No connection received");
        }

        private void CallError(string message)
        {
            Debug.LogError(message);
            TasksDispatcher.Instance.Schedule(delegate { onError(message); });
            Dispose();
        }

        public void SetListeners(Action onStart, Action<string> onError)
        {
            this.onStart = onStart;
            this.onError = onError;
        }

        public void Dispose()
        {
            Debug.Log("Connection ended");

            lock (locker)
            {
                socket?.Close();
                handler?.Close();
                thread.Interrupt();
                //thread.Abort();
                timer.Enabled = false;
                timer.Dispose();
                instance = null;
            }
        }

    }
}