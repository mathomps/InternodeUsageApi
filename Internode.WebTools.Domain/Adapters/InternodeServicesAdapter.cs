using System.Collections.Generic;
using System.Linq;
using Internode.WebTools.Domain.Models;

namespace Internode.WebTools.Domain.Adapters
{
    internal static class InternodeServicesAdapter
    {
        internal static IEnumerable<InternodeService> FromInternodeServiceModel(IEnumerable<InternodeServiceModel> model)
        {
            var result = model.Select(FromInternodeServiceModel);
            return result;
        }

        static InternodeService FromInternodeServiceModel(InternodeServiceModel model)
        {
            return new InternodeService
            {
                ServiceType = model.Type,
                ServiceId = model.Value,
                ServiceEndpoint = model.Href
            };
        }
    }
}
