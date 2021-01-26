using System;
using System.Collections.Generic;

#nullable disable

namespace DataAccess.DataModels
{
    public partial class TemperatureData
    {
        public int Temperature_Id { get; set; }
        public DateTime Date { get; set; }
        public string Place { get; set; }
        public double Temperature { get; set; }
        public int? Humidity { get; set; }
    }
}
