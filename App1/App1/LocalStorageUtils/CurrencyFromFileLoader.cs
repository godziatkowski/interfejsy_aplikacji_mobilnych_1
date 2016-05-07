using App1.DataObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Storage;

namespace App1.LocalStorageUtils
{
    class CurrencyFromFileLoader
    {
        public static readonly string LAST_CURRENCY_FILE_NAME = "LastA.xml";

        public async Task<List<Currency>> loadCurrencyFromFile(string fileName, CancellationToken token)
        {

            try
            {
                StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
                token.ThrowIfCancellationRequested();
                System.Diagnostics.Debug.WriteLine(storageFolder.DisplayName);
                System.Diagnostics.Debug.WriteLine(fileName + ".xml");
                StorageFile file = await storageFolder.GetFileAsync(fileName+ ".xml");

                token.ThrowIfCancellationRequested();
                string fileContent = await FileIO.ReadTextAsync(file);

                token.ThrowIfCancellationRequested();
                return convertFileContentToListOfCurrency(fileContent, token);
            }
            catch (FileNotFoundException ex)
            {
                return new List<Currency>();
            }
        }

        public async Task<List<Currency>> loadLatestCurrencyFile(CancellationToken token)
        {
            try
            {
                StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
                token.ThrowIfCancellationRequested();
                StorageFile file = await storageFolder.GetFileAsync(LAST_CURRENCY_FILE_NAME);

                token.ThrowIfCancellationRequested();
                string fileContent = await FileIO.ReadTextAsync(file);

                token.ThrowIfCancellationRequested();
                return convertFileContentToListOfCurrency(fileContent, token);
            }
            catch (FileNotFoundException ex)
            {
                return new List<Currency>();
            }
        }

        private List<Currency> convertFileContentToListOfCurrency(String fileContent, CancellationToken token)
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
            return result.ToList();
        }
    }
}
