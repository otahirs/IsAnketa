using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using HtmlAgilityPack;
using System.Linq;

namespace isanketa
{
    public static class Utils
    {
        public static void IsLogger(HttpClientHandler httpClientHandle) {
            string baseUrl = "https://is.muni.cz/auth/";

            using (var client = new HttpClient(httpClientHandle, false))
            {
                client.BaseAddress = new Uri(baseUrl);

                var resLoginPage = client.GetAsync(baseUrl).Result;
                resLoginPage.EnsureSuccessStatusCode();
                var loginUrl = resLoginPage.RequestMessage.RequestUri;

                while(true) {
                    // System.Console.WriteLine();
                    // System.Console.Write("UCO: ");
                    // var name = Console.ReadLine();
                    // System.Console.Write("Primary pass: ");
                    // var password = Console.ReadLine();
                    // System.Console.WriteLine();

                    
                    var loginForm = new FormUrlEncodedContent(new Dictionary<string, string> { 
                        {"akce", "login"},
                        {"credential_0", "424242"},// name},
                        {"credential_1", "superSecretPass"}, //password},
                        {"uloz", "uloz"}
                    });
                    
                    var resLogin = client.PostAsync(loginUrl, loginForm).Result;
                    resLogin.EnsureSuccessStatusCode(); 

                    if(resLogin.RequestMessage.RequestUri.AbsoluteUri == baseUrl)
                        break;
                    System.Console.WriteLine("Invalid credentials, try again.");
                }

               
            }

        }

        public static IEnumerable<string> getSurveyIds(HttpClientHandler httpClientHandle, int faculty, int survey) {
            List<string> Ids = new List<string>();
            using (var client = new HttpClient(httpClientHandle, false))
            {
                var response = client.PostAsync(
                        new Uri($"https://is.muni.cz/auth/pruzkumy/odpovedi"), 
                        new StringContent($"fakulta={faculty}&id={survey}&objekty_moje_nebo_jine=objekty_jine",
                        Encoding.UTF8, 
                        "application/x-www-form-urlencoded")
                    ).Result;
                response.EnsureSuccessStatusCode(); 

                var html = response.Content.ReadAsStringAsync().Result;
                var doc = new HtmlDocument();
                doc.LoadHtml(html);
                foreach(var node in doc.DocumentNode.SelectNodes("//input[@name='vyber_objekt' and not(@disabled='1') and not(@value='1')]")) {
                    Ids.Add(node.Attributes["value"].Value);
                }
               
            }
            return Ids;
        }
        
    }
}