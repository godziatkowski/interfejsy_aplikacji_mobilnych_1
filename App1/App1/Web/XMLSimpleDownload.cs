using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using App1.DataObjects;
using System.IO;
using Windows.Data.Xml.Dom;

namespace App1.Web
{
    class XMLSimpleDownload
    {
        private static String lastXML = "http://www.nbp.pl/kursy/xml/LastA.xml";

        public static async Task<List<Currency>> downloadLatestXML() {
            HttpClient httpClient = new HttpClient();
            String fileContent = await httpClient.GetStringAsync(lastXML);            

            XDocument xdoc = new XDocument();

            xdoc = XDocument.Parse(fileContent);

            var result = from elem in xdoc.Descendants("pozycja")
                         select new Currency
                         {
                             currencyName = (String)elem.Element("nazwa_waluty"),
                             conversionRate = Int16.Parse((String)elem.Element("przelicznik")),
                             currencyAsPLN = Double.Parse((String)elem.Element("kurs_sredni")),
                             currencyCode = (String)elem.Element("kod_waluty")                             
                         };

            foreach( var record in result){
                System.Diagnostics.Debug.WriteLine("Currency Name: " + record.currencyName + " Conversion rate: "+ record.conversionRate + 
                    " as PLN " + record.currencyAsPLN + " Currency code "+ record.currencyCode);
            }
            System.Diagnostics.Debug.WriteLine(result.ToList().Count);
            return result.ToList();

        }

        
    }
}
