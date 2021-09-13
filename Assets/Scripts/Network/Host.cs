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
        private static readonly ManualResetEvent reset = new ManualResetEvent(false);

        private void Init()
        {
            try
            {
                IPHostEntry host = Dns.GetHostEntry(ip);
                IPAddress ipAddress = null;

                foreach (var address in host.AddressList)
                    if (address.MapToIPv4().ToString() == ip)
                        ipAddress = host.AddressList[5];

                if (ipAddress == null)
                {
                    Debug.LogError("IP address was not bound");
                    return;
                }

                IPEndPoint endPoint = new IPEndPoint(ipAddress, 1024);
                Socket socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                socket.Bind(endPoint);
                socket.Listen(2);

                while (true)
                {
                    Debug.Log("Waiting for a connection...");
                    reset.Reset();
                    
                    socket.BeginAccept(result =>
                    {
                        reset.Set();
                        
                        Socket handler = socket.EndAccept(result);
                        StateObject stateObject = new StateObject {socket = handler};
                        
                        Debug.Log("Connection received from " + handler.RemoteEndPoint); 
                        
                        handler.BeginReceive(stateObject.buffer, 0, StateObject.BufferSize, 0, HandleClient, stateObject);
                    }, socket);
                    
                    reset.WaitOne();
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }

        private static void HandleClient(IAsyncResult result) 
        {
            StateObject stateObject = (StateObject) result.AsyncState;
            Socket handler = stateObject.socket;
            int bytesRead = handler.EndReceive(result);

            if (bytesRead <= 0) return;
            
            stateObject.sb.Append(Encoding.ASCII.GetString(stateObject.buffer, 0, bytesRead));
            String content = stateObject.sb.ToString();
            
            //TODO aqui vai lidar com as mensagens
            
            if (content.IndexOf("<EOF>", StringComparison.Ordinal) > -1) {
                Debug.Log("<- " + content);

                byte[] byteData = Encoding.ASCII.GetBytes(content);
                handler.BeginSend(byteData, 0, byteData.Length, 0, ar =>
                {
                    try
                    {
                        handler.EndSend(ar);
                        handler.Shutdown(SocketShutdown.Both);
                        handler.Close();
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e.ToString());
                    }
                }, handler);
            } 
            else 
            {
                handler.BeginReceive(stateObject.buffer, 0, StateObject.BufferSize, 0, HandleClient, stateObject);
            }
        }

        void Awake()
        {
            Debug.Log("Starting host...");

            thread = new Thread(Init) {IsBackground = true};
            thread.Start();
        }
        
        private void OnDisable()
        {
            thread.Abort();
        }

    }
}
