using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var port = Convert.ToInt32(args[0]);

            TcpListener server = null;
            try
            {
                IPAddress addr = IPAddress.Parse("127.0.0.1");
                server = new TcpListener(addr, port);
                server.Start();
                Console.WriteLine("Server started. Waiting for connections.");

                TcpClient client = server.AcceptTcpClient();
                var clientAddress = Convert.ToString(((IPEndPoint)client.Client.RemoteEndPoint).Address);
                Console.WriteLine("Client connected");

                while (true)
                {
                    


                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                if (server != null)
                {
                    server.Stop();
                }
            }
        }
    }
}
