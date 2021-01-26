using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLibrary.DataModel
{   

    public class DoorEventModel
    {
        public DateTime DoorOpenDate { get; set; }
        public DateTime? DoorCloseDate { get; set; }


    }

    public class DoorReportModel
    {
        public DateTime? Date { get; set; }
        public int DoorOpenDuration { get; set; }
        public DayTime TimeOfTheDay { get; set; }
        public DateTime DoorOpenAt { get; set; }
        public DateTime? DoorCloseAt { get; set; }
    }

    public enum DayTime
    {
        Morning,
        Afternoon,
        Evening,
        Night
    }
}
