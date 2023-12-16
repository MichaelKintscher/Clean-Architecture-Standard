using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Core.Domain.EventArguments
{
    /// <summary>
    /// Contains event info with an entity value.
    /// NOT intended for direct use; inherit from this class.
    /// </summary>
    /// <typeparam name="T">The type of the entity the args contains.</typeparam>
    public abstract class EntityEventArgs<T> : EventArgs
    {
        /// <summary>
        /// The value.
        /// </summary>
        protected T Value { get; private set; }

        /// <summary>
        /// Constructor sets the given value.
        /// </summary>
        /// <param name="value">The value to set.</param>
        public EntityEventArgs(T value)
        {
            Value = value;
        }
    }
}
