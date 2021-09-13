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
        private Thread thread;

        private static void Init()
        {
            try
            {
                IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddress = host.AddressList[0];
                
                Debug.Log("IPs = " + host.AddressList);
                
                IPEndPoint endPoint = new IPEndPoint(ipAddress, 11000);
                 
                Socket socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                socket.Bind(endPoint);
                socket.Listen(2);

                Debug.Log("Waiting for a connection...");
                Socket handler = socket.Accept();

                // Incoming data from the client.    
                string data = null;
                byte[] bytes = null;

                while (true)
                {
                    bytes = new byte[1024];
                    int bytesRec = handler.Receive(bytes);
                    data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                    if (data.IndexOf("<EOF>") > -1)
                    {
                        break;
                    }
                }

                Debug.Log("Text received : {0}" + data);

                byte[] msg = Encoding.ASCII.GetBytes(data);
                handler.Send(msg);
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }
        
        void Awake()
        {
            Debug.Log("Starting host");
            
            thread = new Thread(Init);
            thread.Start();
        }
        
        private void OnDisable()
        {
            thread.Abort();
        }

    }
}
