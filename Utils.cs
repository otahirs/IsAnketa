using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using HtmlAgilityPack;
using Newtonsoft.Json;

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
        
        
    }
}