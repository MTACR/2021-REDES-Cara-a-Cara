using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace Network
{
    public class Client : MonoBehaviour
    {
        // Start is called before the first frame update
        void Awake()
        {
            Debug.Log("Starting client");

            Thread trd = new Thread(Init);
            trd.Start();
        }

        private static void Init()
        {
            byte[] bytes = new byte[1024];  
  
            try
            {
                // Connect to a Remote server  
                // Get Host IP Address that is used to establish a connection  
                // In this case, we get one IP address of localhost that is IP : 127.0.0.1  
                // If a host has multiple addresses, you will get a list of addresses  
                IPHostEntry host = Dns.GetHostEntry("localhost");
                IPAddress ip = host.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ip, 2560);
      
                // Create a TCP/IP  socket.    
                Socket sender = new Socket(ip.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);
      
                // Connect the socket to the remote endpoint. Catch any errors.    
                try
                {
                    // Connect to Remote EndPoint  
                    sender.Connect(remoteEP);  
      
                    Debug.Log("Socket connected to {0}" +  
                        sender.RemoteEndPoint.ToString());
      
                    // Encode the data string into a byte array.    
                    byte[] msg = Encoding.ASCII.GetBytes("This is a test<EOF>");  
      
                    // Send the data through the socket.    
                    int bytesSent = sender.Send(msg);  
      
                    // Receive the response from the remote device.    
                    int bytesRec = sender.Receive(bytes);
                    Debug.Log("Echoed test = {0}" +
                        Encoding.ASCII.GetString(bytes, 0, bytesRec));
      
                    // Release the socket.
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();
      
                }
                catch (ArgumentNullException ane)
                {  
                    Debug.Log("ArgumentNullException : {0}" + ane.ToString());
                }
                catch (SocketException se)
                {
                    Debug.Log("SocketException : {0}" + se.ToString());
                }
                catch (Exception e)
                {
                    Debug.Log("Unexpected exception : {0}" + e.ToString());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
