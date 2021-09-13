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

                socket.BeginConnect(endPoint, result =>
                {
                    try {
                        reset.Set();
                        socket.EndConnect(result);
  
                        Debug.Log("Connected to " + socket.RemoteEndPoint);
                        
                        StateObject state = new StateObject {socket = socket};
                        
                        Send(socket, "TESTE <EOF>");
                        
                        socket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, ReceiveCallback, state);
                    } catch (Exception e) {  
                        Console.WriteLine(e.ToString());  
                    }  
                }, socket);

                reset.WaitOne();

                /* Release the socket.
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();*/
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }
        
        private static void Send(Socket client, String data) 
        {
            byte[] byteData = Encoding.ASCII.GetBytes(data);  
  
            client.BeginSend(byteData, 0, byteData.Length, SocketFlags.None, result =>
            {
                try 
                {
                    client.EndSend(result);
                    Debug.Log("-> " + data);
  
                    reset.Set();  
                } 
                catch (Exception e) 
                {  
                    Debug.LogError(e.ToString());  
                }
            }, client);  
        }

        private static void ReceiveCallback(IAsyncResult result) 
        {  
            try 
            {
                StateObject state = (StateObject) result.AsyncState;  
                Socket client = state.socket;  
                
                int bytesRead = client.EndReceive(result);  
                if (bytesRead > 0)
                {
                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
                    client.BeginReceive(state.buffer,0,StateObject.BufferSize, 0, ReceiveCallback, state);  
                } 
                else 
                {
                    if (state.sb.Length > 1)
                    {  
                        //não sei pra q serve isso
                        //response = state.sb.ToString();
                        
                        Debug.Log("-> " + Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
                    }
                    
                    reset.Set();  
                }
            } 
            catch (Exception e) 
            {  
                Console.WriteLine(e.ToString());  
            }  
        } 

        void Awake()
        {
            Debug.Log("Starting client...");

            thread = new Thread(Init) {IsBackground = true};
            thread.Start();
        }

        private void OnDisable()
        {
            thread.Abort();
        }
        
    }
}
