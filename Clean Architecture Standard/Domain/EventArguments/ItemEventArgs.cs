using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Core.Domain.EventArguments
{
    /// <summary>
    /// Contains event info with an entity value.
    /// Use this as a generic event args. Manual casting of the stored value is needed.
    /// </summary>
    public class ItemEventArgs : EntityEventArgs<object>
    {
        /// <summary>
        /// The item.
        /// </summary>
        public object Item { get => this.Value; }

        /// <summary>
        /// Creates a new instance of the event args.
        /// </summary>
        /// <param name="item">The item the event was fired for.</param>
        public ItemEventArgs(object item) : base(item) { }
    }
}
