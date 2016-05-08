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
using Windows.Networking.Connectivity;
using App1.LocalStorageUtils;
using App1.Views;

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
        private string nameOfDisplayedFile;

        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            String param = e.Parameter as string;
            if (param != null && param != "" && param != "currentData")
            {
                if (downloadTask != null && cts != null && !downloadTask.IsCompleted)
                {
                    cts.Cancel();
                }
                string stringDate = "20" + param.Split('z')[1];
                DisplayedPublicationDate.Text = stringDate.Substring(4, 2) + "." + stringDate.Substring(6, 2) + "." + stringDate.Substring(0, 4);
                loadData(param);
            }
            else {
                if (downloadTask != null && cts != null && !downloadTask.IsCompleted)
                {
                    cts.Cancel();
                }
                DisplayedPublicationDate.Text = DateTime.Today.ToString("dd/MM/yyyy").Replace("/", ".");
                loadData();
            }
        }

        private bool hasInternetConnection()
        {
            var connectionProfile = NetworkInformation.GetInternetConnectionProfile();
            return (connectionProfile != null &&
                    connectionProfile.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //DownloadedFileList.clearMe();
            DisplayedPublicationDate.Text = DateTime.Today.ToString("dd/MM/yyyy").Replace("/", ".");
            loadData();
        }

        private async void loadData()
        {
            nameOfDisplayedFile = "currentData";
            if (hasInternetConnection())
            {
                internetConnectionStatus.Visibility = Visibility.Collapsed;
                noDataToDisplay.Visibility = Visibility.Collapsed;
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
            else {
                internetConnectionStatus.Visibility = Visibility.Visible;
                this.nameOfDisplayedFile = "currentData";
                cts = new CancellationTokenSource();
                try
                {
                    mylistbox.Visibility = Visibility.Collapsed;
                    LoadingRing.Visibility = Visibility.Visible;
                    LoadingRing.IsActive = true;
                    downloadTask = new CurrencyFromFileLoader().loadLatestCurrencyFile(cts.Token);
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
                    if (currencyList.Count > 0)
                    {
                        mylistbox.Visibility = Visibility.Visible;
                    }
                    else {
                        noDataToDisplay.Visibility = Visibility.Visible;
                    }
                    cts = null;
                    downloadTask = null;
                }
            }
        }
        private async void loadData(String fileName)
        {
            if (DownloadedFileList.isFileNameAlreadyDownloaded(fileName + ".xml"))
            {
                this.nameOfDisplayedFile = fileName;
                internetConnectionStatus.Visibility = Visibility.Collapsed;
                noDataToDisplay.Visibility = Visibility.Collapsed;
                cts = new CancellationTokenSource();
                try
                {
                    mylistbox.Visibility = Visibility.Collapsed;
                    LoadingRing.Visibility = Visibility.Visible;
                    LoadingRing.IsActive = true;
                    downloadTask = new CurrencyFromFileLoader().loadCurrencyFromFile(fileName, cts.Token);
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
                    if (currencyList.Count > 0)
                    {
                        mylistbox.Visibility = Visibility.Visible;
                    }
                    else {
                        noDataToDisplay.Visibility = Visibility.Visible;
                    }
                    cts = null;
                    downloadTask = null;
                }

            }
            else if (hasInternetConnection())
            {
                this.nameOfDisplayedFile = fileName;
                internetConnectionStatus.Visibility = Visibility.Collapsed;
                noDataToDisplay.Visibility = Visibility.Collapsed;
                cts = new CancellationTokenSource();
                try
                {
                    mylistbox.Visibility = Visibility.Collapsed;
                    LoadingRing.Visibility = Visibility.Visible;
                    LoadingRing.IsActive = true;
                    downloadTask = new CurrencyXMLDownload().downloadFileWIthName(fileName, cts.Token);
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
            else {
                internetConnectionStatus.Visibility = Visibility.Visible;
                noDataToDisplay.Visibility = Visibility.Visible;
            }
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
            this.Frame.Navigate(typeof(FileList), nameOfDisplayedFile);
        }

        private void mylistbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (downloadTask != null && cts != null && !downloadTask.IsCompleted)
            {
                cts.Cancel();
            }
            Currency currency = (Currency)mylistbox.SelectedItem;
            currency.fileNameWhichContainsThisCurrency = nameOfDisplayedFile;
            this.Frame.Navigate(typeof(CurrencyDetails), currency);
        }


    }
}
