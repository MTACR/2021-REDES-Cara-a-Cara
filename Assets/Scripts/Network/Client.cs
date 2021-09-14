using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace Network
{
    public class Client : MonoBehaviour
    {
        public string ip = "26.158.168.172";
        private Thread thread;
        private Socket socket;
        public bool isHost;

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
                    return;
                }

                IPEndPoint endPoint = new IPEndPoint(ipAddress, 1024);

                if (isHost)
                {
                    Socket handler = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    handler.Bind(endPoint);
                    handler.Listen(1);
                    
                    handler.BeginAccept(result =>
                    {
                        socket = handler.EndAccept(result);
                        StateObject state = new StateObject();
                        
                        Debug.Log("Connection received from " + socket.RemoteEndPoint); 
                        
                        socket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, ReceiveCallback, state);
                    }, handler);
                }
                else
                {
                    socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                    socket.BeginConnect(endPoint, result =>
                    {
                        try
                        {
                            socket.EndConnect(result);
                            StateObject state = new StateObject();
                            
                            Debug.Log("Connected to " + socket.RemoteEndPoint);
                            
                            socket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, ReceiveCallback, state);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                        }
                    }, socket);
                }

                /* Release the socket.
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();*/
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }
        
        public void Send(String data)
        {
            if (socket == null)
            {
                Debug.LogError("Client socket is null");
                return;
            }
            
            byte[] bytes = Encoding.ASCII.GetBytes(data);
  
            socket.BeginSend(bytes, 0, bytes.Length, SocketFlags.None, result =>
            {
                try 
                {
                    socket.EndSend(result);
                    Debug.Log("-> " + data + " :: " + bytes.Length + " bytes");
                } 
                catch (Exception e) 
                {
                    Debug.LogError(e.ToString());
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
            
            String data = Encoding.ASCII.GetString(state.buffer, 0, bytes);
            Debug.Log("<- " + data + " :: " + bytes + " bytes");

            socket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, ReceiveCallback, state);
        } 

        void Awake()
        {
            Debug.Log("Starting client...");
            
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
