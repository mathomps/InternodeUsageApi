using Internode.WebTools.Domain.Models;

namespace Internode.WebTools.Domain.Adapters
{
    internal static class ServiceAdapter
    {
        internal static AdslServiceInfo FromAdslServiceModel(AdslServiceInfoModel adslModel)
        {
            var result = new AdslServiceInfo();
            UpdateServiceInfoFromModel(result, adslModel);
            result.Speed = adslModel.Speed;
            return result;
        }

        internal static MobileServiceInfo FromMobileServiceInfoModel(MobileServiceInfoModel mobileModel)
        {
            var result = new MobileServiceInfo();
            UpdateServiceInfoFromModel(result, mobileModel);
            result.Sim = mobileModel.Sim;
            return result;

        }


        private static void UpdateServiceInfoFromModel(ServiceInfo info, ServiceInfoModel model)
        {
            info.Id = model.Id;
            info.Username = model.Username;
            info.Quota = model.Quota;
            info.Plan = model.Plan;
            info.Carrier = model.Carrier;
            info.UsageRating = model.UsageRating;
            info.Rollover = model.Rollover;
            info.ExcessCost = model.ExcessCost;
            info.ExcessCharged = model.ExcessCharged;
            info.ExcessShaped = model.ExcessShaped;
            info.ExcessRestrictAccess = model.ExcessRestrictAccess;
            info.PlanInterval = model.PlanInterval;
            info.PlanCost = model.PlanCost;
        }

    }
}
