using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Callbacks;
using Cards;
using Unity.VisualScripting;
using UnityEngine;

namespace Network
{
    public class Client : MonoBehaviour
    {
        public string ip = "26.158.168.172";
        private Thread thread;
        private Socket socket;
        public bool isHost;
        private Action onStart;
        private Action onError;
        private Action<string> onConnection;
        private TasksDispatcher dispatcher;
        public bool isReady { get; private set; }

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
                    Debug.LogError("IP address not bound");
                    dispatcher.Schedule(delegate
                    {
                        onError();
                    });
                    return;
                }

                IPEndPoint endPoint = new IPEndPoint(ipAddress, 1024);

                if (isHost)
                {
                    Socket handler = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    handler.Bind(endPoint);
                    handler.Listen(1);
                    
                    Debug.Log("Waiting for connection...");
                    
                    dispatcher.Schedule(delegate
                    {
                        onStart();
                    });
                    
                    handler.BeginAccept(result =>
                    {
                        socket = handler.EndAccept(result);
                        StateObject state = new StateObject();
                        isReady = true;

                        Debug.Log("Connection received from " + socket.RemoteEndPoint);
                        
                        dispatcher.Schedule(delegate
                        {
                            onConnection(socket.RemoteEndPoint.ToString());
                        });
                        
                        socket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, ReceiveCallback, state);
                    }, handler);
                }
                else
                {
                    socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    
                    dispatcher.Schedule(delegate
                    {
                        onStart();
                    });

                    socket.BeginConnect(endPoint, result =>
                    {
                        socket.EndConnect(result);
                        StateObject state = new StateObject();
                        isReady = true;
                            
                        Debug.Log("Connected to " + socket.RemoteEndPoint);
                        
                        dispatcher.Schedule(delegate
                        {
                            onConnection(socket.RemoteEndPoint.ToString());
                        });
                            
                        socket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, ReceiveCallback, state);
                    }, socket);
                }

                /* Release the socket.
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();*/
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
                isReady = false;
                dispatcher.Schedule(delegate
                {
                    onError();
                });
            }
        }
        
        public void Send(byte[] bytes)
        {
            if (socket == null)
            {
                Debug.LogError("Client socket is null");
                dispatcher.Schedule(delegate
                {
                    onError();
                });
                return;
            }

            socket.BeginSend(bytes, 0, bytes.Length, SocketFlags.None, result =>
            {
                try 
                {
                    socket.EndSend(result);
                    Debug.Log("-> " + bytes.ToCommaSeparatedString() + " :: " + bytes.Length + " bytes");
                } 
                catch (Exception e) 
                {
                    Debug.LogError(e.ToString());
                    isReady = false;
                    dispatcher.Schedule(delegate
                    {
                        onError();
                    });
                }
            }, socket);
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            if (socket == null)
            {
                Debug.LogError("Client socket is null");
                return;
            }

            StateObject state = (StateObject) result.AsyncState; 
            int bytes = socket.EndReceive(result);
            if (bytes <= 0) return;
            
            Debug.Log("<- " + state.buffer.ToCommaSeparatedString() + " :: " + bytes + " bytes");
            ReceiverParser.ParseMessage(state);
            
            socket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, ReceiveCallback, state);
        }

        public void StartClient(bool isHost, Action onWaitng, Action<string> onConnection, Action onError)
        {
            this.isHost = isHost;
            this.onStart = onWaitng;
            this.onConnection = onConnection;
            this.onError = onError;
            
            Debug.Log("Starting client...");
            
            thread = new Thread(Init) {IsBackground = true};
            thread.Start();
        }

        void Awake()
        {
            DontDestroyOnLoad(this);
            dispatcher = TasksDispatcher.Instance;
        }

        private void OnDisable()
        {
            if (socket != null)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            thread.Abort();
        }
        
    }
}
