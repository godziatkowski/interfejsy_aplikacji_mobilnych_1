using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App1.Web
{
    class TxtDirDownload
    {
        private static readonly string baseUrl = "http://www.nbp.pl/kursy/xml/";
        private static readonly string latestDirFile = "dir.txt";

        public async Task<Dictionary<string, String>> downloadLatestDirFile(CancellationToken token)
        {

            HttpClient httpClient = new HttpClient();
            String fileContent = await httpClient.GetStringAsync(baseUrl + latestDirFile);
            token.ThrowIfCancellationRequested();

            return convertFileContentToDictionaryWithFileNamesWithTheirPublicationDates(fileContent, token);
        }

        public async Task<Dictionary<string, string>> downloadDirFileWithName(String fileName, CancellationToken token)
        {

            HttpClient httpClient = new HttpClient();
            String fileContent = await httpClient.GetStringAsync(baseUrl + "dir" + fileName + ".txt");
            token.ThrowIfCancellationRequested();
            return convertFileContentToDictionaryWithFileNamesWithTheirPublicationDates(fileContent, token);
        }

        private Dictionary<string, string> convertFileContentToDictionaryWithFileNamesWithTheirPublicationDates(String fileContent, CancellationToken token)
        {
            List<String> resultList = new List<String>(fileContent.Replace("\n", " ").Split(' '));
            token.ThrowIfCancellationRequested();
            resultList = resultList.Where(listItem => listItem.StartsWith("a")).ToList();

            Dictionary<string, string> fileNamesWithPublicationDate = new Dictionary<string, string>();
            token.ThrowIfCancellationRequested();
            foreach (var item in resultList)
            {
                string dateInfo = "20" + item.Split('z')[1];
                dateInfo = dateInfo.Substring(6, 2) + '-' + dateInfo.Substring(4, 2) + '-' + dateInfo.Substring(0, 4);
                String publicationDate = dateInfo;
                fileNamesWithPublicationDate.Add(publicationDate, item);
                
                token.ThrowIfCancellationRequested();
            }
            return fileNamesWithPublicationDate;
        }
    }
}
