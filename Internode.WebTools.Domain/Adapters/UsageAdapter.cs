using System.Collections.Generic;
using Internode.WebTools.Domain.Models;

namespace Internode.WebTools.Domain.Adapters
{
    internal static class UsageAdapter
    {
        internal static ServiceUsage FromServiceUsageModel(IEnumerable<Traffic> serviceUsages)
        {
            var result = new ServiceUsage();

            foreach (var model in serviceUsages)
            {
                switch (model.Name)
                {
                    case "metered":
                        result.MeteredBytes = long.Parse(model.Value);
                        break;
                    case "unmetered":
                        result.UnmeteredBytes = long.Parse(model.Value);
                        break;
                    case "total":
                        result.TotalBytes = long.Parse(model.Value);
                        result.PlanInterval = model.planinterval;
                        result.Quota = long.Parse(model.Quota);
                        break;
                }
            }
            
            return result;

        }

    }
}
