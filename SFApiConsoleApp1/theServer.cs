using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFApiConsoleApp1
{
    class theServer
    {
        System.Net.Sockets.TcpListener myListener;
        //start listing on the given port
        int port = 9286;

        static System.Net.HttpListener _httpListener = new System.Net.HttpListener();
        public static void test(string serverURL)
        {
            Console.WriteLine("Starting server...");
            _httpListener.Prefixes.Add(serverURL + "/"); // add prefix "http://localhost:5000/"
            _httpListener.Start(); // start server (Run application as Administrator!)
            Console.WriteLine("Server started.");
            System.Threading.Thread _responseThread = new System.Threading.Thread(ResponseThread);
            _responseThread.Start(); // start the response thread
        }

        static void ResponseThread()
        {
            while (true)
            {
                System.Net.HttpListenerContext context = _httpListener.GetContext(); // get a context
                // grant_type
                //string authorization_url = "/services/oauth2/authorize";
                string token_url = "/services/oauth2/token";
                //string revoke_url = "/services/oauth2/revoke";
                string baseUrl = "https://login.salesforce.com";
                string client_id = "3MVG9YDQS5WtC11o5Mbm9Am1IBP7MyithezCXauojL8lCuh42psSRB4CRxCxQ8BcWpzZMOvvnPi6oQioIO8Ot";
                //string redirect_url = "http://localhost:9286/token";
                string redirect_url = "http://localhost:9286";
                string client_secret = "158FF1F4FBE35220BB658C5BFF30771CE2D9FF7F6CAF11925984956C184C20F8";
                /*
                grant_type = authorization_code & code = aPrxsmIEeqM9PiQroGEWx1UiMQd95_5JUZ
VEhsOFhS8EVvbfYBBJli2W5fn3zbo.8hojaNW_1g % 3D % 3D & client_id = 3MVG9lKcPoNI
        NVBIPJjdw1J9LLM82HnFVVX19KY1uA5mu0QqEWhqKpoW3svG3XHrXDiCQjK1mdgAvhCs
cA9GE & client_secret = 1955279925675241571 &
redirect_uri = https % 3A % 2F % 2Fwww.mysite.com % 2Fcode_callback.jsp
*/

                // string code = context.Request
                bool isTrue = context.Request.Url.ToString().EndsWith("token");
                string code = context.Request.QueryString["code"];
                string state = context.Request.QueryString["state"];
                string domainInstanceUri = context.Request.UrlReferrer == null || string.IsNullOrEmpty(context.Request.UrlReferrer.AbsoluteUri)
                    ? baseUrl : context.Request.UrlReferrer.AbsoluteUri;
                domainInstanceUri = domainInstanceUri.TrimEnd('/');
                //if (isTrue)
                //{
                //    // context.Response.Headers["method"] = "POST";
                //    string responseContent = string.Format("grant_type=authorization_code&code={0}&client_id={1}&client_secret={2}&redirect_uri={3}"
                //                    , "", client_id, client_secret, redirect_url);
                //    // Now, you'll find the request URL in context.Request.Url
                //    byte[] _responseArray = Encoding.UTF8.GetBytes(responseContent); // get the bytes to response
                //    //context.Response.OutputStream.Write(_responseArray, 0, _responseArray.Length); // write bytes to the output stream
                //    //context.Response.KeepAlive = false; // set the KeepAlive bool to false
                //    //context.Response.Close(); // close the connection
                //    //Console.WriteLine("Respone given to a request.");
                //}
                /*
                Dictionary<String, String> dic = new Dictionary<string, string>();
                dic.Add("grant_type", "authorization_code");
                dic.Add("client_id", client_id);
                dic.Add("client_secret", client_secret);
                dic.Add("redirect_uri", redirect_url);

                System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
                System.Net.Http.HttpContent content = new System.Net.Http.FormUrlEncodedContent(dic);
                var response = await client.PostAsync(baseUrl + token_url, content);
                */
                isTrue = string.Compare("one", state, true) == 0;
                if (isTrue)
                {
                    System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls
                        | System.Net.SecurityProtocolType.Tls11
                        | System.Net.SecurityProtocolType.Tls12;

                    //string postData = string.Format("grant_type=authorization_code&code={0}&client_id={1}&client_secret={2}&redirect_uri={3}"
                    //                    , System.Net.WebUtility.UrlEncode(code), client_id, client_secret, System.Net.WebUtility.UrlEncode(redirect_url));
                    string postData = string.Format("grant_type=authorization_code&code={0}&&redirect_uri={1}"
                                       , code, System.Net.WebUtility.UrlEncode(redirect_url));
                    var request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(baseUrl + token_url);
                    // var request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(domainInstanceUri + token_url);

                    var data = Encoding.UTF8.GetBytes(postData);

                    //foreach (var key in context.Request.Headers.AllKeys)
                    //{
                    //    request.Headers[key] = context.Request.Headers[key];
                    //}

                    request.Method = "POST";
                    request.ContentType = "application/x-www-form-urlencoded";// grant type not supported
                    request.ContentLength = data.Length;
                    //request.Accept = "application/json;charset=UTF-8";
                    request.Accept = "application/x-www-form-urlencoded"; 

                    // request.Headers.Add("Authorization", string.Format("Basic client_id={0}&client_secret={1}", client_id, client_secret));
                    string basicCredential = string.Format("Basic {0}", Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", client_id, client_secret))));
                    request.Headers.Add("Authorization", basicCredential);

                    using (var stream = request.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }

                    try
                    {
                        var response = (System.Net.HttpWebResponse)request.GetResponse();
                        var responseString = new System.IO.StreamReader(response.GetResponseStream()).ReadToEnd();
                        Console.WriteLine("response: " + responseString);

                        string msg = "Authentication is done, you may close browser now.";
                        var responseMsg = Encoding.UTF8.GetBytes(msg);

                        context.Response.OutputStream.Write(responseMsg, 0, responseMsg.Length);
                    }
                    catch (System.Net.WebException ex)
                    {
                        if (ex.Response != null) {
                            string content = new System.IO.StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                            Console.WriteLine(content);
                        }
                        //throw ex;
                    }
                }
            }
        }

        public void startServer()
        {
            myListener = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Parse("127.0.0.1"), port);

            myListener.Start();
        }

        public void StartListen()
        {

            int iStartPos = 0;
            String sRequest;
            String sDirName;
            String sRequestedFile;

            //String sErrorMessage;

            //String sLocalDir;

            //String sMyWebServerRoot = "C:\\MyWebServerRoot\\";

            //String sPhysicalFilePath = "";

            //String sFormattedMessage = "";

            //String sResponse = "";

            while (true)
            {

                //Accept a new connection  
                System.Net.Sockets.Socket mySocket = myListener.AcceptSocket();
                Console.WriteLine("Socket Type " + mySocket.SocketType);

                if (mySocket.Connected)
                {
                    Console.WriteLine(@"\nClient Connected!!\n==================\n CLient IP {0}\n", mySocket.RemoteEndPoint);

                    //make a byte array and receive data from the client

                    if (mySocket.Available == 0)
                    {
                        break;
                    }

                    Byte[] bReceive = new Byte[1024];
                    int i = mySocket.Receive(bReceive, bReceive.Length, 0);
                    //Convert Byte to String  
                    string sBuffer = Encoding.UTF8.GetString(bReceive);

                    ////At present we will only deal with GET type
                    //if (sBuffer.Substring(0, 3) != "GET")
                    //{
                    //    Console.WriteLine("Only Get Method is supported..");
                    //    mySocket.Close();
                    //    break;
                    //}

                    // Look for HTTP request  
                    iStartPos = sBuffer.IndexOf("HTTP", 1);
                    // Get the HTTP text and version e.g. it will return "HTTP/1.1"
                    string sHttpVersion = sBuffer.Substring(iStartPos, 8);
                    // Extract the Requested Type and Requested file/directory  

                    sRequest = sBuffer.Substring(0, iStartPos - 1);
                    //Replace backslash with Forward Slash, if Any  

                    sRequest.Replace("\\", "/");
                    //If file name is not supplied add forward slash to indicate   
                    //that it is a directory and then we will look for the   

                    //default file name..  
                    if ((sRequest.IndexOf(".") < 1) && (!sRequest.EndsWith("/")))
                    {
                        sRequest = sRequest + "/";
                    }

                    //Extract the requested file name  
                    iStartPos = sRequest.LastIndexOf("/") + 1;
                    sRequestedFile = sRequest.Substring(iStartPos);

                    //Extract The directory Name
                    sDirName = sRequest.Substring(sRequest.IndexOf("/"), sRequest.LastIndexOf("/") - 3);
                }
            }
        }
    }
}
