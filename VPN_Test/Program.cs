using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace VPN_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                IPAddress ipAd = IPAddress.Parse("172.20.10.14"); //IPv4 local

                /* Initializes the Listener */
                TcpListener myList = new TcpListener(ipAd, 8001);

                /* Start Listening at the specified port */
                myList.Start();

                Console.WriteLine("The server is running at port 8001...");
                Console.WriteLine("The local End point is  :" + myList.LocalEndpoint);
                Console.WriteLine("Waiting for a connection.....");

                /* Client connection */
                Socket s = myList.AcceptSocket();
                Console.WriteLine("Connection accepted from " + s.RemoteEndPoint);

                while (true)
                {
                    byte[] b = new byte[100];
                    int k = s.Receive(b);
                    string message = "";
                    for (int i = 0; i < k; i++)
                    {
                        message += Convert.ToChar(b[i]);
                    }
                    if (message.ToLower().Equals("x"))
                    {
                        ASCIIEncoding asen = new ASCIIEncoding();
                        s.Send(asen.GetBytes("x"));
                        break;
                    }
                    else
                        Console.WriteLine("\nRequest for VPN IP address.");
                    //send ip address
                    try
                    {
                        var vpn = NetworkInterface.GetAllNetworkInterfaces().First(x => x.Name == "TunnelBear");
                        var ip = vpn.GetIPProperties().UnicastAddresses.First(x => x.Address.AddressFamily == AddressFamily.InterNetwork).Address.ToString();
                        
                        ASCIIEncoding asen = new ASCIIEncoding();
                        s.Send(asen.GetBytes(ip));
                        Console.WriteLine("\nSent Acknowledgement");
                        OpenVPNServer(IPAddress.Parse(ip));
                        break;
                    }
                    catch (Exception e)
                    {
                        string str = "VPN is not activated.";
                        Console.WriteLine(e.Message);
                        ASCIIEncoding asen = new ASCIIEncoding();
                        s.Send(asen.GetBytes(str));
                        Console.WriteLine("\nSent Acknowledgement");
                    }

                };

                /* clean up */
                s.Close();
                myList.Stop();

            }
            catch (Exception e)
            {
                Console.WriteLine("Error..... " + e.Message);
            }

        }
        public static void OpenVPNServer(IPAddress ipAdr)
        {
            try
            {
                /* Initializes the Listener */
                TcpListener myList = new TcpListener(ipAdr, 8001);

                /* Start Listeneting at the specified port */
                myList.Start();

                Console.WriteLine("The server on VPN is running at port 8001...");
                Console.WriteLine("Waiting for a connection.....");

                Socket s = myList.AcceptSocket();
                Console.WriteLine("Connection accepted from " + s.RemoteEndPoint);

                while (true)
                {
                    byte[] b = new byte[100];
                    int k = s.Receive(b);
                    string message = "";
                    for (int i = 0; i < k; i++)
                    {
                        message += Convert.ToChar(b[i]);
                    }
                    if (message.ToLower().Equals("x"))
                    {
                        ASCIIEncoding asen = new ASCIIEncoding();
                        s.Send(asen.GetBytes("x"));
                        break;
                    }
                    else
                        Console.WriteLine(message);
                    
                    try
                    {
                        string ack = "Message succesfully transmitted.";
                        ASCIIEncoding asen = new ASCIIEncoding();
                        s.Send(asen.GetBytes(ack));
                        Console.WriteLine("\nSent Acknowledgement");
                    }
                    catch (Exception e)
                    {
                        string str = "Acknowledgment wasn't sent due to error: ";
                        Console.WriteLine(str + e.Message);
                    }
                };

                /* clean up */
                s.Close();
                myList.Stop();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error..... " + e.Message);
            }
        }
    }
}
