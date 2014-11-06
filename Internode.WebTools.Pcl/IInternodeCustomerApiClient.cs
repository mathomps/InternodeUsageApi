using System.Collections.Generic;
using System.Threading.Tasks;

namespace Internode.WebTools.Pcl
{
    public interface IInternodeCustomerApiClient
    {
        /// <summary>
        /// Provide authentication credentials to use with all subsequent requests to the API
        /// </summary>
        /// <param name="username">Internode account username (excluding @internode.on.net)</param>
        /// <param name="password">Password for the account</param>
        void SetCredentials(string username, string password);

        /// <summary>
        /// Performs an initial query for all services that a customer has.
        /// </summary>
        /// <returns></returns>
        Task QueryForServicesAsync();

        /// <summary>
        /// Retrieves ADSL service information for the specified ServiceId
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        Task<AdslServiceInfo> GetAdslServiceInfoAsync(string serviceId);

        /// <summary>
        /// Retrieves NodeMobile service information for the specified ServiceId
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        Task<MobileServiceInfo> GetMobileServiceInfoAsync(string serviceId);

        /// <summary>
        /// Retrieves basic usage information for the specified service
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        Task<ServiceUsage> GetServiceUsageAsync(string serviceId);

        /// <summary>
        /// Retrieves detailed usage information for the specified service
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        Task<ServiceHistory> GetServiceHistoryAsync(string serviceId);

        /// <summary>
        /// A list of services that is populated after calling QueryForServicesAsync()
        /// </summary>
        List<InternodeService> Services { get; }
    }
}