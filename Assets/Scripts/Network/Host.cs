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
        public string ip = "localhost";

        void Awake()
        {
            Debug.Log("Starting host");

            Thread trd = new Thread(Init);
            trd.Start();
        }

        private void Init()
        {
            IPHostEntry host = Dns.GetHostEntry(ip);
            IPAddress ipAddress = host.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 2560);

            try
            {
                // Create a Socket that will use Tcp protocol      
                Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                // A Socket must be associated with an endpoint using the Bind method  
                listener.Bind(localEndPoint);
                // Specify how many requests a Socket can listen before it gives Server busy response.  
                // We will listen 10 requests at a time  
                listener.Listen(10);

                Debug.Log("Waiting for a connection...");
                Socket handler = listener.Accept();

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

    }
}
