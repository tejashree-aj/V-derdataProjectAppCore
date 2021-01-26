using BusinessLibrary.DataModel;
using DataAccess.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;


namespace BusinessLibrary
{
    public class WeatherActivityManager
    {        
        public static List<AverageTemperatureView> GetAverageTemparature(List<TemperatureView> temperatureData)
        {
            var data = temperatureData
                      .GroupBy(x => x.Place)
                      .Select(x => new AverageTemperatureView
                      {
                          Place = x.Key,
                          Temperature = Math.Round(x.Average(y => y.Temperature), 2)
                      }).ToList();
            return data;
        }

        //public static List<GroupByDateClass> PerDayAverageTemperature(DateTime date1, DateTime date2)
        //{
        //    using (var context = new TemperatureDataContext())
        //    {
        //        var data = context.Temperaturs.Where(X => X.Date >= date1 && X.Date < date2)
        //                   .GroupBy(x => x.Date.Date)
        //                   .Select(x => new GroupByDateClass
        //                   {
        //                       perDayTemperature = x.Average(temp => temp.Temperature),
        //                       day = x.Key
        //                   }).ToList();

        //        return data;
        //    }
        //}

        public static List<AverageTemperatureView> AverageTemperaturePerDay(List<TemperatureView> temperatureData)
        {
            var data = temperatureData
                       .GroupBy(x => new { x.Date.Date, x.Place })
                       .Select(x => new AverageTemperatureView
                       {
                           Temperature = Math.Round(x.Average(temp => temp.Temperature), 2),
                           Date = x.Key.Date,
                           Place = x.Key.Place
                       }).OrderBy(x => x.Date).ToList();

            return data;
        }

        public static List<AverageHumidityView> AverageHumidityPerDay(List<TemperatureView> temperatureData)
        {
            var data = temperatureData
                       .GroupBy(x => new { x.Date.Date, x.Place })
                       .Select(x => new AverageHumidityView
                       {
                           Humidity = (int?)x.Average(humidity => humidity.Humidity),
                           Date = x.Key.Date,
                           Place = x.Key.Place
                       }).OrderBy(x => x.Humidity).ToList();

            return data;
        }

        public static List<MoldRiskView> MoldRiskIndex(List<TemperatureView> temperatureData)
        {

            var data = temperatureData
                       .GroupBy(x => new { x.Date.Date, x.Place })
                       .Select(x => new
                       {
                           Date = x.Key.Date,
                           Place = x.Key.Place,
                           Humidity = x.Average(humidity => humidity.Humidity),
                           Temperature = x.Average(temp => temp.Temperature),

                       }).Select(x => new MoldRiskView
                       {
                           Date = x.Date,
                           Place = x.Place,
                           MoldValue = (double)((((x.Humidity.HasValue ? x.Humidity.Value : 0) - 78) * (x.Temperature / 15)) / 0.22)
                       }
                ).ToList();
            return data;
        }

      


        //public static DateTime GetAutumnDateForTheYear()
        //{
        //    DateTime autumnDate = new DateTime(2016, 09, 01);

        //    List<AverageTemperatureView> data;
        //    using (var context = new TemperatureDataContext())
        //    {
        //       // && x.Date.Date >= autumnDate
        //        data = context.Temperatures.Where(x => x.Place == "Ute")
        //                .GroupBy(x => new { x.Date.Date, x.Place })
        //                .Select(x => new AverageTemperatureView
        //                {
        //                    Date = x.Key.Date,
        //                    Temperature = Math.Round(x.Average(temp => temp.Temperature), 2),

        //                }).Where(x => x.Temperature < 10).OrderBy(x => x.Date).ToList();

        //    }

        //    var sequenceData = FindConsecutiveGroups(data, 5);
        //    if (sequenceData != null)
        //    {
        //        autumnDate = sequenceData.First().Date;
        //    }
        //    return autumnDate;
        //}

        //private static IEnumerable<AverageTemperatureView> FindConsecutiveGroups(IEnumerable<AverageTemperatureView> sequence, int count)
        //{
        //    while (sequence.Count() > count)
        //    {
        //        var window = sequence.Take(count);

        //        if (window.Last().Date.Subtract(window.First().Date).TotalDays == count-1)
        //            return window;

        //        sequence = sequence.Skip(1);
        //    }
        //    return null;
        //}


        //public static DateTime GetWinterDateForTheYear()
        //{
        //    using (var context = new TemperatureDataContext())
        //    {
        //        DateTime winterDate = new DateTime(2016, 09, 01);

        //        var data = context.Temperatures.Where(x => x.Place == "Ute")
        //                .GroupBy(x => new { x.Date.Date, x.Place })
        //                .Select(x => new AverageTemperatureView
        //                {
        //                    Date = x.Key.Date,
        //                    Temperature = Math.Round(x.Average(temp => temp.Temperature), 2),

        //                }).Where(x => x.Temperature < 0).OrderBy(x => x.Date);

        //        var sequenceData = FindConsecutiveGroups(data, 4);
        //        if (sequenceData != null)
        //        {
        //            winterDate = sequenceData.First().Date;
        //        }
        //        return winterDate;
        //    }

        //}
    }
}


