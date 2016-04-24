using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace App1.Web
{
    class TxtDirDownload
    {
        private static readonly string baseUrl = "http://www.nbp.pl/kursy/xml/";
        private static readonly string latestDirFile = "dir.txt";
        private static readonly string defaultDateFormat = "dd-mm-yyyy";

        public async Task<Dictionary<DateTime, String>> downloadLatestDirFile()
        {

            HttpClient httpClient = new HttpClient();
            String fileContent = await httpClient.GetStringAsync(baseUrl + latestDirFile);
            return convertFileContentToDictionaryWithFileNamesWithTheirPublicationDates(fileContent);
        }

        public async Task<Dictionary<DateTime, string>> downloadDirFileWithName(String fileName)
        {

            HttpClient httpClient = new HttpClient();
            String fileContent = await httpClient.GetStringAsync(baseUrl + fileName);

            return convertFileContentToDictionaryWithFileNamesWithTheirPublicationDates(fileContent);
        }

        private Dictionary<DateTime, string> convertFileContentToDictionaryWithFileNamesWithTheirPublicationDates(String fileContent)
        {
            List<String> resultList = new List<String>(fileContent.Replace("\n", " ").Split(' '));
            resultList = resultList.Where(listItem => listItem.StartsWith("a")).ToList();

            Dictionary<DateTime, string> fileNamesWithPublicationDate = new Dictionary<DateTime, string>();
            foreach (var item in resultList)
            {
                string dateInfo = "20" + item.Split('z')[1];
                dateInfo = dateInfo.Substring(6, 2) + '-' + dateInfo.Substring(4, 2) + '-' + dateInfo.Substring(0, 4);
                try
                {
                    DateTime publicationDate = DateTime.ParseExact(dateInfo, defaultDateFormat, System.Globalization.CultureInfo.InvariantCulture);
                    fileNamesWithPublicationDate.Add(publicationDate, item);
                }
                catch (FormatException) {
                    System.Diagnostics.Debug.Write("Cannot parse " + dateInfo);
                }
            }
            return fileNamesWithPublicationDate;
        }
    }
}
