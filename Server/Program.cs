using System;
using System.Text;
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
                var addr = IPAddress.Parse("127.0.0.1");
                server = new TcpListener(addr, port);
                server.Start();
                Console.WriteLine("[*] Server started. Waiting for connections.");

                while (true)
                {
                    TcpClient client = server.AcceptTcpClient();
                    var clientAddress = Convert.ToString(((IPEndPoint)client.Client.RemoteEndPoint).Address);
                    Console.WriteLine($"[*] Client connected: {clientAddress}");

                    NetworkStream stream = client.GetStream();

                    byte[] data = new byte[2048];

                    while (true)
                    {
                        var closeFlag = false;

                        stream.Read(data, 0, data.Length);
                        var length = new UnicodeEncoding().GetString(data);
                        length = length.Substring(0, 1);

                        stream.Read(data, 0, data.Length);
                        string cmd = new UnicodeEncoding().GetString(data);
                        cmd = cmd.Substring(0, Convert.ToInt32(length));

                        switch (cmd)
                        {
                            case "Sign":
                                Console.WriteLine(Environment.NewLine, "Sign action");
                                stream.Read(data, 0, data.Length);
                                CryptoActions.VerifySign(data);
                                break;

                            case "Encrypt":
                                Console.WriteLine(Environment.NewLine, "Encrypt Action");
                                stream.Read(data, 0, data.Length);
                                byte[] decryptedData = CryptoActions.Decrypt(data);
                                CryptoActions.VerifyMsg(decryptedData, out byte[] msg);
                                Console.WriteLine($"\tDecrpyted message: {new UnicodeEncoding().GetString(msg)}");
                                break;

                            case "Close":
                                stream.Close();
                                client.Close();
                                Console.WriteLine($"[*] Client disconnected: {clientAddress}");
                                closeFlag = true;
                                break;

                            default:
                                break;
                        }

                        if (closeFlag)
                        {
                            break;
                        }
                    }
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
