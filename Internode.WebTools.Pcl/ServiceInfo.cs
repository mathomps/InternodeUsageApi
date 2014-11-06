using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Internode.WebTools.Pcl
{

    /// <summary>
    /// Represents an individual service belonging to a customer.
    /// </summary>
    public class ServiceInfo
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public long Quota { get; set; }
        public string Plan { get; set; }
        public string Carrier { get; set; }
        public string UsageRating { get; set; }
        public DateTime Rollover { get; set; }
        public string ExcessCost { get; set; }
        public string ExcessCharged { get; set; }
        public string ExcessShaped { get; set; }
        public string ExcessRestrictAccess { get; set; }
        public string PlanInterval { get; set; }
        public string PlanCost { get; set; }
    }

    /// <summary>
    /// An ADSL service that belongs to a customer.
    /// </summary>
    public class AdslServiceInfo : ServiceInfo
    {
        public string Speed { get; set; }
    }

    // A NodePhone service that belongs to a customer.
    public class MobileServiceInfo : ServiceInfo
    {
        public string Sim { get; set; }
    }

}
