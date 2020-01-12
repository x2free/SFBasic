using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SFApiConsoleApp1
{
    class HttpListenerServer
    {
        HttpListener server;

        public void startServer(int port)
        {
            if (!HttpListener.IsSupported)
            {
                throw new Exception("This windows version does not support HttpListener.");
            }

            if (this.server == null)
            {
                server = new HttpListener();
                // server.Prefixes.Add(string.Format("http://localhost:{0}", port)); // error: only uri prefix supports
                server.Prefixes.Add(string.Format("http://localhost:{0}/", port));
            }

            server.Start();
            Console.WriteLine("Listening on port " + port);
        }

        public void handleRequest()
        {
            while (true)
            {
                HttpListenerContext context = server.GetContext();

                HttpListenerRequest requst = context.Request;
                HttpListenerResponse response = context.Response;

                string resBody = string.Format(@"<html>
                                        <head>
                                            <title>From Server</title>
                                        </head>
                                        <body>TcpListener@{0}</body>
                                    </html>", DateTime.Now);

                response.ContentLength64 = Encoding.UTF8.GetByteCount(resBody);
                response.ContentType = "text/html;charset=utf-8";
                StreamWriter sw = new StreamWriter(response.OutputStream);
                // sw.Write(Encoding.UTF8.GetBytes(resBody));
                sw.Write(resBody);
                sw.Close();
            }
        }

        public void stopServer()
        {
            server.Stop();
        }
    }
}
