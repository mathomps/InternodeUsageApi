using System.Collections.Generic;

namespace Internode.WebTools.Domain.Models
{
    class DataUsageModel
    {
        public string Day { get; set; }
        public List<TrafficModel> Traffic { get; set; }
    }

    class TrafficModel
    {
        public string Direction { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }

}
