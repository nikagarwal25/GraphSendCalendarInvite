using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace POCGraphAPICreateCalendar.Models
{
    /// <summary>
    /// The outlook calendar event response status
    /// </summary>
    [DataContract]
    public class ResponseStatus
    {
        /// <summary>
        /// Gets or sets the event response
        /// </summary>
        [DataMember(Name = "response")]
        public string Response { get; set; }

        /// <summary>
        /// Gets or sets the event time
        /// </summary>
        [DataMember(Name = "time")]
        public DateTime Time { get; set; }
    }
}
