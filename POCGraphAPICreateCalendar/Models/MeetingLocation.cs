using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace POCGraphAPICreateCalendar.Models
{
    [DataContract]
    public class MeetingLocation
    {
        /// <summary>
        /// Gets or sets the location display name.
        /// </summary>
        [DataMember(Name = "displayName")]
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the location address.
        /// </summary>
        [DataMember(Name = "address")]
        public MeetingAddress Address { get; set; }
    }
}
