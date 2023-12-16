using CleanArchitecture.Core.Application;
using CleanArchitecture.Core.Domain;
using CleanArchitecture.Core.Domain.EventArguments;
using System;
using System.Collections.Generic;
using System.Text;

namespace Network
{
    /// <summary>
    /// Implements the connectivity service provider functions for Windows.
    /// </summary>
    public class WindowsConnectivityService : Singleton<WindowsConnectivityService>, IConnectivityService
    {
        public bool IsConnected { get; }

        public event IConnectivityService.ConnectionStatusChangedHandler ConnectionEstablished;
        private void RaiseConnectionEstablished()
        {
            // Create the args and call the listening event handlers, if there are any.
            ConnectionStatusChangedEventArgs args = new ConnectionStatusChangedEventArgs(true);
            this.ConnectionEstablished?.Invoke(this, args);
        }

        public event IConnectivityService.ConnectionStatusChangedHandler ConnectionLost;
        private void RaiseConnectionLost()
        {
            // Create the args and call the listening event handlers, if there are any.
            ConnectionStatusChangedEventArgs args = new ConnectionStatusChangedEventArgs(false);
            this.ConnectionLost?.Invoke(this, args);
        }
    }
}
