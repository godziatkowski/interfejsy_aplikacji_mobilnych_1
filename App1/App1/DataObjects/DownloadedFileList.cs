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
            if (fileName != null && fileName.Trim() != "")
            {
                downloadedFileNames.Add(fileName);

                saveToFile();
            }
        }

        public static Boolean isFileNameAlreadyDownloaded(String fileName)
        {
            return downloadedFileNames.Contains(fileName);
        }

        public static List<String> getFileNamesPublishedBetweenDates(DateTime from, DateTime to)
        {
            List<String> foundFileNames = new List<string>();
            foreach (string filename in downloadedFileNames)
            {
                string[] splittedFileName = filename.Split('z');
                if (splittedFileName.Length == 1)
                {
                    continue;
                }

                string datePart = splittedFileName[1];
                DateTime filePublicationDate = new DateTime(Int16.Parse(datePart.Substring(0, 2)), Int16.Parse(datePart.Substring(2, 2)), Int16.Parse(datePart.Substring(4, 2)));
                if (from.Ticks < filePublicationDate.Ticks && filePublicationDate.Ticks < to.Ticks)
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

            //System.Diagnostics.Debug.WriteLine(sb.ToString());

            //System.Diagnostics.Debug.WriteLine(downloadedFileNames.Count);
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
