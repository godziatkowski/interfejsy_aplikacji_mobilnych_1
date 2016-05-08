using App1.DataObjects;
using App1.LocalStorageUtils;
using App1.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WinRTXamlToolkit;
using WinRTXamlToolkit.Composition;
using WinRTXamlToolkit.Controls.DataVisualization.Charting;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace App1.Views
{

    public sealed partial class CurrencyDetails : Page
    {
        private DateTime fromDateFilter;
        private DateTime toDateFilter;
        private DateTime minDate;
        private DateTime maxDate;
        private Task<Dictionary<String, String>> downloadTask;
        private Task<List<Currency>> currencyLoadingTask;
        private Dictionary<DateTime, String> namesOfFilesToLoadWithPublicationDates;
        private Dictionary<DateTime, Double> currencyData;
        private CancellationTokenSource cts;
        private Currency displayedCurrency;

        public CurrencyDetails()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            //initlize date filters
            fromDateFilter = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            toDateFilter = DateTime.Today;
            minDate = new DateTime(2002, 01, 02);
            maxDate = new DateTime(DateTime.Today.Year, DateTime.Today.AddMonths(1).Month, 1).AddDays(-1);
            DateFromFilter.Date = fromDateFilter;
            DateFromFilter.MinDate = minDate;
            DateFromFilter.MaxDate = maxDate;
            DateToFilter.Date = toDateFilter;
            DateToFilter.MinDate = minDate;
            DateToFilter.MaxDate = maxDate;

            //set page currency name
            displayedCurrency = (Currency)e.Parameter;
            CurrencyName.Text = convertToUpperCaseEachWord(displayedCurrency.currencyName.Trim());

        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }

        private void BackToCurrencyPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (downloadTask != null && cts != null && !downloadTask.IsCompleted)
            {
                cts.Cancel();
            }
            this.Frame.Navigate(typeof(MainPage), displayedCurrency.fileNameWhichContainsThisCurrency);
        }

        private String convertToUpperCaseEachWord(String phrase)
        {
            StringBuilder sb = new StringBuilder("");
            foreach (string word in phrase.Split(' '))
            {
                sb.Append(char.ToUpper(word[0]) + word.Substring(1) + " ");
            }

            return sb.ToString().Trim();
        }

        private async void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            await loadRequiredData();
        }

        private async Task loadRequiredData()
        {
            if (DateToFilter.Date.Value != null && DateFromFilter.Date.Value != null && (DateToFilter.Date.Value.Ticks > DateFromFilter.Date.Value.Ticks))
            {
                int fromYear = DateFromFilter.Date.Value.Year;
                int toYear = DateToFilter.Date.Value.Year;
                IncorrectDateFiltersInfo.Visibility = Visibility.Collapsed;
                lineChart.Visibility = Visibility.Collapsed;
                Filters.Visibility = Visibility.Collapsed;
                //Save.Visibility = Visibility.Collapsed;
                LoadingRing.Visibility = Visibility.Visible;
                LoadingRing.IsActive = true;
                List<int> yearsToLoad = new List<int>();

                if (fromYear != toYear)
                {
                    for (; fromYear <= toYear; fromYear++)
                    {
                        yearsToLoad.Add(fromYear);
                    }
                }
                else {
                    yearsToLoad.Add(fromYear);
                }
                namesOfFilesToLoadWithPublicationDates = new Dictionary<DateTime, string>();
                foreach (int year in yearsToLoad)
                {
                    cts = new CancellationTokenSource();
                    try
                    {
                        if (year == DateTime.Today.Year)
                        {
                            downloadTask = new TxtDirDownload().downloadLatestDirFile(cts.Token);
                        }
                        else {
                            downloadTask = new TxtDirDownload().downloadDirFileWithName(year.ToString(), cts.Token);
                        }
                        await downloadTask;

                        var dictionaryWithFileNames = downloadTask.Result;
                        foreach (var element in dictionaryWithFileNames)
                        {
                            string[] publicationDateAsStringArray = element.Key.Split('-');
                            DateTime publicationDate = new DateTime(Int16.Parse(publicationDateAsStringArray[2]), Int16.Parse(publicationDateAsStringArray[1]), Int16.Parse(publicationDateAsStringArray[0]));
                            if (publicationDate.Ticks > DateFromFilter.Date.Value.Ticks && publicationDate.Ticks < DateToFilter.Date.Value.Ticks)
                            {
                                namesOfFilesToLoadWithPublicationDates.Add(publicationDate, element.Value);
                            }
                        }

                    }
                    catch (OperationCanceledException ex)
                    {

                    }
                    finally
                    {
                        cts = null;
                        downloadTask = null;
                    }
                }

                List<String> alreadyDownloadedFileNames = DownloadedFileList.getFileNamesPublishedBetweenDates(DateFromFilter.Date.Value.DateTime, DateToFilter.Date.Value.DateTime);
                currencyData = new Dictionary<DateTime, Double>();
                foreach (var dictionaryElement in namesOfFilesToLoadWithPublicationDates)
                {
                    String fileName = dictionaryElement.Value;
                    if (alreadyDownloadedFileNames.Contains(fileName))
                    {
                        cts = new CancellationTokenSource();
                        try
                        {
                            currencyLoadingTask = new CurrencyFromFileLoader().loadCurrencyFromFile(fileName, cts.Token);
                            await currencyLoadingTask;
                            var currencyList = currencyLoadingTask.Result;
                            Currency requestedCurrency = currencyList.Find(x => x.currencyCode == displayedCurrency.currencyCode);
                            if (requestedCurrency != null)
                            {
                                currencyData.Add(dictionaryElement.Key, requestedCurrency.currencyAsPLN);
                            }
                        }
                        catch (OperationCanceledException ex) { }
                        finally
                        {
                            cts = null;
                            currencyLoadingTask = null;
                        }
                    }
                    else {
                        cts = new CancellationTokenSource();
                        try
                        {
                            currencyLoadingTask = new CurrencyXMLDownload().downloadFileWIthName(fileName, cts.Token);
                            await currencyLoadingTask;
                            var currencyList = currencyLoadingTask.Result;
                            Currency requestedCurrency = currencyList.Find(x => x.currencyCode == displayedCurrency.currencyCode);
                            if (requestedCurrency != null)
                            {
                                currencyData.Add(dictionaryElement.Key, requestedCurrency.currencyAsPLN);
                            }
                        }
                        catch (OperationCanceledException ex) { }
                        finally
                        {
                            cts = null;
                            currencyLoadingTask = null;
                        }
                    }
                }

                drawLineChart();

                Filters.Visibility = Visibility.Visible;
                LoadingRing.Visibility = Visibility.Collapsed;
                LoadingRing.IsActive = false;
                lineChart.Visibility = Visibility.Visible;
                //Save.Visibility = Visibility.Visible;
            }
            else {
                IncorrectDateFiltersInfo.Visibility = Visibility.Visible;
            }
        }

        private void drawLineChart()
        {
            List<ChartPoint> chartPoints = new List<ChartPoint>();
            foreach (var currencyData in currencyData)
            {
                chartPoints.Add(new ChartPoint()
                {
                    Label = currencyData.Key,
                    Amount = currencyData.Value
                });
            }
            (lineChart.Series[0] as LineSeries).ItemsSource = chartPoints;

        }

        //private async void SaveButton_Click(object sender, RoutedEventArgs e)
        //{
        //    System.Diagnostics.Debug.WriteLine("start");
        //    MemoryStream ms = await WriteableBitmapRenderExtensions.RenderToPngStream(lineChart);
            
        //    System.Diagnostics.Debug.WriteLine(1);
        //    Windows.Storage.Pickers.FileSavePicker savePicker = new Windows.Storage.Pickers.FileSavePicker();
        //    System.Diagnostics.Debug.WriteLine(2);
        //    savePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
        //    System.Diagnostics.Debug.WriteLine(3);
        //    savePicker.FileTypeChoices.Add("JPG file", new List<string>() { ".png" });
        //    System.Diagnostics.Debug.WriteLine(4);
        //    Windows.Storage.StorageFile file = await savePicker.PickSaveFileAsync();
        //    System.Diagnostics.Debug.WriteLine(5);
        //    using (Stream x = await file.OpenStreamForWriteAsync())
        //    {
        //        System.Diagnostics.Debug.WriteLine(6);
        //        x.Seek(0, SeekOrigin.Begin);
        //        ms.WriteTo(x);
        //        System.Diagnostics.Debug.WriteLine(7);
        //    }
        //    System.Diagnostics.Debug.WriteLine("end");
        //}
    }
}
