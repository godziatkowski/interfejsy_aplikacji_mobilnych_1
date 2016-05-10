using App1.LocalStorageUtils;
using App1.Web;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.Connectivity;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace App1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FileList : Page
    {
        ObservableCollection<String> publicationDates { get; set; }
        private Task<Dictionary<String, String>> downloadTask;
        private Dictionary<String, String> filesWithPublicationDate;
        private CancellationTokenSource cts;
        private string callbackFile;

        public FileList()
        {
            this.InitializeComponent();
            loadFiles();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            new PageSettingsUtil().setCurrentPage("fileList");
            callbackFile = (String)e.Parameter;
        }

        private bool hasInternetConnection()
        {
            var connectionProfile = NetworkInformation.GetInternetConnectionProfile();
            return (connectionProfile != null &&
                    connectionProfile.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess);
        }

        private void ShowValues_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage), callbackFile);
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }

        private void yearComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (fileListBox != null && fileListLoadingRing != null)
            {
                loadFiles();
            };
        }

        private async void loadFiles()
        {

            if (hasInternetConnection())
            {
                internetConnectionStatus.Visibility = Visibility.Collapsed;
                cts = new CancellationTokenSource();
                try
                {
                    String year = ((ComboBoxItem)yearComboBox.SelectedItem).Name.Substring(1);
                    fileListBox.Visibility = Visibility.Collapsed;
                    fileListLoadingRing.Visibility = Visibility.Visible;
                    fileListLoadingRing.IsActive = true;
                    if (Int16.Parse(year).Equals(2016))
                    {
                        downloadTask = new TxtDirDownload().downloadLatestDirFile(cts.Token);
                    }
                    else {
                        downloadTask = new TxtDirDownload().downloadDirFileWithName(year, cts.Token);
                    }
                    await downloadTask;


                    filesWithPublicationDate = downloadTask.Result;
                    List<String> publishDates = new List<String>(filesWithPublicationDate.Keys);
                    publicationDates = new ObservableCollection<string>(publishDates);
                    fileListBox.ItemsSource = publicationDates;

                }
                catch (OperationCanceledException ex)
                {

                }
                finally
                {
                    fileListLoadingRing.IsActive = false;
                    fileListLoadingRing.Visibility = Visibility.Collapsed;
                    fileListBox.Visibility = Visibility.Visible;
                    cts = null;
                    downloadTask = null;
                }
            }
            else
            {
                internetConnectionStatus.Visibility = Visibility.Visible;
            }
        }

        private void fileListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            String keyInDictionary = fileListBox.SelectedItem.ToString();
            String fileName = filesWithPublicationDate[keyInDictionary].Trim();

            this.Frame.Navigate(typeof(MainPage), fileName);

        }
    }
}
