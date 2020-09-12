using System;
using System.Net;
using System.Net.Http;

namespace isanketa
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            HttpClientHandler handler = new HttpClientHandler();
            handler.CookieContainer = new CookieContainer();
            
            Utils.IsLogger(handler);

            using (var client = new HttpClient(handler))
            {
                string baseUrl = "https://is.muni.cz/auth/";
                var resSession = client.GetAsync(baseUrl).Result;
                resSession.EnsureSuccessStatusCode();

                var html = resSession.Content.ReadAsStringAsync().Result;
                System.Console.WriteLine(html);

                //var doc = new HtmlDocument();
                //doc.LoadHtml(html);
               
            }
        }
    }
}
