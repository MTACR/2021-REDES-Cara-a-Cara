using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace Network
{
    public class Host : MonoBehaviour
    {
        public string ip = "26.158.168.172";
        private Thread thread;
        private Socket socket;
        private static readonly ManualResetEvent reset = new ManualResetEvent(false);

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
                    Debug.LogError("IP address was not bound");
                    return;
                }

                IPEndPoint endPoint = new IPEndPoint(ipAddress, 1024);
                Socket handler = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                handler.Bind(endPoint);
                handler.Listen(1);

                while (true)
                {
                    Debug.Log("Waiting for a connection...");
                    reset.Reset();
                    
                    handler.BeginAccept(result =>
                    {
                        reset.Set();
                        
                        socket = handler.EndAccept(result);
                        StateObject stateObject = new StateObject();
                        
                        Debug.Log("Connection received from " + socket.RemoteEndPoint); 
                        
                        socket.BeginReceive(stateObject.buffer, 0, StateObject.BufferSize, 0, HandleClient, stateObject);
                    }, handler);
                    
                    reset.WaitOne();
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }

        private void HandleClient(IAsyncResult result) 
        {
            if (socket == null)
            {
                Debug.LogError("Host socket is null");
                return;
            }
            
            StateObject stateObject = (StateObject) result.AsyncState;
            //Socket socket = stateObject.socket;
            int bytesRead = socket.EndReceive(result);

            if (bytesRead <= 0) return;
            
            stateObject.sb.Append(Encoding.ASCII.GetString(stateObject.buffer, 0, bytesRead));
            String data = stateObject.sb.ToString();
            
            //TODO aqui vai lidar com as mensagens
            
            if (data.IndexOf("<EOF>", StringComparison.Ordinal) > -1) 
            {
                
                
                Send(data);
            }
            else 
            {
                socket.BeginReceive(stateObject.buffer, 0, StateObject.BufferSize, 0, HandleClient, stateObject);
            }
        }

        private void Send(String data)
        {
            if (socket == null)
            {
                Debug.LogError("Host socket is null");
                return;
            }
            
            byte[] byteData = Encoding.ASCII.GetBytes(data);
            
            socket.BeginSend(byteData, 0, byteData.Length, SocketFlags.None, result =>
            {
                try
                {
                    socket.EndSend(result);
                    //socket.Shutdown(SocketShutdown.Both);
                    //socket.Close();
                    Debug.Log("<- " + data);
                    
                    reset.Set();
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                }
            }, socket);
        }

        void Awake()
        {
            Debug.Log("Starting host...");
            
            DontDestroyOnLoad(this);

            thread = new Thread(Init) {IsBackground = true};
            thread.Start();
        }
        
        private void OnDisable()
        {
            thread.Abort();
        }

    }
}
