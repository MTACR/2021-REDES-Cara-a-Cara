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
                socket.Connect(endPoint);  
      
                Debug.Log("Socket connected to " + socket.RemoteEndPoint);
      
                // Encode the data string into a byte array.    
                byte[] msg = Encoding.ASCII.GetBytes("This is a test<EOF>");  
      
                // Send the data through the socket.    
                int bytesSent = socket.Send(msg);  
      
                byte[] bytes = new byte[1024];
                // Receive the response from the remote device.    
                int bytesRec = socket.Receive(bytes);
                
                Debug.Log("Echoed test = " + Encoding.ASCII.GetString(bytes, 0, bytesRec));
      
                // Release the socket.
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }
        
        void Awake()
        {
            Debug.Log("Starting client");

            thread = new Thread(Init) {IsBackground = true};
            thread.Start();
        }

        private void OnDisable()
        {
            thread.Abort();
        }
        
    }
}
