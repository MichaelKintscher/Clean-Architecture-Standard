using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Core.Domain.EventArguments
{
    /// <summary>
    /// Contains event info with an entity value.
    /// </summary>
    public class EntityEventArgs<T> : EventArgs
    {
        /// <summary>
        /// The value.
        /// </summary>
        public T Value { get; private set; }

        public EntityEventArgs(T value)
        {
            Value = value;
        }
    }
}
