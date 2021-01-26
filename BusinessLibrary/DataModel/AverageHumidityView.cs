using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLibrary.DataModel
{
    public class AverageHumidityView
    {
        public string Place { get; set; }
        public int? Humidity { get; set; }
        public DateTime Date { get; set; }
    }
}
