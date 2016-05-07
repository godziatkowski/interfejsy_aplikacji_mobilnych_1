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
using System.Xml;
using Windows.Storage;

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
            return convertFileContentToListOfCurrency(fileContent, lastXML, token);

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
            return convertFileContentToListOfCurrency(fileContent, fileName.Trim() + ".xml", token);
        }

        private List<Currency> convertFileContentToListOfCurrency(String fileContent, String fileName, CancellationToken token)
        {
            XDocument xdoc = new XDocument();

            xdoc = XDocument.Parse(fileContent);
            var result = from elem in xdoc.Descendants("pozycja")
                         select new Currency
                         {
                             currencyName = (String)elem.Element("nazwa_waluty") ?? (String)elem.Element("nazwa_kraju"),
                             conversionRate = Int16.Parse((String)elem.Element("przelicznik")),
                             currencyAsPLN = Double.Parse(((String)elem.Element("kurs_sredni")).Replace(',', '.')),
                             currencyCode = (String)elem.Element("kod_waluty")
                         };
            //foreach( var record in result){
            //    System.Diagnostics.Debug.WriteLine("Currency Name: " + record.currencyName + " Conversion rate: "+ record.conversionRate + 
            //        " as PLN " + record.currencyAsPLN + " Currency code "+ record.currencyCode);
            //}
            //System.Diagnostics.Debug.WriteLine(result.ToList().Count);
            token.ThrowIfCancellationRequested();
            storeFile(fileName, fileContent);
            return result.ToList();
        }

        private async void storeFile(string fileName, String fileContent)
        {

            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            System.Diagnostics.Debug.WriteLine(storageFolder.DisplayName);
            StorageFile file = await storageFolder.CreateFileAsync( fileName, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(file, fileContent);

            DownloadedFileList.newFileDownloaded(fileName);

            storageFolder = ApplicationData.Current.LocalFolder;
                        
            StorageFile readedFile = await storageFolder.GetFileAsync(fileName);

            string readeedFileContent = await FileIO.ReadTextAsync(readedFile);



        }


    }
}
