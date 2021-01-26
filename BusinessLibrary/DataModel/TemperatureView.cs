using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLibrary.DataModel
{
    public class TemperatureView
    {
        public DateTime Date { get; set; }
        public string Place { get; set; }
        public double Temperature { get; set; }
        public int? Humidity { get; set; }
        public double MoldValue { get; set; }
    }
}
