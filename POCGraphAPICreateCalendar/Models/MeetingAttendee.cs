using System;
using System.Collections.Generic;
using System.Text;

namespace POCGraphAPICreateCalendar.Models
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The meeting attendee.
    /// </summary>
    [DataContract]
    public class MeetingAttendee
    {
        /// <summary>
        /// Gets or sets the attendee type.
        /// </summary>
        [DataMember(Name = "type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the attendee email address.
        /// </summary>
        [DataMember(Name = "emailAddress")]
        public MeetingAttendeeEmailAddress EmailAddress { get; set; }

        /// <summary>
        /// Gets or sets the meeting attendee status
        /// </summary>
        [DataMember(Name = "status")]
        public MeetingAttendeeStatus Status { get; set; }
    }
}
