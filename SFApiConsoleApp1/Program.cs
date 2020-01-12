using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SFApiConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            string id = "https://login.salesforce.com/id/00D6F000002WOdlUAG/0056F00000ACTvNQAX";
            string pt = @"id/(?<oId>\w+)*?/(?<uId>\S+)";
            Regex r0 = new Regex(pt);
            Match m0 = r0.Match(id);


            string searchString = "Title: {MyTitle} Incident Description: {MyDescription} Incident Level: {MyLevel}";
            string theValue = string.Empty;
            string theDescription = string.Empty;
            string theLevel = string.Empty;
            string patternn = @"\{(?<myvalue>\w+)\}.+:\s*\{(?<mydescription>\w+)\}.+:\s*\{(?<mylevel>\w+)\}"; // continue the pattern for your needs
            Regex rxx = new Regex(patternn);

            Match mm = rxx.Match(searchString);

            if (mm.Success)
            {
                theValue = mm.Groups["myvalue"].Value;
                theDescription = mm.Groups["mydescription"].Value;
                theLevel = mm.Groups["mylevel"].Value;
            }


            string resBody = @"GET /?code=aPrxFYRxG6g7t8rt1SrG_MgvPAgbyN8Y8OplaQpb7.83GhHmj_8aWlxa0ZOIsWMCc6n3u7oQZw%3D%3D&state=one HTTP/1.1
                                Host: localhost:9286
                                Connection: keep-alive
                                Upgrade-Insecure-Requests: 1
                                User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/79.0.3945.117 Safari/537.36
                                Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9
                                Sec-Fetch-Site: cross-site
                                Sec-Fetch-Mode: navigate
                                Referer: https://tctestdomain-dev-ed.my.salesforce.com/
                                Accept-Encoding: gzip, deflate, br
                                Accept-Language: en-US,en;q=0.9
                                Cookie: MicrosoftApplicationsTelemetryDeviceId=1ff623dc-d655-4d22-a9f0-9c4d29ea8889; MicrosoftApplicationsTelemetryFirstLaunchTime=2019-08-12T14:04:24.338Z; _csrf=O2IQGLMwDM33J7r9q35iQlGG";

            // string pattern = @"code=.*(\s*)";
            // string pattern = @"code=.* ";
            // string pattern = @"code=.*[\s]$";
            // string pattern = @"\{(?<code>\w+)\}.+:\s*\{(?<state>\w+)\}.+:\s*\{(?<Referer>\w+)\}";
            // string pattern = @"code=(?<code>.*)(&state=(?<state>.*))?\S*";
            // string pattern = @"code=(?<code>.*)(&|\S)";
            // string pattern = @"code=(?<code>.*)(&| )";
            // string pattern = @"code=(?<code>\S*)&state=(?<state>\S*)";
            // string pattern = @"code=(?<code>\S*?)( |&state=(?<state>\S*))";
            string pattern = @"code=(?<code>\S*?)( |&state=(?<state>\S*)).+?\sReferer: (?<referer>\S*)";
            Regex rx = new Regex(pattern, RegexOptions.Singleline);
            Match m = rx.Match(resBody);

            if (m.Success)
            {
                string code = m.Groups["code"].Success ? m.Groups["code"].Value : "";
                string state = m.Groups["state"].Success ? m.Groups["state"].Value : "";
                string referer = m.Groups["referer"].Success ? m.Groups["referer"].Value : "";
            }


            pattern = @"Referer: .*";
            rx = new Regex(pattern);
            Match m2 = rx.Match(resBody);



            HttpListenerServer ts = new HttpListenerServer();
            ts.startServer(9286);
            ts.handleRequest();

            string authorization_url = "/services/oauth2/authorize";
            //string token_url = "/services/oauth2/token";
            //string revoke_url = "/services/oauth2/revoke";
            string baseUrl = "https://login.salesforce.com";
            string client_id = "3MVG9YDQS5WtC11o5Mbm9Am1IBP7MyithezCXauojL8lCuh42psSRB4CRxCxQ8BcWpzZMOvvnPi6oQioIO8Ot";
            string redirect_url = "http://localhost:9286";

            /*
                https://login.salesforce.com/services/oauth2/authorize?response_type=code
                &client_id=3MVG9lKcPoNINVBIPJjdw1J9LLM82HnFVVX19KY1uA5mu0QqEWhqKpoW3svG3X
                HrXDiCQjK1mdgAvhCscA9GE&redirect_uri=https%3A%2F%2Fwww.mysite.com%2F
                code_callback.jsp&state=mystate 
             */

            //theServer server = new theServer();
            //server.startServer();
            //System.Threading.Thread thread = new System.Threading.Thread(server.StartListen);
            //thread.Start();

            theServer.test(redirect_url);

            string getTookenReqUrl = string.Format("{0}{1}?response_type=code&client_id={2}&redirect_uri={3}&state={4}"
                        , baseUrl, authorization_url
                        , client_id, redirect_url
                        , "one");

            System.Diagnostics.Process.Start(getTookenReqUrl);
        }
    }
}
