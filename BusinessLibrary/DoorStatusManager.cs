using BusinessLibrary.DataModel;
using DataAccess.DataModels;
using Microsoft.EntityFrameworkCore;
using StoredProcedureEFCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLibrary
{
    public class DoorStatusManager
    {
        public static List<DoorReportModel> GetDoorStatusReport(List<TemperatureChange> rows, DateTime fromDate, DateTime toDate)
        {
            // --Inne when temperature decreases and outside temp increases its door open
            // --When inner temp and out temp increases -means door open
            // --when inner temp const and our temp increase or decrease then door closed
            // -- when inner temp and out temp decreases -means door closed

            List<TemperatureChange> indoorTemp = rows.Where(x => x.Place.Equals("I", StringComparison.OrdinalIgnoreCase)).ToList();
            List<TemperatureChange> outdoorTemp = rows.Where(x => x.Place.Equals("U", StringComparison.OrdinalIgnoreCase)).ToList();

            List<DoorEventModel> doorOpenDuration = FindDoorStatus(indoorTemp, outdoorTemp);
            rows = null;
            indoorTemp = null;
            outdoorTemp = null;
            return CreateReportData(doorOpenDuration, fromDate, toDate);
        }

        private static List<DoorReportModel> CreateReportData(List<DoorEventModel> doorOpenDuration, DateTime fromDate, DateTime toDate)
        {
            List<DoorReportModel> doorStatusList = new List<DoorReportModel>();
            DateTime fromDateCopy = fromDate;
            doorOpenDuration.ForEach(x => doorStatusList.AddRange(CategorizeDoorEventOnDayTime(x, toDate.AddDays(1))));
            return doorStatusList;
        }

        private static List<DoorReportModel> CategorizeDoorEventOnDayTime(DoorEventModel doorEvent, DateTime toDate)
        {
            List<DoorReportModel> doorStatusList = new List<DoorReportModel>();
            bool eventComplete = false;
            DateTime doorOpeningTime = doorEvent.DoorOpenDate;

            if (!doorEvent.DoorCloseDate.HasValue)
                doorEvent.DoorCloseDate = toDate;
            do
            {
                if (doorOpeningTime.Hour >= 5 && doorOpeningTime.Hour < 12)
                {
                    DateTime slabEndTime = new DateTime(doorOpeningTime.Year, doorOpeningTime.Month, doorOpeningTime.Day, 12, 00, 00);
                    if (slabEndTime >= doorEvent.DoorCloseDate)
                    {
                        doorStatusList.Add(new DoorReportModel
                        {
                            Date = doorOpeningTime.Date,
                            TimeOfTheDay = DayTime.Morning,
                            DoorOpenDuration = (int)doorEvent.DoorCloseDate.Value.Subtract(doorOpeningTime).TotalMinutes,
                            DoorOpenAt = doorOpeningTime,
                            DoorCloseAt = doorEvent.DoorCloseDate.Value
                        });
                        eventComplete = true;
                    }
                    else
                    {
                        doorStatusList.Add(new DoorReportModel
                        {
                            Date = doorOpeningTime.Date,
                            TimeOfTheDay = DayTime.Morning,
                            DoorOpenDuration = (int)slabEndTime.Subtract(doorOpeningTime).TotalMinutes,
                            DoorOpenAt = doorOpeningTime,
                            DoorCloseAt = slabEndTime
                        });
                        doorOpeningTime = slabEndTime;
                    }
                }
                else if (doorOpeningTime.Hour >= 12 && doorOpeningTime.Hour < 17)
                {
                    // Afternoon
                    DateTime slabEndTime = new DateTime(doorOpeningTime.Year, doorOpeningTime.Month, doorOpeningTime.Day, 17, 00, 00);
                    if (slabEndTime >= doorEvent.DoorCloseDate)
                    {
                        doorStatusList.Add(new DoorReportModel
                        {
                            Date = doorOpeningTime.Date,
                            TimeOfTheDay = DayTime.Afternoon,
                            DoorOpenDuration = (int)doorEvent.DoorCloseDate.Value.Subtract(doorOpeningTime).TotalMinutes,
                            DoorOpenAt = doorOpeningTime,
                            DoorCloseAt = doorEvent.DoorCloseDate.Value
                        });
                        eventComplete = true;
                    }
                    else
                    {
                        doorStatusList.Add(new DoorReportModel
                        {
                            Date = doorOpeningTime.Date,
                            TimeOfTheDay = DayTime.Afternoon,
                            DoorOpenDuration = (int)slabEndTime.Subtract(doorOpeningTime).TotalMinutes,
                            DoorOpenAt = doorOpeningTime,
                            DoorCloseAt = slabEndTime
                        });
                        doorOpeningTime = slabEndTime;
                    }
                }
                else if (doorOpeningTime.Hour >= 17 && doorOpeningTime.Hour < 22)
                {
                    // Evening
                    DateTime slabEndTime = new DateTime(doorOpeningTime.Year, doorOpeningTime.Month, doorOpeningTime.Day, 22, 00, 00);
                    if (slabEndTime >= doorEvent.DoorCloseDate)
                    {
                        doorStatusList.Add(new DoorReportModel
                        {
                            Date = doorOpeningTime.Date,
                            TimeOfTheDay = DayTime.Evening,
                            DoorOpenDuration = (int)doorEvent.DoorCloseDate.Value.Subtract(doorOpeningTime).TotalMinutes,
                            DoorOpenAt = doorOpeningTime,
                            DoorCloseAt = doorEvent.DoorCloseDate.Value
                        });
                        eventComplete = true;
                    }
                    else
                    {
                        doorStatusList.Add(new DoorReportModel
                        {
                            Date = doorOpeningTime.Date,
                            TimeOfTheDay = DayTime.Evening,
                            DoorOpenDuration = (int)slabEndTime.Subtract(doorOpeningTime).TotalMinutes,
                            DoorOpenAt = doorOpeningTime,
                            DoorCloseAt = slabEndTime
                        });
                        doorOpeningTime = slabEndTime;
                    }
                }
                else if (doorOpeningTime.Hour >= 22)
                {
                    // night
                    DateTime slabEndTime = new DateTime(doorOpeningTime.Year, doorOpeningTime.Month, doorOpeningTime.Day, 00, 00, 00).AddDays(1);
                    if (slabEndTime >= doorEvent.DoorCloseDate)
                    {
                        doorStatusList.Add(new DoorReportModel
                        {
                            Date = doorOpeningTime.Date,
                            TimeOfTheDay = DayTime.Night,
                            DoorOpenDuration = (int)doorEvent.DoorCloseDate.Value.Subtract(doorOpeningTime).TotalMinutes,
                            DoorOpenAt = doorOpeningTime,
                            DoorCloseAt = doorEvent.DoorCloseDate.Value
                        });
                        eventComplete = true;
                    }
                    else
                    {
                        doorStatusList.Add(new DoorReportModel
                        {
                            Date = doorOpeningTime.Date,
                            TimeOfTheDay = DayTime.Night,
                            DoorOpenDuration = (int)slabEndTime.Subtract(doorOpeningTime).TotalMinutes,
                            DoorOpenAt = doorOpeningTime,
                            DoorCloseAt = slabEndTime
                        });
                        doorOpeningTime = slabEndTime;
                    }
                }
                else if (doorOpeningTime.Hour >= 00 && doorOpeningTime.Hour < 5)
                {
                    // night
                    DateTime slabEndTime = new DateTime(doorOpeningTime.Year, doorOpeningTime.Month, doorOpeningTime.Day, 5, 00, 00);
                    if (slabEndTime >= doorEvent.DoorCloseDate)
                    {
                        doorStatusList.Add(new DoorReportModel
                        {
                            Date = doorOpeningTime.Date,
                            TimeOfTheDay = DayTime.Night,
                            DoorOpenDuration = (int)doorEvent.DoorCloseDate.Value.Subtract(doorOpeningTime).TotalMinutes,
                            DoorOpenAt = doorOpeningTime,
                            DoorCloseAt = doorEvent.DoorCloseDate.Value
                        });
                        eventComplete = true;
                    }
                    else
                    {
                        doorStatusList.Add(new DoorReportModel
                        {
                            Date = doorOpeningTime.Date,
                            TimeOfTheDay = DayTime.Night,
                            DoorOpenDuration = (int)slabEndTime.Subtract(doorOpeningTime).TotalMinutes,
                            DoorOpenAt = doorOpeningTime,
                            DoorCloseAt = slabEndTime
                        });
                        doorOpeningTime = slabEndTime;
                    }
                }
            }
            while (!eventComplete);
            return doorStatusList;
        }

        private static List<DoorEventModel> FindDoorStatus(List<TemperatureChange> indoorTemp, List<TemperatureChange> outdoorTemp)
        {
            bool doorStatus = false;
            List<TemperatureChange> insideTempAfter = null;
            List<DoorEventModel> doorOpenDuration = new List<DoorEventModel>();
            foreach (var item in outdoorTemp)
            {
                if (item.TempStatus == "-")
                {
                    // No change
                }
                // Temperature change
                else
                {
                    // Previous outdoor temp
                    // outsideTempBeforeAfter.Add(outdoorTemp.Where(x => x.Date < item.Date).Take(1).FirstOrDefault());
                    // Next outdoor temp
                    insideTempAfter = new List<TemperatureChange>();
                    // outsideTempBeforeAfter.AddRange(outdoorTemp.Where(x => x.Date < item.Date).OrderByDescending(x => x.Date).Take(1).ToList());
                    insideTempAfter.AddRange(indoorTemp.Where(x => x.Date >= item.Date && item.Date < x.Date.AddMinutes(2)).Take(1).ToList());

                    if (item.TempStatus == "up" && insideTempAfter.Any(x => x.TempStatus == "down") && !doorStatus)
                    {
                        doorOpenDuration.Add(new DoorEventModel { DoorOpenDate = item.Date });
                        doorStatus = true;
                    }
                    //else if (item.TempStatus == "up" && insideTempAfter.Any(x => x.TempStatus == "up") && !doorStatus)
                    //{
                    //    doorOpenDuration.Add(new DoorEventModel { DoorOpenDate = item.Date });
                    //    doorStatus = true;
                    //}
                    //else if (item.TempStatus == "down" && insideTempAfter.Any(x => x.TempStatus == "down") && !doorStatus)
                    //{
                    //    doorOpenDuration.Add(new DoorEventModel { DoorOpenDate = item.Date });
                    //    doorStatus = true;
                    //}

                    // || x.TempStatus == "-"
                    else if (item.TempStatus == "down" && insideTempAfter.Any(x => x.TempStatus == "up" ) && doorStatus)
                    {
                        doorStatus = false;
                        if (doorOpenDuration.Any(x => x.DoorCloseDate.HasValue == false))
                        {
                            doorOpenDuration.Where(x => x.DoorCloseDate.HasValue == false).OrderByDescending(x => x.DoorOpenDate).First().DoorCloseDate = item.Date;
                        }
                    }
                    insideTempAfter = null;
                }
            }
            return doorOpenDuration;
        }
    }
}

