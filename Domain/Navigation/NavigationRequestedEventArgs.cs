using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArchitecture.Windows.Navigation
{
    /// <summary>
    /// Contains event info for a navigation request.
    /// </summary>
    internal class NavigationRequestedEventArgs : EventArgs
    {
        /// <summary>
        /// The string value representing the page being reqested as the target of the navigation.
        /// </summary>
        public string ToPage { get; private set; }

        /// <summary>
        /// A flag indicating whether this navigation request is a back navigation request or not.
        /// </summary>
        public bool IsBackRequest { get; private set; }

        public NavigationRequestedEventArgs(string toPage, bool isBackRequest)
        {
            this.ToPage = toPage;
            this.IsBackRequest = isBackRequest;
        }
    }
}
