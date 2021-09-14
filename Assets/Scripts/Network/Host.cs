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
        //private static readonly ManualResetEvent reset = new ManualResetEvent(false);

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

                Debug.Log("Waiting for a connection...");
                
                while (true)
                {
                    //reset.Reset();
                    
                    handler.BeginAccept(result =>
                    {
                        //reset.Set();
                        
                        socket = handler.EndAccept(result);
                        StateObject state = new StateObject();
                        
                        Debug.Log("Connection received from " + socket.RemoteEndPoint); 
                        
                        socket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, ReceiveCallback, state);
                    }, handler);
                    
                    //reset.WaitOne();
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }

        private void ReceiveCallback(IAsyncResult result) 
        {
            if (socket == null)
            {
                Debug.LogError("Host socket is null");
                return;
            }

            StateObject state = (StateObject) result.AsyncState;
            int bytesRead = socket.EndReceive(result);
            
            Debug.Log("Host <- :: " + bytesRead + " bytes");
            
            if (bytesRead <= 0) return;
            
            String data = Encoding.ASCII.GetString(state.buffer, 0, bytesRead);
            Debug.Log("Host <- " + data);
            
            socket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, ReceiveCallback, state);
            //TODO aqui vai lidar com as mensagens
            
            /*if (data.StartsWith("CRD_OP"))
            {
                int id = Convert.ToInt32(data.Substring(7, 8));
            }
            
            if (data.IndexOf("<EOF>", StringComparison.Ordinal) > -1) 
            {
                Send(data);
            }
            else
            {
                socket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, ReceiveCallback, state);
            }*/
        }

        public void Send(String data)
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
                    Debug.Log("Host -> " + data);
                    
                    //reset.Set();
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
