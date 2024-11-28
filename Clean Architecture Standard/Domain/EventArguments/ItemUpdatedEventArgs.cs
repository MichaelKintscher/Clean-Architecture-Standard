using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Core.Domain.EventArguments
{
    /// <summary>
    /// Contains event info with an updated item.
    /// Use this as a generic event args. Manual casting of the stored value is needed.
    /// </summary>
    public class ItemUpdatedEventArgs : EntityUpdatedEventArgs<object>
    {
        /// <summary>
        /// The updated item.
        /// </summary>
        public object Item { get => this.UpdatedValue; }

        /// <summary>
        /// The item before it was updated.
        /// </summary>
        public object? OriginalItem { get => OriginalValue; }

        /// <summary>
        /// Creates a new instance of the event args.
        /// </summary>
        /// <param name="item">The item that was updated. Recommended: Use the original reference of the item (not a copy), as this will help data-bindings work properly.</param>
        /// <param name="updatedProperties">The list of names of properties that have been updated on the item.</param>
        public ItemUpdatedEventArgs(object item, List<string> updatedProperties) : base(item, updatedProperties) { }

        /// <summary>
        /// Creates a new instance of the event args.
        /// </summary>
        /// <param name="item">The item that was updated. Recommended: Use the original reference of the item (not a copy), as this will help data-bindings work properly.</param>
        /// <param name="updatedItem">The item before it was updated. Recommended: Use a copy of the item.</param>
        /// <param name="updatedProperties">The list of names of properties that have been updated on the item.</param>
        public ItemUpdatedEventArgs(object item, object updatedItem, List<string> updatedProperties) : base(item, updatedItem, updatedProperties) { }
    }
}
