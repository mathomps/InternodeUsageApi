using System.Collections.Generic;

namespace Internode.WebTools.Domain.Models
{
    class InternodeServicesModel
    {
        public List<InternodeServiceModel> Services { get; set; }
    }

    class InternodeServiceModel
    {
        public string Type { get; set; }
        public string Href { get; set; }
        public string Value { get; set; }
    }

}
