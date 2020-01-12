using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SFApiConsoleApp1
{
    class TcpListenerServer
    {
        private TcpListener server;
        public void startServer(int port)
        {
            if (this.server == null)
            {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, port);
                this.server = new TcpListener(endPoint);
            }

            this.server.Start();
            Console.WriteLine("Listening on port " + port);
        }

        public void handleRequest()
        {
            while (true)
            {
                TcpClient client = server.AcceptTcpClient();

                NetworkStream stream = client.GetStream();

                byte[] buffer = new byte[1024 * 4];
                int length = stream.Read(buffer, 0, 4096);

                if (length == 0)
                {
                    continue;
                }

                string reqString = Encoding.UTF8.GetString(buffer);
                Console.WriteLine("Request string: {0}", reqString);

                // response
                string statusLine = "HTTP/1.1 200 OK\r\n";
                string resBody = string.Format(@"<html>
                                        <head>
                                            <title>From Server</title>
                                        </head>
                                        <body>TcpListener@{0}</body>
                                    </html>", DateTime.Now);
                string resHeader = string.Format("Content-type:text/html;charset=utf-8\r\nContent-Lenght:{0}\r\n", resBody.Length);
                byte[] resBodyBytes = Encoding.UTF8.GetBytes(resBody);
                byte[] statusLineBytes = Encoding.UTF8.GetBytes(statusLine);
                byte[] resHeaderBytes = Encoding.UTF8.GetBytes(resHeader);

                stream.Write(statusLineBytes, 0, statusLineBytes.Length);
                stream.Write(resHeaderBytes, 0, resHeaderBytes.Length);
                stream.Write(new byte[] { 13, 10}, 0, 2); // ??
                stream.Write(resBodyBytes, 0, resBodyBytes.Length);

                client.Close();

                //if (Console.ReadKey().Key == ConsoleKey.Escape) {
                //    break;
                //}
            }
        }

        public void stopServer()
        {
            server.Stop();
        }
    }
}
