using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace App1.DataObjects
{
    class DownloadedFileList
    {
        private const char SEPARATOR = '|';
        private static HashSet<String> downloadedFileNames;
        private static readonly String FILE_NAME = "downloadedCurrencyFileNames.txt";

        public static void clearMe()
        {
            downloadedFileNames.Clear();
        }

        public static void newFileDownloaded(String fileName)
        {
            if (fileName != null && fileName != "")
            {
                downloadedFileNames.Add(fileName);

                saveToFile();
            }
        }

        public static Boolean isFileNameAlreadyDownloaded(String fileName)
        {
            return downloadedFileNames.Contains(fileName);
        }

        public static List<String> getFileNamesFromSpcifiedMonthInYear(string month, string year)
        {
            List<String> foundFileNames = new List<string>();
            string desiredDate = year.Substring(2, 2) + month;
            foreach (string filename in downloadedFileNames)
            {
                if (filename.Split('z')[1].StartsWith(desiredDate))
                {
                    foundFileNames.Add(filename);
                }
            }

            return foundFileNames;
        }

        public static List<String> getFileNamesFromSpecificYear(String year)
        {
            List<String> foundFileNames = new List<string>();
            string desiredDate = year.Substring(2, 2);
            foreach (string filename in downloadedFileNames)
            {
                if (filename.Split('z')[1].StartsWith(desiredDate))
                {
                    foundFileNames.Add(filename);
                }
            }

            return foundFileNames;
        }

        private static async void saveToFile()
        {
            StringBuilder sb = new StringBuilder();
            foreach (string fileName in downloadedFileNames)
            {
                sb.Append(fileName).Append(SEPARATOR);
            }
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            StorageFile file = await storageFolder.CreateFileAsync(FILE_NAME, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(file, sb.ToString());

            System.Diagnostics.Debug.WriteLine(sb.ToString());

            System.Diagnostics.Debug.WriteLine(downloadedFileNames.Count);
        }

        public static async void readListFromFile()
        {
            try
            {
                StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
                StorageFile file = await storageFolder.GetFileAsync(FILE_NAME);

                string fileContent = await FileIO.ReadTextAsync(file);

                downloadedFileNames = new HashSet<string>(fileContent.Split(SEPARATOR));

            }
            catch (FileNotFoundException ex)
            {
                downloadedFileNames = new HashSet<string>();
            }
        }
    }
}
