using BusinessLibrary.DataModel;
using DataAccess.DataModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLibrary
{
    public class DBManager
    {
        TemperatureDataContext dataContext;

        public DBManager(TemperatureDataContext _dataContext)
        {
            this.dataContext = _dataContext;
        }

        public DateTime GetAutumnDateForTheYear()
        {
            IEnumerable<AverageTemperatureView> rows = null;

            DateTime autumnDate = new DateTime(2016, 09, 01);

            rows = dataContext.Set<AverageTemperatureView>().FromSqlRaw($"execute dbo.[GetAutumnTemperatureData]");

            var sequenceData = FindConsecutiveGroups(rows, 5);
            if (sequenceData != null)
            {
                autumnDate = sequenceData.First().Date;
            }
            return autumnDate;

        }

        public DateTime GetWinterDateForTheYear()
        {
            IEnumerable<AverageTemperatureView> rows = null;

            DateTime autumnDate = new DateTime(2016, 09, 01);

            rows = dataContext.Set<AverageTemperatureView>().FromSqlRaw($"execute dbo.[GetWinterTemperatureData]");

            var sequenceData = FindConsecutiveGroups(rows, 4);
            if (sequenceData != null)
            {
                autumnDate = sequenceData.First().Date;
            }
            return autumnDate;

        }

        private IEnumerable<AverageTemperatureView> FindConsecutiveGroups(IEnumerable<AverageTemperatureView> sequence, int count)
        {
            while (sequence.Count() > count)
            {
                var window = sequence.Take(count);

                if (window.Last().Date.Subtract(window.First().Date).TotalDays == count - 1)
                    return window;

                sequence = sequence.Skip(1);
            }
            return null;
        }

        public List<TemperatureView> GetDataFromDataBase(DateTime fromDate, DateTime toDate)
        {
            var data = dataContext.Set<TemperatureData>().FromSqlRaw($"execute dbo.GetCompleteData '{fromDate}', '{toDate}'").ToList();

            var completeData = data.Select(x => new TemperatureView
            {
                Date = x.Date,
                Humidity = x.Humidity,
                Place = x.Place,
                Temperature = x.Temperature
            }).ToList();            
            
            return completeData;
        }

        public List<TemperatureChange> GetDoorStatusReport(DateTime fromDate, DateTime toDate)
        {
            return dataContext.Set<TemperatureChange>().FromSqlRaw($"execute dbo.Gettemperaturedata '{fromDate}','{toDate}'").ToList();
        }

       
    }
}


//public List<TemperatureView> GetDataFromDataBase(DateTime date1, DateTime date2)
//{
//    var data = dataContext.Temperatures.Where(X => X.Date >= date1 && X.Date < date2).Select(
//        x => new TemperatureView
//        {
//            Date = x.Date,
//            Humidity = x.Humidity,
//            Place = x.Place,
//            Temperature = x.Temperature
//        }).ToList();
//    return data;
//}
