using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitectureEntities.Tasks
{
    /// <summary>
    /// Represents a common task interface.
    /// </summary>
    public interface ITask
    {
        /// <summary>
        /// The unique task ID.
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// The task name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The task description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The task due date.
        /// </summary>
        public DateTime DueDate { get; set; }
    }
}
