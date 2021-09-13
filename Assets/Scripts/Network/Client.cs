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
        private static string ip = "26.158.168.172";
        private Thread thread;

        private static void Init()
        {
            byte[] bytes = new byte[1024];  
  
            try
            {
                IPHostEntry host = Dns.GetHostEntry(ip);
                IPAddress ipAddress = host.AddressList[0];
                
                Debug.Log("IPs = " + host.AddressList);
                
                IPEndPoint endPoint = new IPEndPoint(ipAddress, 11000);
       
                Socket socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(endPoint);  
      
                Debug.Log("Socket connected to {0}" + socket.RemoteEndPoint);
      
                    // Encode the data string into a byte array.    
                byte[] msg = Encoding.ASCII.GetBytes("This is a test<EOF>");  
      
                    // Send the data through the socket.    
                int bytesSent = socket.Send(msg);  
      
                    // Receive the response from the remote device.    
                int bytesRec = socket.Receive(bytes);
                
                Debug.Log("Echoed test = {0}" + Encoding.ASCII.GetString(bytes, 0, bytesRec));
      
                // Release the socket.
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }
        
        void Awake()
        {
            Debug.Log("Starting client");

            thread = new Thread(Init);
            thread.Start();
        }

        private void OnDisable()
        {
            thread.Abort();
        }
        
    }
}
