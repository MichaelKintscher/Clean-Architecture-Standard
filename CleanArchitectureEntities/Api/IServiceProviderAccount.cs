using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Entities.Api
{
    /// <summary>
    /// Represents an account with an API service provider.
    /// </summary>
    public interface IServiceProviderAccount
    {
        /// <summary>
        /// The local unique ID of the account.
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// The unique ID of the account given by the service provider.
        /// </summary>
        public string ProviderGivenID { get; set; }

        /// <summary>
        /// The local unique ID of the account's service provder.
        /// </summary>
        public string ProviderID { get; set; }

        /// <summary>
        /// The local nickname for the account.
        /// </summary>
        public string FriendlyName { get; set; }

        /// <summary>
        /// The username for the account with the service provider.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// The uri for the profile picture online.
        /// </summary>
        public string PictureUri { get; set; }

        /// <summary>
        /// The local uri for the cahced profile picture. Use this is a fallback if PictureUri is inacessible.
        /// </summary>
        public string PictureLocalUri { get; set; }

        /// <summary>
        /// Whether the account is connected.
        /// </summary>
        public bool Connected { get; set; }

        /// <summary>
        /// When data from this account was last synced.
        /// </summary>
        public DateTime LastSynced { get; set; }
    }
}
