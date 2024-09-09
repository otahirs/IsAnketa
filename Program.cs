using System;
using System.Net;
using System.Net.Http;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System.Text.Json;

namespace isanketa
{
    class Program
    {
        static void Main(string[] args)
        {
            

            while(true) {
                 System.Console.WriteLine("Fetch (f) or Use (u) or Quit (q)");
                 switch(Console.ReadLine()){
                    case "f": 
                    case "F":
                        load("data.json",1431,32862);
                        break;
                    case "u":
                    case "U":
                        use("data.json");
                        break;
                    case "q":
                        return;
                    default:
                        break;
                }

            }    

        }
        static void use(string file) {
            var data = JsonSerializer.Deserialize<List<SurveyPage>>(File.ReadAllText(file));
            var data2 = data
                .Where(d => float.Parse(d.rozviji) < 2)
                .Where(d => float.Parse(d.vyborny) < 2)
                .Where(d => float.Parse(d.snadne) < 2)
                .OrderBy(d => d.rozviji);
                        ;
            foreach (var p in data2)
            {
                //System.Console.WriteLine($"{p.body.ToString("0.00")} - {p.jmeno} - {p.predmety}");
                System.Console.WriteLine($"{p.predmet}\nmedian hodin: {p.hodiny} | náročnost: {p.snadne} | rozviji: {p.rozviji} | kredity {p.kredity} | ucitel {p.vyborny} {p.ucitel} \n");
            }
 

        }
        static void load(string file, int fakulta=1433, int obdobi=32862) {
            HttpClientHandler handler = new HttpClientHandler();
            handler.CookieContainer = new CookieContainer();
            
            Utils.IsLogger(handler);

            var ids = Utils.getSurveyIds(handler, fakulta,obdobi).ToArray();
            var surveyPages = new List<SurveyPage>();

            Parallel.For(0, ids.Length,
                   index => { 
                       try {
                        var ch = new HttpClientHandler();
                        ch.CookieContainer = handler.CookieContainer;
                        var id = ids[index];
                        using (var client = new HttpClient(ch))
                        {
                                var resSession = client.GetAsync($"https://is.muni.cz/auth/pruzkumy/odpovedi?id={obdobi};vyber_objekt={id};vypis_odpovedi=1").Result;
                                resSession.EnsureSuccessStatusCode();

                                var html = resSession.Content.ReadAsStringAsync().Result;
                                try {     
                                    var page = new SurveyPage(html, obdobi.ToString());
                                    surveyPages.Add(page);
                                    System.Console.WriteLine($"{id} done");
                                }
                                catch {      
                                    System.Console.WriteLine($"{id} failed");
                                }
                                
                        }
                       }
                       catch {
                           System.Console.WriteLine("error");
                       }
                   } );
            
            handler.Dispose();
            var jsonString = JsonSerializer.Serialize(surveyPages);
            File.WriteAllText(file, jsonString);
        }
    }
}
