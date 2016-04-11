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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace App1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private List<Currency> latestCurrency;
        //ObservableCollection<Currency> currencyList { get; set; 

        public MainPage()
        {
            this.InitializeComponent();
            loadData();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            loadData();
            

        }

        private void loadData() {
            LoadingRing.IsActive = true;
            Task<List<Currency>> downloadTask = XMLSimpleDownload.downloadLatestXML();

            Task continuation = downloadTask.ContinueWith(antecedent => {
                latestCurrency = antecedent.Result;
                LoadingRing.IsActive = false;
            });
        }
    }
}
