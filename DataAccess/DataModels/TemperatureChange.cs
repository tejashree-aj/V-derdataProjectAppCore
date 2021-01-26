using System;
using System.Collections.Generic;

#nullable disable

namespace DataAccess.DataModels
{
    public class TemperatureChange
    {
        public DateTime Date { get; set; }
        public string Place { get; set; }
        public double Temperature { get; set; }
        public double PreviousTemperature { get; set; }
        public string TempStatus { get; set; }
    }
}
