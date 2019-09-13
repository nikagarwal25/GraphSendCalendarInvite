using System;
using System.Collections.Generic;
using System.Text;

namespace POCGraphAPICreateCalendar
{
    public class SchedulerConstants
    {
        public const string ThirtyMinuteFreeBusy = "30";
        public const string UTCTimezone = "UTC";
        public const string ExpandExtensionFilter = "?$expand=extensions($filter=id%20eq%20'Microsoft.OutlookServices.OpenTypeExtension.Com.Dynamics.Hcm.Scheduler')";

    }

    public class Constants
    {
        public const string BearerAuthenticationScheme = "Bearer";
    }
}
