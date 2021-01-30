using BusinessLibrary;
using BusinessLibrary.DataModel;
using DataAccess.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls.DataVisualization.Charting;

namespace VäderdataProjectAppCore
{
    /// <summary>
    /// Interaction logic for WeatherReport.xaml
    /// </summary>
    public partial class WeatherReport : Window
    {
        DBManager dbManager;
        public WeatherReport(TemperatureDataContext _dbContext)
        {
            InitializeComponent();

            dbManager = new DBManager(_dbContext);

            ShowAutumnAndWinterDate();
        }

        private void ShowAutumnAndWinterDate()
        {
            var autumnDateForTheYear = dbManager.GetAutumnDateForTheYear();
            AutumnDate.Text = autumnDateForTheYear.ToShortDateString();

            var winterDateForTheYear = dbManager.GetWinterDateForTheYear();
            WinterDate.Text = winterDateForTheYear.ToShortDateString();
        }

        List<TemperatureView> temperatureViewData;

        private void GetWeatherData_Click(object sender, RoutedEventArgs e)
        {
            DateTime date1, date2;

            date1 = DateTime.Parse(selectedDate1.Text);
            date2 = DateTime.Parse(selectedDate2.Text).AddDays(1);

            if (date1 >= DateTime.Parse("05-31-2016") && date2 <= DateTime.Parse("10-01-2017"))
            {
                errorMessage.Visibility = Visibility.Collapsed;

                temperatureViewData = dbManager.GetDataFromDataBase(date1, date2);

                ShowAverageTemperatureInAndOutReport(temperatureViewData);

                ShowWarmToColdDaysReport(temperatureViewData);

                ShowDryToWetDaysReport(temperatureViewData);

                ShowMoldRiskIndex(temperatureViewData);

                ShowDoorReports(date1, date2);
            }
            else
            {
                errorMessage.Visibility = Visibility.Visible;
            }
        }

        private void ShowDoorReports(DateTime fromDate, DateTime toDate)
        {
            var temperatureChangeData = dbManager.GetDoorStatusReport(fromDate, toDate);

            List<DoorReportModel> doorReportModels = DoorStatusManager.GetDoorStatusReport(temperatureChangeData, fromDate, toDate)
                                                    .Where(x => x.DoorOpenDuration > 10).ToList();

            totaldoorOpenduration.Text = $"Total door open duration {(doorReportModels.Sum(x => x.DoorOpenDuration) / 60).ToString()} hours out of {toDate.Subtract(fromDate).TotalHours} hours";

            ((LineSeries)DoorOpenReport.Series[0]).ItemsSource = doorReportModels
                .GroupBy(x => x.Date)
                    .Select(x => new KeyValuePair<string, double>(x.Key.Value.ToShortDateString(),
                        (double)x.Sum(y => y.DoorOpenDuration) / (double)60
                    )).ToList();

            ((PieSeries)DoorOpenPieReport.Series[0]).ItemsSource = doorReportModels
                .GroupBy(x => x.TimeOfTheDay)
                    .Select(x => new KeyValuePair<string, double>(x.Key.ToString(),
                        (double)x.Sum(y => y.DoorOpenDuration) / (double)60
                    )).ToList();
        }

        private void ShowAverageTemperatureInAndOutReport(List<TemperatureView> temperatureViewData)
        {
            var avgWeather = WeatherActivityManager.GetAverageTemparature(temperatureViewData);
            if (avgWeather.Any(x => x.Place == "Inne"))
                avgIndoorTemp.Text = $"{avgWeather.FirstOrDefault(x => x.Place == "Inne").Temperature}";
            else
                avgIndoorTemp.Text = $"0";
            if (avgWeather.Any(x => x.Place == "Ute"))
                avgOutdoorTemp.Text = $"{avgWeather.FirstOrDefault(x => x.Place == "Ute").Temperature}";
            else
                avgOutdoorTemp.Text = $"0";
            //AverageTemperature.DataContext = avgWeather;
        }

        private void ShowWarmToColdDaysReport(List<TemperatureView> temperatureViewData)
        {
            var avgTempPerDay = WeatherActivityManager.AverageTemperaturePerDay(temperatureViewData);


            ((LineSeries)WarmToColdDayOutdoorReport.Series[0]).ItemsSource = avgTempPerDay
                    .Where(x => x.Place.Equals("Ute", StringComparison.OrdinalIgnoreCase)).OrderByDescending(x => x.Temperature)
                    .Select(x => new KeyValuePair<string, double>(x.Date.ToShortDateString(), x.Temperature)).ToList();

            ((LineSeries)WarmToColdDayIndoorReport.Series[0]).ItemsSource = avgTempPerDay
                    .Where(x => x.Place.Equals("Inne", StringComparison.OrdinalIgnoreCase)).OrderByDescending(x => x.Temperature)
                    .Select(x => new KeyValuePair<string, double>(x.Date.ToShortDateString(), x.Temperature)).ToList();
        }

        private void ShowDryToWetDaysReport(List<TemperatureView> temperatureViewData)
        {
            var avgHumidityPerDay = WeatherActivityManager.AverageHumidityPerDay(temperatureViewData);

            ((LineSeries)DryToWetDayOutdoorReport.Series[0]).ItemsSource = avgHumidityPerDay
                .Where(x => x.Place.Equals("Ute", StringComparison.OrdinalIgnoreCase)).OrderByDescending(x => x.Humidity)
                .Select(x => new KeyValuePair<string, int?>(x.Date.ToShortDateString(), x.Humidity)).ToList();

            ((LineSeries)DryToWetDayIndoorReport.Series[0]).ItemsSource = avgHumidityPerDay
                .Where(x => x.Place.Equals("Inne", StringComparison.OrdinalIgnoreCase)).OrderByDescending(x => x.Humidity)
                .Select(x => new KeyValuePair<string, int?>(x.Date.ToShortDateString(), x.Humidity)).ToList();
        }

        private void ShowMoldRiskIndex(List<TemperatureView> temperatureViewData)
        {
            var moldIndex = WeatherActivityManager.MoldRiskIndex(temperatureViewData);

            var uteMoldIndex = moldIndex
                .Where(x => x.Place.Equals("Ute", StringComparison.OrdinalIgnoreCase))
                .Select(x => new KeyValuePair<string, double>(x.Date.ToShortDateString(), x.MoldValue)).ToList();

            avgOutdoorMold.Text = $"{MoldState(uteMoldIndex.Average(x => x.Value))}";

            ((LineSeries)MoldRiskIndexOutdoorReport.Series[0]).ItemsSource = uteMoldIndex;

            var inneMoldIndex = moldIndex
                .Where(x => x.Place.Equals("Inne", StringComparison.OrdinalIgnoreCase))
                .Select(x => new KeyValuePair<string, double>(x.Date.ToShortDateString(), x.MoldValue)).ToList();

            avgIndoorMold.Text = $"{MoldState(inneMoldIndex.Average(x => x.Value))}";

            ((LineSeries)MoldRiskIndexIndoorReport.Series[0]).ItemsSource = inneMoldIndex;

        }

        private string MoldState(double moldIndex)
        {
            return moldIndex < 0 ? "Low Risk" : moldIndex > 0 && moldIndex < 10 ? "Moderate Risk" : "High Risk";
        }

    }
}
