using HtmlAgilityPack;

namespace isanketa
{
    public class SurveyPage
    {
        public string fakulta { get; set; }
        public string obdobi_is { get; set; }
        public string pruzkum { get; set; }
        public string predmet { get; set; }
        public string kredity { get; set; }
        public string hodiny { get; set; }
        public string rozviji { get; set; }
        public string snadne { get; set; }
        public string vyborny { get; set; }
        public string ucitel { get; set; }

        public SurveyPage(){}
        public SurveyPage(string html, string obdobi_is) {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var dom = doc.DocumentNode;
            fakulta = dom.SelectSingleNode("//input[@name='fakulta']").Attributes["value"].Value;
            this.obdobi_is = obdobi_is;
            pruzkum = dom.SelectSingleNode("//input[@name='id']").Attributes["value"].Value;
            predmet = dom.SelectSingleNode("//main//h3").InnerText;
            if(predmet.Contains(':'))
                predmet = predmet.Split(':')[1].Split(' ')[0];
            if (predmet.Contains('/'))
                predmet = predmet.Split('/')[0];
            
            ucitel = dom.SelectSingleNode("//main//h3").InnerText;
            if(ucitel.Contains('-'))
                ucitel = ucitel.Split('-')[1].Trim();
            kredity = dom.SelectSingleNode("//text()[contains(., 'počet kreditů tohoto předmětu')]/..").NextSibling.NextSibling.InnerText;
            hodiny = dom.SelectSingleNode("//text()[contains(., 'medián')]/..").NextSibling.NextSibling.InnerText;
            rozviji = dom.SelectSingleNode("//td[@title='1. Předmět pro mne má vzdělávací hodnotu, rozvíjí mne.']").ParentNode.LastChild.PreviousSibling.InnerText;
            snadne = dom.SelectSingleNode("//td[@title='FI 1. Předmět bylo velmi snadné absolvovat. (fakultní otázka)']").ParentNode.LastChild.PreviousSibling.InnerText;
            vyborny = dom.SelectSingleNode("//td[@title='FI 2. Učitel/ka je výborný/á pedagog/pedagožka a velmi dobře učí. (fakultní otázka)']").ParentNode.LastChild.PreviousSibling.InnerText;
        }

    }
}