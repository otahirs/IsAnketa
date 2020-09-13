using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using HtmlAgilityPack;
using Newtonsoft.Json;
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
                    System.Console.WriteLine();
                    System.Console.Write("UCO: ");
                    var name = Console.ReadLine();
                    System.Console.Write("Primary pass: ");
                    var password = Console.ReadLine();
                    System.Console.WriteLine();

                    
                    var loginForm = new FormUrlEncodedContent(new Dictionary<string, string> { 
                        {"akce", "login"},
                        {"credential_0", name},
                        {"credential_1", password},
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

        public static IEnumerable<string> getSurveyIds(HttpClientHandler httpClientHandle, int facultaId, int obdobiId) {
            List<string> Ids = new List<string>();
            using (var client = new HttpClient(httpClientHandle, false))
            {
                var response = client.PostAsync(
                        new Uri($"https://is.muni.cz/auth/pruzkumy/odpovedi?id={obdobiId}"), 
                        new StringContent($"fakulta={facultaId}&obdobi=7644&id={14982}&objekty_moje_nebo_jine=objekty_jine&vyber_fakulta=1433&zmena_fakulta=Zm%C4%9Bnit&nacist_odpovedi_nectene=0&radit=objekty&tisk_soubor=pdf&nup=&pages=", 
                        Encoding.UTF8, 
                        "application/x-www-form-urlencoded")
                    ).Result;
                response.EnsureSuccessStatusCode(); 

                var html = response.Content.ReadAsStringAsync().Result;
                var doc = new HtmlDocument();
                doc.LoadHtml(html);
                foreach(var node in doc.DocumentNode.SelectNodes("//input[@name='vyber_objekt']")) {
                    if(!node.ParentNode.NextSibling.HasClass("nedurazne"))
                        Ids.Add(node.Attributes["value"].Value);
                }
               
            }
            return Ids.Where(id => id != "1").Distinct();
        }
        
    }
}