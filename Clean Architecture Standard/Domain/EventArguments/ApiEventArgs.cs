using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Core.Domain.EventArguments
{
    /// <summary>
    /// Contains event info for when an API event is raised.
    /// </summary>
    public class ApiEventArgs : EntityEventArgs<string>
    {
        /// <summary>
        /// The name of the Api the event was raised for.
        /// </summary>
        public string ApiName
        {
            get => this.Value;
        }

        public ApiEventArgs(string apiName)
            : base(apiName) { }
    }
}
