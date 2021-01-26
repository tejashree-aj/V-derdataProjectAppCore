using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.DataModels
{
    public class AverageTemperatureView
    {
        public string Place { get; set; }
        public double Temperature { get; set; }
        public DateTime Date { get; set; }
    }
}
