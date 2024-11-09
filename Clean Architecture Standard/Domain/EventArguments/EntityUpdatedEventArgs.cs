using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Core.Domain.EventArguments
{
    /// <summary>
    /// Contains event info with an entity value that has been updated and a list of names of the updated properties on that entity..
    /// NOT intended for direct use; inherit from this class. 
    /// </summary>
    /// <typeparam name="T">The type of the entity the args contains.</typeparam>
    public abstract class EntityUpdatedEventArgs<T> : EntityEventArgs<T>
    {
        private List<string> _updatedProperties;
        /// <summary>
        /// A list of names of the properties that have been updated. Empty list if no properties were updated.
        /// </summary>
        public List<string> UpdatedProperties
        {
            get { return new List<string>(this._updatedProperties); }
            private set { this._updatedProperties = value; }
        }

        /// <summary>
        /// Constructor sets the given value and list of names of properties that have been updated in the given value.
        /// </summary>
        /// <param name="value">The value to set.</param>
        /// <param name="updatedProperties">The list of names of properties that have been updated in the given value.</param>
        public EntityUpdatedEventArgs(T value, List<string> updatedProperties)
            : base(value)
        {
            // Assign the given list. If the given list is null, assign an empty list.
            this._updatedProperties = updatedProperties == null ? new List<string>() : new List<string>(updatedProperties);
        }
    }
}
