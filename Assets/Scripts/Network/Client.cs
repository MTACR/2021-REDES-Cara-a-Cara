using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Timers;
using Callbacks;
using Unity.VisualScripting;
using UnityEngine;
using Object = System.Object;
using Timer = System.Timers.Timer;

namespace Network
{
    public sealed class Client
    {
        //"26.158.168.172"
        private string ip;
        private Thread thread;
        private Socket socket;
        private Action onStart;
        private Action<string> onError;
        private Timer timer;
        private readonly TasksDispatcher dispatcher;
        private static readonly object locker = new object();  
        private static Client instance;
        public bool isHost{ get; private set; }
        public bool isReady { get; private set; }
        public static Client Instance
        {
            get
            {
                lock (locker)
                    return instance ??= new Client();
            }
        }
        //Talvez não seja necessário fazer a classe thread-safe. Veremos...

        private Client()
        {
            dispatcher = TasksDispatcher.Instance;
        }

        private void Init()
        {
            try
            {
                IPHostEntry host = Dns.GetHostEntry(ip);
                IPAddress ipAddress = null;

                foreach (var address in host.AddressList)
                    if (address.MapToIPv4().ToString() == ip)
                        ipAddress = address;

                if (ipAddress == null)
                {
                    CallError("IP address not bound");
                    return;
                }

                IPEndPoint endPoint = new IPEndPoint(ipAddress, 1024);

                if (isHost)
                {
                    Socket handler = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    handler.Bind(endPoint);
                    handler.Listen(1);

                    Log("Waiting for connection...");

                    dispatcher.Schedule(delegate { onStart(); });
                    timer.Start();

                    handler.BeginAccept(result =>
                    {
                        socket = handler.EndAccept(result);
                        StateObject state = new StateObject();
                        isReady = true;
                        timer.Enabled = false;

                        Log("Connection received from " + socket.RemoteEndPoint);

                        socket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, ReceiveCallback, state);
                    }, handler);
                }
                else
                {
                    socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                    dispatcher.Schedule(delegate { onStart(); });

                    socket.BeginConnect(endPoint, result =>
                    {
                        socket.EndConnect(result);
                        StateObject state = new StateObject();
                        isReady = true;
                        timer.Enabled = false;

                        Log("Connected to " + socket.RemoteEndPoint);

                        socket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, ReceiveCallback, state);
                    }, socket);
                }
            }
            catch (Exception e)
            {
                isReady = false;

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
                    Log("-> " + bytes.ToCommaSeparatedString() + " :: " + bytes.Length + " bytes");
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

            StateObject state = (StateObject) result.AsyncState; 
            int bytes = socket.EndReceive(result);
            if (bytes <= 0) return;
            
            Log("<- " + state.buffer.ToCommaSeparatedString() + " :: " + bytes + " bytes");
            ReceiverParser.ParseMessage(state);
            
            socket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, ReceiveCallback, state);
        }

        public void StartClient(bool isHost, string ip, Action onWaitng, Action<string> onError)
        {
            this.isHost = isHost;
            this.ip = ip;
            this.onStart = onWaitng;
            this.onError = onError;
            
            Log("Starting client...");

            timer = new Timer(10000) {Enabled = true, AutoReset = false};
            timer.Elapsed += OnElapsed;
            
            thread = new Thread(Init) {IsBackground = true};
            thread.Start();
        }

        private void OnElapsed(Object source, ElapsedEventArgs e)
        {
            CallError("No connection received");
        }

        private void CallError(string message)
        {
            Debug.LogError(GetHashCode() + ": " + message);
            dispatcher.Schedule(delegate { onError(message); });
            Dispose();
        }

        public void Dispose()
        {
            Log("Cancelling client...");
            
            lock (locker)
            {
                instance = null;
                socket?.Shutdown(SocketShutdown.Both);
                socket?.Close();
                socket?.Dispose();
                thread.Interrupt();
                thread.Abort();
                timer.Enabled = false;
                GC.SuppressFinalize(this);
            }
        }

        private void Log(string message)
        {
            Debug.Log(GetHashCode() + ": " + message);
        }
        
        ~Client() => this.Dispose();
        
    }
}
