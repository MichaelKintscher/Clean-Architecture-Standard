using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Core.Domain.EventArguments
{
    /// <summary>
    /// Contains event info for when the connection status changes.
    /// </summary>
    public class ConnectionStatusChangedEventArgs : EntityEventArgs<bool>
    {
        /// <summary>
        /// Whether there is a network connection.
        /// </summary>
        public bool IsConnected
        {
            get => this.Value;
        }

        public ConnectionStatusChangedEventArgs(bool isConnected)
            : base(isConnected) { }
    }
}
