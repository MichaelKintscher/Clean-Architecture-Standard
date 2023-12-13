using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitectureEntities.Calendar
{
    /// <summary>
    /// Represents a common calendar interface.
    /// </summary>
    public interface ICalendar
    {
        /// <summary>
        /// The unique ID of the calendar.
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// The ID of the service provider account the calendar is associated with.
        /// </summary>
        public string AccountID { get; set; }

        /// <summary>
        /// The name of the calendar.
        /// </summary>
        public string Name { get; set; }
    }
}
