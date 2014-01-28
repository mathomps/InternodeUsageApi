using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Internode.WebTools.Domain
{
    /// <summary>
    /// Represents an Internode Service that a customer has
    /// </summary>
    public class InternodeService
    {
        public string ServiceType { get; set; }
        public string ServiceId { get; set; }
        public string ServiceEndpoint { get; set; }
    }
}
