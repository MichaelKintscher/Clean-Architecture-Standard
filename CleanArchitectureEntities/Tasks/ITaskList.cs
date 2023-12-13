using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitectureEntities.Tasks
{
    /// <summary>
    /// Represents a common task list interface.
    /// </summary>
    public interface ITaskList
    {
        /// <summary>
        /// The unique task list ID.
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// The task list name.
        /// </summary>
        public string Name { get; set; }
    }
}
