using CleanArchitecture.Core.Application;
using CleanArchitecture.Core.Domain;
using CleanArchitecture.Core.Domain.EventArguments;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.Networking.Connectivity;

namespace Network
{
    /// <summary>
    /// Implements the connectivity service provider functions for Windows.
    /// </summary>
    public class WindowsConnectivityService : Singleton<WindowsConnectivityService>, IConnectivityService
    {
        #region Properties
        private bool isConnected;
        /// <summary>
        /// Whether there is a network connection.
        /// </summary>
        public bool IsConnected
        {
            get => this.isConnected;
        }
        #endregion

        #region Events
        /// <summary>
        /// Raised when a connection is established.
        /// </summary>
        public event IConnectivityService.ConnectionStatusChangedHandler ConnectionEstablished;
        private void RaiseConnectionEstablished()
        {
            // Create the args and call the listening event handlers, if there are any.
            ConnectionStatusChangedEventArgs args = new ConnectionStatusChangedEventArgs(true);
            this.ConnectionEstablished?.Invoke(this, args);
        }

        /// <summary>
        /// Risd when a connection is lost.
        /// </summary>
        public event IConnectivityService.ConnectionStatusChangedHandler ConnectionLost;
        private void RaiseConnectionLost()
        {
            // Create the args and call the listening event handlers, if there are any.
            ConnectionStatusChangedEventArgs args = new ConnectionStatusChangedEventArgs(false);
            this.ConnectionLost?.Invoke(this, args);
        }
        #endregion

        #region Constructors
        public WindowsConnectivityService()
        {
            // Initialize the connection status.
            this.isConnected = this.TestConnection();

            // Wire up the event handler to the Windows Network Status Changed event.
            NetworkInformation.NetworkStatusChanged += NetworkInformation_NetworkStatusChanged;
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Handles the Windows Network Status Changed event.
        /// </summary>
        /// <param name="sender"></param>
        private void NetworkInformation_NetworkStatusChanged(object sender)
        {
            // Capture the update the old connectivity state.
            bool previousConnectivity = this.isConnected;
            this.isConnected = this.TestConnection();

            // Raise the appropriate network status changed event.
            if (previousConnectivity != this.isConnected)
            {
                if (this.isConnected) this.RaiseConnectionEstablished();
                else this.RaiseConnectionLost();
            }
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Returns whether there is full internet access or not.
        /// </summary>
        /// <returns></returns>
        private bool TestConnection()
        {
            bool connection = false;

            // Attempt to get the connection profile.
            ConnectionProfile profile = NetworkInformation.GetInternetConnectionProfile();

            // If no profile was found, then there is no network connection.
            if (profile != null)
            {
                connection = false;
            }

            // A profile was found. Check the network connectivity level.
            NetworkConnectivityLevel networkConnectivityLevel = profile.GetNetworkConnectivityLevel();
            switch (networkConnectivityLevel)
            {
                // No connection detected... change to offline mode.
                case NetworkConnectivityLevel.None:
                    connection = false;
                    break;
                case NetworkConnectivityLevel.LocalAccess:
                    connection = false;
                    break;
                case NetworkConnectivityLevel.ConstrainedInternetAccess:
                    connection = false;
                    break;
                case NetworkConnectivityLevel.InternetAccess:
                    connection = true;
                    break;
                default:
                    connection = false;
                    break;
            }

            return connection;
        }
        #endregion
    }
}
