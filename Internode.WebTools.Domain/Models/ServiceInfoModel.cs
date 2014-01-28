using System;

namespace Internode.WebTools.Domain.Models
{
    class ServiceInfoModel
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

    class AdslServiceInfoModel : ServiceInfoModel
    {
        public string Speed { get; set; }
    }

    class MobileServiceInfoModel : ServiceInfoModel
    {
        public string Sim { get; set; }
    }

}
