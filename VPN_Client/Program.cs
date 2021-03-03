using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VPN_Client
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                TcpClient tcpclnt = new TcpClient();
                Console.WriteLine("Connecting.....");

                tcpclnt.Connect("192.168.1.31", 8001);  

                Console.WriteLine("Connected");
                while (true)
                {
                    Console.Write("Send vpn request? : Y/N [x - exit]");

                    String str = Console.ReadLine();
                    if (str.ToLower().Equals("n"))
                        continue;

                    Stream stm = tcpclnt.GetStream();

                    ASCIIEncoding asen = new ASCIIEncoding();
                    byte[] ba = asen.GetBytes(str);
                    Console.WriteLine("Transmitting.....");

                    stm.Write(ba, 0, ba.Length);

                    byte[] bb = new byte[100];
                    int k = stm.Read(bb, 0, 100);
                    string recv = "";
                    for (int i = 0; i < k; i++)
                        recv += Convert.ToChar(bb[i]);
                    if (recv.ToLower().Equals("x"))
                    {
                        tcpclnt.Close();
                        break;
                    }
                        
                    Console.WriteLine("VPN IPv4 address: " +recv);
                    Regex rgx = new Regex(@"((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)");
                    MatchCollection result = rgx.Matches(recv);
                    if(result.Count != 0)
                    {
                        tcpclnt.Close();
                        OpenVPNClient(recv);
                    }
                        
                }
                
            }

            catch (Exception e)
            {
                Console.WriteLine("Error..... " + e.Message);
            }
        }

        public static void OpenVPNClient(string ipAdr)
        {
            try
            {
                TcpClient tcpclnt = new TcpClient();
                Console.WriteLine("Connecting.....");

                tcpclnt.Connect(ipAdr, 8001); 

                Console.WriteLine("Connected");
                while (true)
                {

                    Console.Write("Send message [x - exit]");

                    String str = Console.ReadLine();

                    Stream stm = tcpclnt.GetStream();

                    ASCIIEncoding asen = new ASCIIEncoding();
                    byte[] ba = asen.GetBytes(str);
                    Console.WriteLine("Transmitting.....");

                    stm.Write(ba, 0, ba.Length);

                    byte[] bb = new byte[100];
                    int k = stm.Read(bb, 0, 100);
                    string recv = "";
                    for (int i = 0; i < k; i++)
                        recv += Convert.ToChar(bb[i]);
                    if (recv.ToLower().Equals("x"))
                        break;
                    Console.WriteLine(recv);
                }
                tcpclnt.Close();
            }

            catch (Exception e)
            {
                Console.WriteLine("Error..... " + e.Message);
            }
        }
    }
}
