using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Core.Domain.EventArguments
{
    /// <summary>
    /// Contains event info with an entity value that has been updated and a list of names of the updated properties on that entity.
    /// NOT intended for direct use; inherit from this class. 
    /// </summary>
    /// <typeparam name="T">The type of the entity the args contains. Must be a reference type (nullable or non-nullable).</typeparam>
    public abstract class EntityUpdatedEventArgs<T> : EntityEventArgs<T> where T : class?
    {
        /// <summary>
        /// The entity with the properties updated.
        /// </summary>
        protected T UpdatedValue { get => this.Value; }

        /// <summary>
        /// The entity before the properties were updated.
        /// </summary>
        protected T? OriginalValue { get; private set; }

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
        /// Constructor sets the given entity before and after updates, and list of names of properties that have been updated in the given entity.
        /// </summary>
        /// <param name="updatedValue">The entity to set. Recommended: Use the original reference of the item (not a copy), as this will help data-bindings work properly.</param>
        /// <param name="updatedProperties">The list of names of properties that have been updated in the given entity.</param>
        public EntityUpdatedEventArgs(T updatedValue, List<string> updatedProperties)
            : base(updatedValue)
        {
            // Assign the original value.
            this.OriginalValue = null;

            // Assign the given list. If the given list is null, assign an empty list.
            this._updatedProperties = updatedProperties == null ? new List<string>() : new List<string>(updatedProperties);
        }

        /// <summary>
        /// Constructor sets the given entity before and after updates, and list of names of properties that have been updated in the given entity.
        /// </summary>
        /// <param name="updatedValue">The entity to set. Recommended: Use the original reference of the entity (not a copy), as this will help data-bindings work properly.</param>
        /// <param name="originalValue">The entity before it was updated. Recommended: Use a copy of the item.</param>
        /// <param name="updatedProperties">The list of names of properties that have been updated in the given entity.</param>
        public EntityUpdatedEventArgs(T updatedValue, T originalValue, List<string> updatedProperties)
            : base(updatedValue)
        {
            // Assign the original value.
            this.OriginalValue = originalValue;

            // Assign the given list. If the given list is null, assign an empty list.
            this._updatedProperties = updatedProperties == null ? new List<string>() : new List<string>(updatedProperties);
        }
    }
}
