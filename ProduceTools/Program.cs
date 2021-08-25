using System.Xml.Linq;
using System.Net.Http;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ProduceTools
{
    class Program
    {
        static void Main(string[] args)
        {
            string url = "https://o365exchange.visualstudio.com/O365%20Core/_workitems/assignedtome/";
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(
                    string.Format("{0}:{1}", "", ""))));
                using (HttpResponseMessage responseMessage = client.GetAsync(url).Result)
                {
                    responseMessage.EnsureSuccessStatusCode();
                    string responseString = responseMessage.Content.ReadAsStringAsync().Result;
                    XDocument xml = XDocument.Load(new StringReader(responseString));
                    string version = xml.Root.Elements()
                        .Where(x => x.Name.LocalName == "ItemGroup").Elements()
                        .Where(x => x.Name.LocalName == "PackageReference")
                        .FirstOrDefault(x => x.Attribute("Update").Value.Equals("Microsoft.Exchange.MapiAbstraction", StringComparison.OrdinalIgnoreCase)).Attribute("Version").Value;
                    version = Regex.Match(version, @"\[(.*)\]").Groups[1].Value;
                    System.Console.WriteLine(version);
                }
            }
        }
    }
}
