using CleanArchitecture.Core.Domain.EventArguments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Core.Application
{
    public interface IConnectivityService
    {
        /// <summary>
        /// Whether there is a network connection.
        /// </summary>
        public bool IsConnected { get; }

        #region Events
        public delegate void ConnectionStatusChangedHandler(object sender, ConnectionStatusChangedEventArgs e);
        /// <summary>
        /// Raised when a connection is established.
        /// </summary>
        public event ConnectionStatusChangedHandler ConnectionEstablished;
        /// <summary>
        /// Risd when a connection is lost.
        /// </summary>
        public event ConnectionStatusChangedHandler ConnectionLost;
        #endregion
    }
}
