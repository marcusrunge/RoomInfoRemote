using RoomInfoRemote.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RoomInfoRemote.Helpers
{
    public class TimeValidator
    {
        public static TimeSpan ValidateStartTime(TimeSpanItem TimeSpanItem, List<TimeSpanItem> TimeSpanItems)
        {
            var collisioningTimeSpanItem = TimeSpanItems.Where(x => x.Id != TimeSpanItem.Id && TimeSpanItem.Start >= x.Start && TimeSpanItem.Start <= x.End).Select(x => x).FirstOrDefault();
            return collisioningTimeSpanItem != null ? collisioningTimeSpanItem.End.Add(TimeSpan.FromMinutes(1)) : TimeSpan.Zero;
        }

        public static TimeSpan ValidateEndTime(TimeSpanItem TimeSpanItem, List<TimeSpanItem> TimeSpanItems)
        {
            if (TimeSpanItem.End < TimeSpanItem.Start) return TimeSpanItem.Start;
            else
            {
                var collisioningTimeSpanItem = TimeSpanItems.Where(x => x.Id != TimeSpanItem.Id && TimeSpanItem.End >= x.Start && TimeSpanItem.End <= x.End).Select(x => x).FirstOrDefault();
                return collisioningTimeSpanItem != null ? collisioningTimeSpanItem.Start.Subtract(TimeSpan.FromMinutes(1)) > TimeSpan.Zero ? collisioningTimeSpanItem.Start.Subtract(TimeSpan.FromMinutes(1)) : TimeSpanItem.Start.Add(TimeSpan.FromMinutes(1)) : TimeSpan.Zero;
            }
        }
    }
}
