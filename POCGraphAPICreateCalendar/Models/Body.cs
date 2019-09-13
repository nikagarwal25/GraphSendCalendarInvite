using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace POCGraphAPICreateCalendar.Models
{
    /// <summary>
    /// Outlook calendar event body
    /// </summary>
    [DataContract]
    public class Body
    {
        /// <summary>
        /// Gets or sets the content type
        /// </summary>
        [DataMember(Name = "contentType")]
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the content
        /// </summary>
        [DataMember(Name = "content")]
        public string Content { get; set; }
    }
}
