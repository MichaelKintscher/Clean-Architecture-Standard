using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Core.Domain.EventArguments
{
    /// <summary>
    /// Contains event info with a list of entity values.
    /// NOT intended for direct use; inherit from this class.
    /// </summary>
    /// <typeparam name="T">The type of the entity the args contains a list of.</typeparam>
    public abstract class EntityListEventArgs<T> : EventArgs
    {
        /// <summary>
        /// The list of values.
        /// </summary>
        protected List<T> Values { get; private set; }

        /// <summary>
        /// Constructor sets the given list of value.
        /// </summary>
        /// <param name="values">The list of values to set.</param>
        public EntityListEventArgs(List<T> values)
        {
            Values = values;
        }
    }
}
