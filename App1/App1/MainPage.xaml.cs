using App1.Web;
using App1.DataObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Threading;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace App1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        ObservableCollection<Currency> currencyList { get; set; }
        private Task<List<Currency>> downloadTask;
        private CancellationTokenSource cts;

        public MainPage()
        {
            this.InitializeComponent();
            loadData();
            //loadFileNamesFromCurrentYear();
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            loadData();
        }

        private async void loadData()
        {
            cts = new CancellationTokenSource();
            try
            {
                mylistbox.Visibility = Visibility.Collapsed;
                LoadingRing.Visibility = Visibility.Visible;
                LoadingRing.IsActive = true;
                downloadTask = new CurrencyXMLDownload().downloadLatestXML(cts.Token);
                await downloadTask;
                currencyList = new ObservableCollection<Currency>(downloadTask.Result);
                mylistbox.ItemsSource = currencyList;
            }
            catch (OperationCanceledException ex)
            {
            }
            finally
            {
                LoadingRing.IsActive = false;
                LoadingRing.Visibility = Visibility.Collapsed;
                mylistbox.Visibility = Visibility.Visible;
                cts = null;
                downloadTask = null;
            }
        }

        private async void loadFileNamesFromCurrentYear()
        {
            Task<Dictionary<DateTime, String>> downloadTask = new TxtDirDownload().downloadLatestDirFile();
            await downloadTask;
            List<DateTime> filesPubblicationDates = downloadTask.Result.Keys.ToList();

        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (downloadTask != null && cts != null && !downloadTask.IsCompleted)
            {
                cts.Cancel();
            }
            Application.Current.Exit();
        }

        private void ShowOtherFilesButton_Click(object sender, RoutedEventArgs e)
        {
            if (downloadTask != null && cts != null && !downloadTask.IsCompleted)
            {
                cts.Cancel();
            }
            this.Frame.Navigate(typeof(FileList));

        }
    }
}
