﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitectureEntities.Messaging
{
    /// <summary>
    /// Model for an email message.
    /// </summary>
    public class Email
    {
        /// <summary>
        /// The unique ID of the message given by the app.
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// The ID of the message given by the email provider's API.
        /// </summary>
        public string ProviderGivenID { get; set; }

        /// <summary>
        /// The subject of the email.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// The body of the email.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Default constructor - creates an email with an empty string subject and body.
        /// </summary>
        public Email()
        {
            this.Subject = string.Empty;
            this.Body = string.Empty;
        }
    }
}