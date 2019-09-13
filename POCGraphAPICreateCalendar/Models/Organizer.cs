using System;
using System.Collections.Generic;
using System.Text;

namespace POCGraphAPICreateCalendar.Models
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The outlook calendar event organizer
    /// </summary>
    [DataContract]
    public class Organizer
    {
        /// <summary>
        ///  Gets or sets the email address
        /// </summary>
        [DataMember(Name = "emailAddress")]
        public MeetingAttendeeEmailAddress EmailAddress { get; set; }
    }
}
