using System.Collections.Generic;
using System.Threading.Tasks;

namespace Internode.WebTools.Pcl
{
    public interface IInternodeCustomerApiClient
    {
        void SetCredentials(string username, string password);

        List<InternodeService> Services { get; }
        Dictionary<string, IEnumerable<InternodeServiceResource>> ServiceResources { get; set; }
        Task QueryForServices();
        Task<AdslServiceInfo> GetAdslServiceInfo(string serviceId);
    }
}