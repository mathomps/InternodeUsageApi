using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Internode.WebTools.Pcl
{
    /// <summary>
    /// Represents an Internode Service that a customer has
    /// </summary>
    public class InternodeService
    {
        public InternodeService(string serviceType, string serviceId, string serviceEndpoint)
        {
            ServiceId = serviceId;
            ServiceEndpoint = serviceEndpoint;

            switch (serviceType.ToLowerInvariant())
            {
                case "personal_adsl":
                    ServiceType = ServiceType.PersonalAdsl;
                    break;

                case "nodemobile":
                    ServiceType = ServiceType.NodeMobile;
                    break;
                default:
                    throw new ArgumentException("Unsupported service type", "serviceType");
            }
        }
        public ServiceType ServiceType { get; set; }
        public string ServiceId { get; set; }
        public string ServiceEndpoint { get; set; }
    }

    public enum ServiceType
    {
        PersonalAdsl,
        NodeMobile
    }
}
