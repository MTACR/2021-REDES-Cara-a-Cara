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
                    Debug.LogError("IP address not bound");
                    return;
                }

                IPEndPoint endPoint = new IPEndPoint(ipAddress, 1024);
                socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                socket.BeginConnect(endPoint, result =>
                {
                    try
                    {
                        //reset.Set();
                        socket.EndConnect(result);
  
                        Debug.Log("Connected to " + socket.RemoteEndPoint);
                        
                        StateObject state = new StateObject();
                        
                        //Send("TESTE <EOF>");
                        
                        socket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, ReceiveCallback, state);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                }, socket);
                
                //reset.WaitOne();

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
                
            byte[] byteData = Encoding.ASCII.GetBytes(data);  
  
            socket.BeginSend(byteData, 0, byteData.Length, SocketFlags.None, result =>
            {
                try 
                {
                    socket.EndSend(result);
                    Debug.Log("Client -> " + data);
  
                    //reset.Set();
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
            int bytesRead = socket.EndReceive(result);
            
            Debug.Log("Client <- :: " + bytesRead + " bytes");
            
            if (bytesRead <= 0) return;
            
            String data = Encoding.ASCII.GetString(state.buffer, 0, bytesRead);
            Debug.Log("Client <- " + data);
                
            socket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, ReceiveCallback, state);
                //Socket client = state.socket;  
                
                /*int bytesRead = socket.EndReceive(result);  
                if (bytesRead > 0)
                {
                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
                    socket.BeginReceive(state.buffer,0,StateObject.BufferSize, 0, ReceiveCallback, state);  
                }
                else
                {
                    if (state.sb.Length > 1)
                    {  
                        //não sei pra q serve isso
                        //response = state.sb.ToString();

                        String data = Encoding.ASCII.GetString(state.buffer, 0, bytesRead);
                        Debug.Log("-> " + data);

                        if (data.StartsWith("CRD_OP"))
                        {
                            int id = Convert.ToInt32(data.Substring(7, 8));
                        }

                    }
                    
                    reset.Set();
                }*/
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
