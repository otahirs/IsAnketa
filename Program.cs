using System;
using System.Net;
using System.Net.Http;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;

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
                        load();
                        break;
                    case "u":
                    case "U":
                        use();
                        break;
                    case "q":
                        return;
                    default:
                        break;
                }

            }    

        }
        static void use() {
            var data = JsonConvert.DeserializeObject<List<SurveyPage>>(File.ReadAllText("fi.json"));
            var data2 = data
                        .GroupBy(p => p.ucitel)
                        .Select(g => new {jmeno = g.Key,body = g.Aggregate(
                            0.0,
                            (sum, next) => sum + float.Parse(next.vyborny),
                            f => f / g.ToArray().Length), predmety = string.Join(", ", g.Select(p => p.predmet))}
                            ).OrderBy(h => h.body)
                        ;
            foreach (var p in data2)
            {
                System.Console.WriteLine($"{p.body.ToString("0.00")} - {p.jmeno} - {p.predmety}");
                //System.Console.WriteLine($"{p.predmet}\nmedian hodin: {p.hodiny} | náročnost: {p.snadne} | kredity {p.kredity}\n");
            }
 

        }
        static void load(int fakulta=1433, int obdobi=14982) {
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
                                    var page = new SurveyPage(html);
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
            var jsonString = JsonConvert.SerializeObject(surveyPages);
            File.WriteAllText("fi.json", jsonString);
        }
    }
}
