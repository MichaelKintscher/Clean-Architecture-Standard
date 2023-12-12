using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitectureStandard.Domain.EventArguments
{
    /// <summary>
    /// Contains event info with a list of entity values.
    /// </summary>
    class EntityListEventArgs<T> : EventArgs
    {
        /// <summary>
        /// The list of values.
        /// </summary>
        public List<T> Values { get; private set; }

        public EntityListEventArgs(List<T> values)
        {
            Values = values;
        }
    }
}
