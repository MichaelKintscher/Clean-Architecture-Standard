using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Entities.Calendar
{
    /// <summary>
    /// Represents a common calendar event interface.
    /// </summary>
    public interface ICalendarEvent
    {
        /// <summary>
        /// The unique ID of the event.
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// The name of the event.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The start time and date of the event.
        /// </summary>
        public DateTime Start { get; set; }

        /// <summary>
        /// The end time and date of the event.
        /// </summary>
        public DateTime End { get; set; }
    }
}
