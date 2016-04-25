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
using System.Threading;

namespace App1.Web
{
    class CurrencyXMLDownload
    {
        private static readonly string baseUrl = "http://www.nbp.pl/kursy/xml/";
        private static readonly String lastXML = "LastA.xml";

        public async Task<List<Currency>> downloadLatestXML(CancellationToken token)
        {
            HttpClient httpClient = new HttpClient();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            token.ThrowIfCancellationRequested();
            byte[] bytes = await httpClient.GetByteArrayAsync(baseUrl + lastXML);
            token.ThrowIfCancellationRequested();            
            String fileContent = Encoding.GetEncoding("iso-8859-2").GetString(bytes, 0, bytes.Length);
            token.ThrowIfCancellationRequested();
            return convertFileContentToListOfCurrency(fileContent, token);

        }

        public async Task<List<Currency>> downloadFileWIthName(String fileName, CancellationToken token)
        {
            HttpClient httpClient = new HttpClient();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            token.ThrowIfCancellationRequested();
            byte[] bytes = await httpClient.GetByteArrayAsync(baseUrl + fileName.Trim() + ".xml");
            token.ThrowIfCancellationRequested();
            String fileContent = Encoding.GetEncoding("iso-8859-2").GetString(bytes, 0, bytes.Length);
            token.ThrowIfCancellationRequested();
            return convertFileContentToListOfCurrency(fileContent, token);
        }

        private List<Currency> convertFileContentToListOfCurrency(String fileContent, CancellationToken token)
        {
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
            //foreach( var record in result){
            //    System.Diagnostics.Debug.WriteLine("Currency Name: " + record.currencyName + " Conversion rate: "+ record.conversionRate + 
            //        " as PLN " + record.currencyAsPLN + " Currency code "+ record.currencyCode);
            //}
            //System.Diagnostics.Debug.WriteLine(result.ToList().Count);
            token.ThrowIfCancellationRequested();
            return result.ToList();
        }


    }
}
