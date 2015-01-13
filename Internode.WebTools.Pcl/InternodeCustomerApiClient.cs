using Internode.WebTools.Pcl.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Internode.WebTools.Pcl
{
    public class InternodeCustomerApiClient : IInternodeCustomerApiClient
    {
        private HttpClient client;

        public InternodeCustomerApiClient()
        {
            // Initialise properties
            ServiceResources = new Dictionary<string, IEnumerable<InternodeServiceResource>>();
        }

        public void SetCredentials(string username, string password)
        {
            var credentials = new NetworkCredential(username, password);
            var handler = new HttpClientHandler { Credentials = credentials };

            client = new HttpClient(handler) {
                BaseAddress = new Uri("https://customer-webtools-api.internode.on.net/")
            };
            client.DefaultRequestHeaders.Add("user-agent", "IntranetUsageMeter/0.01 (in development)");
            
        }

        public Task<ServiceUsage> GetServiceUsageAsync(string serviceId)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceHistory> GetServiceHistoryAsync(string serviceId)
        {
            throw new NotImplementedException();
        }

        private List<InternodeService> Services { get; set; }

        private Dictionary<string, IEnumerable<InternodeServiceResource>> ServiceResources { get; set; }



        // Public API

        public async Task<List<InternodeService>> QueryForServicesAsync()
        {
            var xdoc = await ReadEndpoint("api/v1.5/");

            var services = xdoc.Descendants("service").ToList();

            Services = services.Select(GetServiceFromServiceElement)
                               .ToList();
            return Services;
        }


        public async Task<AdslServiceInfo> GetAdslServiceInfoAsync(string serviceId)
        {
            var adslService = Services.SingleOrDefault(s => s.ServiceId == serviceId &&
                                                            s.ServiceType == ServiceType.PersonalAdsl);
            if (adslService == null)
            {
                throw new ServiceNotFoundException();
            }

            await GetServiceResources(adslService);

            var infoEndpoint = ServiceResources[serviceId].Single(sr => sr.Type == "service").Endpoint;

            var xdoc = await ReadEndpoint(infoEndpoint);

            var serviceInfo = xdoc.Descendants("service").Single();

            var result = new AdslServiceInfo {
                Id = int.Parse(serviceInfo.Descendants("id").Single().Value),
                Username = serviceInfo.Descendants("username").Single().Value,
                Quota = long.Parse(serviceInfo.Descendants("quota").Single().Value),
                Plan = serviceInfo.Descendants("plan").Single().Value,
                Carrier = serviceInfo.Descendants("carrier").Single().Value,
                Speed = serviceInfo.Descendants("speed").Single().Value,
                UsageRating = serviceInfo.Descendants("usage-rating").Single().Value,
                Rollover = DateTime.Parse(serviceInfo.Descendants("rollover").Single().Value),

                ExcessCharged = serviceInfo.Descendants("excess-charged").Single().Value,
                ExcessShaped = serviceInfo.Descendants("excess-shaped").Single().Value,
                ExcessRestrictAccess = serviceInfo.Descendants("excess-restrict-access").Single().Value,
                PlanInterval = serviceInfo.Descendants("plan-interval").Single().Value,
                PlanCost = serviceInfo.Descendants("plan-cost").Single().Value,
            };

            // 'excess-cost' will only be present if an excess is charged!
            if (result.ExcessCharged == "yes")
            {
                result.ExcessCost = serviceInfo.Descendants("excess-cost").Single().Value;
            }

            return result;

        }

        public Task<MobileServiceInfo> GetMobileServiceInfoAsync(string serviceId)
        {
            throw new NotImplementedException();
        }

        private async Task GetServiceResources(InternodeService service)
        {
            if (ServiceResources.ContainsKey(service.ServiceId)) return;        // Already got the data, don't query again.

            var xdoc = await ReadEndpoint(service.ServiceEndpoint);

            var resources = xdoc.Descendants("resource");

            var serviceResources = resources.Select(resource => new InternodeServiceResource {
                Type = resource.Attribute("type").Value,
                Endpoint = resource.Attribute("href").Value
            });

            ServiceResources.Add(service.ServiceId, serviceResources);
        }

        // Internal Helpers

        private async Task<XDocument> ReadEndpoint(string endpointAddress)
        {
            HttpResponseMessage response = null;
            var retryCount = 3;

            while (response == null && retryCount > 0)
            {
                try
                {
                    response = await GetResponseMessageAsync(endpointAddress);
                }
                catch (ServerErrorException)
                {
                    var re = new ManualResetEvent(initialState: false);
                    re.WaitOne(2000);
                    retryCount--;
                }
            }

            if (response == null) return null;

            var resultStream = await response.Content.ReadAsStreamAsync();
            return XDocument.Load(resultStream);
        }

        private async Task<HttpResponseMessage> GetResponseMessageAsync(string uri)
        {
            var response = await client.GetAsync(uri);
            OnResponseReceived(response);
            return response;
        }

        /// <summary>
        /// Automatically throw exceptions if the response is not valid for any reason.
        /// </summary>
        /// <param name="response"></param>
        private void OnResponseReceived(HttpResponseMessage response)
        {
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    throw new AuthenticationException("Authorisation failed");

                case HttpStatusCode.InternalServerError:
                    throw new ServerErrorException("Internal Server error");

            }

            // Generic catch-all for status codes other than 200 (OK)
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(response.StatusCode.ToString());
            }

        }



        public InternodeService GetServiceFromServiceElement(XElement serviceElement)
        {
            return new InternodeService(
                serviceType: serviceElement.Attribute("type").Value,
                serviceId: serviceElement.Value,
                serviceEndpoint: serviceElement.Attribute("href").Value
                );
        }


    }

    public class InternodeServiceResource
    {
        public string Type { get; set; }
        public string Endpoint { get; set; }
    }
}
