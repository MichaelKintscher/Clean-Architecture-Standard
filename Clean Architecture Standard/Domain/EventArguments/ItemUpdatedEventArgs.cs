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
        public object Item { get => this.Value; }

        /// <summary>
        /// Creates a new instance of the event args.
        /// </summary>
        /// <param name="item">The item that was updated.</param>
        /// <param name="updatedProperties">The list of names of properties that have been updated on the item.</param>
        public ItemUpdatedEventArgs(object item, List<string> updatedProperties) : base(item, updatedProperties) { }
    }
}
