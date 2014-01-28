
namespace Internode.WebTools.Domain.Models
{
    class Traffic
    {
        public string Name { get; set; }
        public string Unit { get; set; }
        public string Value { get; set; }

        public string Rollover { get; set; }
        // ReSharper disable once InconsistentNaming
        public string planinterval { get; set; }        // Not recognised by RestSharp if using "PlanInterval" as the property name (bug?)
        public string Quota { get; set; }

    }
}
